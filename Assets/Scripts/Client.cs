using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorldItem {
  public int itemId;
  public Vector3 position;
}

public class Client : MonoBehaviour {
  private const int MAX_CONNECTION = 100;

  private int hostId;
  private int webHostId;

  private int reliableChannel;
  private int unreliableChannel;

  public int ourClientId;
  private int connectionId;

  private float connectionTime;
  private bool isConnected = false;
  private bool isStarted = false;
  private byte error;

  private string playerName;
  private int playerId;

  public GameObject playerPrefab;
  public Dictionary<int, Player> players = new Dictionary<int, Player>();

  public GameObject itemPrefab;

  private WWWForm form;
  private string loginEndpoint = "login.php";

  public float timeBetweenMovementStart;
  public float timeBetweenMovementEnd;

  private Text errorText;
  private bool isDeveloper = false;
  public GameObject devCanvasPrefab;

  public bool paused = false;
  public bool inMenu = false;

  public List<Item> items = new List<Item> {
    new HealthPotion(
      0,
      "health potion",
      1,
      10,
      20
    ),
    new Weapon(
      1,
      "generic weapon",
      10,
      20,
      15
    ),
    new Apparel(
      2,
      "generic armor",
      5,
      10,
      10
    )
  };
  
  public Dictionary<int, GameObject> worldItems = new Dictionary<int, GameObject>();

  public void Connect() {
    errorText = GameObject.Find("ErrorText").GetComponent<Text>();
    errorText.text = "Logging in...";
    StartCoroutine("RequestLogin");
  }

  public IEnumerator RequestLogin() {
    string username = GameObject.Find("UsernameInput").GetComponent<InputField>().text;
    string password = GameObject.Find("PasswordInput").GetComponent<InputField>().text;

    form = new WWWForm();
    form.AddField("username", username);
    form.AddField("password", password);

    WWW w = new WWW(NetworkSettings.API + loginEndpoint, form);
    yield return w;

    if (string.IsNullOrEmpty(w.error)) {
      User user = JsonUtility.FromJson<User>(w.text);
      if (user.success) {
        if (user.error != "") {
          errorText.text = user.error;
        } else {
          errorText.text = "Login successful";
          playerName = user.username;
          playerId = user.user_id;
          JoinGame();
        }
      } else {
        errorText.text = user.error;
      }
    } else {
      errorText.text = w.error;
    }
  }

  public void JoinGame() {
    NetworkTransport.Init();
    ConnectionConfig cc = new ConnectionConfig();

    reliableChannel = cc.AddChannel(QosType.Reliable);
    unreliableChannel = cc.AddChannel(QosType.Unreliable);

    HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

    hostId = NetworkTransport.AddHost(topo, 0);
    connectionId = NetworkTransport.Connect(hostId, NetworkSettings.ADDRESS, NetworkSettings.PORT, 0, out error);

    connectionTime = Time.time;
    timeBetweenMovementStart = Time.time;
    isConnected = true;
  }

  public void JoinOfflineGame() {
    playerName = GameObject.Find("UsernameOptionalInput").GetComponent<InputField>().text;
    ourClientId = -1;
    SpawnPlayer(playerName, ourClientId);
  }

  private void Start() {
    if (Application.isEditor) {
      isDeveloper = true;
    }

    GameObject.Find("PauseCanvas").GetComponent<Canvas>().enabled = false;
    GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = false;
  }

  private void Update() {
    if (isStarted) {
      if (Input.GetButtonDown("Cancel") && !inMenu) {
        TogglePause();
      }

      if (Input.GetKeyDown(KeyCode.F) && !paused) {
        ToggleMenu();
      }

      if (isDeveloper && !paused && !inMenu) {
        if (Input.GetKeyDown(KeyCode.I)) {
          ToggleCamera();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
          ToggleMovement();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
          ToggleAttacks();
        }
      }
    }

    if (!isConnected) {
      return;
    }

    int recHostId;
    int connectionId;
    int channelId;
    byte[] recBuffer = new byte[1024];
    int bufferSize = 1024;
    int dataSize;
    byte error;
    NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
      bufferSize, out dataSize, out error);
    switch (recData) {
      case NetworkEventType.DataEvent:
        string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
//        Debug.Log("Receiving : " + msg);
        string[] splitData = msg.Split('|');
        switch (splitData[0]) {
          case "ASKNAME":
            OnAskName(splitData);
            break;
          case "ITEMS":
            OnItems(splitData);
            break;
          case "INVENTORY":
            OnInventory(splitData);
            break;
          case "CNN":
            SpawnPlayer(splitData[1], int.Parse(splitData[2]));
            break;
          case "DC":
            PlayerDisconnected(int.Parse(splitData[1]));
            break;
          case "ASKPOSITION":
            OnAskPosition(splitData);
            break;
          case "CHARGE":
            OnCharge(int.Parse(splitData[1]));
            break;
          case "ATK":
            OnAttack(int.Parse(splitData[1]));
            break;
          case "HIT":
            OnHit(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]));
            break;
          case "USE":
            OnUse(int.Parse(splitData[1]), int.Parse(splitData[2]));
            break;
          case "DROP":
            OnDrop(int.Parse(splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), float.Parse(splitData[4]), float.Parse(splitData[5]), float.Parse(splitData[6]));
            break;
          case "PICKUP":
            OnPickup(int.Parse(splitData[1]), int.Parse(splitData[2]));
            break;
          default:
            Debug.Log("Invalid message : " + msg);
            break;
        }

        break;
    }
  }

  private void OnAskName(string[] data) {
    // Set this client's ID
    ourClientId = int.Parse(data[1]);

    // Send our name to the server
    Send("NAMEIS|" + playerName + "|" + playerId, reliableChannel);

    // Create all the other players
    for (int i = 2; i < data.Length - 1; i++) {
      string[] d = data[i].Split('%');
      SpawnPlayer(d[0], int.Parse(d[1]));
    }
  }

  private void OnItems(string[] data) {
    // Create all items
    for (int i = 1; i < data.Length; i++) {
      string[] d = data[i].Split('%');
      SpawnItem(int.Parse(d[0]), int.Parse(d[1]), float.Parse(d[2]), float.Parse(d[3]), float.Parse(d[4]));
    }
  }
  
  private void OnInventory(string[] data) {
    for (int i = 1; i < data.Length; i++) {
      players[ourClientId].inventory.Add(int.Parse(data[i]));
    }
  }

  private void OnAskPosition(string[] data) {
    if (!isStarted) {
      return;
    }

    // Update everyone else
    for (int i = 1; i < data.Length; i++) {
      string[] d = data[i].Split('%');

      // Prevent the server from updating us
      if (ourClientId != int.Parse(d[0])) {
        Vector3 position = Vector3.zero;
        position.x = float.Parse(d[1]);
        position.y = float.Parse(d[2]);
        position.z = float.Parse(d[3]);

        Quaternion rotation = Quaternion.identity;
        rotation.w = float.Parse(d[4]);
        rotation.x = float.Parse(d[5]);
        rotation.y = float.Parse(d[6]);
        rotation.z = float.Parse(d[7]);

        players[int.Parse(d[0])].lastRealPosition = players[int.Parse(d[0])].realPosition;
        players[int.Parse(d[0])].lastRealRotation = players[int.Parse(d[0])].realRotation;

        players[int.Parse(d[0])].realPosition = position;
        players[int.Parse(d[0])].realRotation = rotation;

        players[int.Parse(d[0])].timeToLerp = float.Parse(d[8]);
        if (players[int.Parse(d[0])].realPosition != players[int.Parse(d[0])].avatar.transform.position) {
          players[int.Parse(d[0])].isLerpingPosition = true;
        }

        if (players[int.Parse(d[0])].realRotation != players[int.Parse(d[0])].avatar.transform.rotation) {
          players[int.Parse(d[0])].isLerpingRotation = true;
        }

        players[int.Parse(d[0])].timeStartedLerping = Time.time;
      }
    }

    // Send our own position
    Vector3 myPosition = players[ourClientId].avatar.transform.position;
    Quaternion myRotation = players[ourClientId].avatar.transform.rotation;
    timeBetweenMovementEnd = Time.time;
    string m = "MYPOSITION|" + myPosition.x.ToString() + '|' + myPosition.y.ToString() + '|' + myPosition.z.ToString() +
               '|' + myRotation.w + '|' + +myRotation.x + '|' + +myRotation.y + '|' + +myRotation.z + '|' +
               (timeBetweenMovementEnd - timeBetweenMovementStart).ToString();
    Send(m, unreliableChannel);
    timeBetweenMovementStart = Time.time;
  }

  private void OnCharge(int cnnId) {
    if (cnnId != ourClientId) {
      players[cnnId].avatar.transform.Find("Arms").GetComponent<PlayerSwing>().Charge();
    }
  }

  private void OnAttack(int cnnId) {
    if (cnnId != ourClientId) {
      players[cnnId].avatar.transform.Find("Arms").GetComponent<PlayerSwing>().Attack();
    }
  }

  private void OnHit(int cnnId, int hitId, int damage) {
    players[hitId].avatar.GetComponent<PlayerController>().TakeDamage(damage);
  }

  private void OnUse(int cnnId, int itemId) {
    if (cnnId != ourClientId) {
      items[itemId].Use(players[cnnId]);
    }
  }

  private void OnDrop(int cnnId, int worldId, int itemId, float x, float y, float z) {
    SpawnItem(worldId, itemId, x, y, z);
    if (inMenu) {
      GameObject.Find("MenuCanvas").GetComponent<MenuController>().RefreshMenu();
    }
  }
  
  private void OnPickup(int cnnId, int worldId) {
    players[cnnId].inventory.Add(worldItems[worldId].GetComponent<ItemController>().item_id);
    Destroy(worldItems[worldId]);
    worldItems.Remove(worldId);
  }

  private void SpawnItem(int worldId, int itemId, float x, float y, float z) {
    GameObject item = Instantiate(itemPrefab);
    item.transform.position = new Vector3(x, y, z);
    item.GetComponent<ItemController>().item_id = itemId;
    item.GetComponent<ItemController>().world_id = worldId;
    item.name = items[itemId].getName();
    worldItems.Add(worldId, item);
  }

  private void SpawnPlayer(string playerName, int cnnId) {
    GameObject go = Instantiate(playerPrefab);
    go.name = playerName;
    go.GetComponent<PlayerController>().id = cnnId;
    Player p = new Player(playerName, go, playerId, items);

    // Is this ours?
    if (cnnId == ourClientId) {
      Destroy(go.transform.Find("PlayerCanvas").gameObject);
      Destroy(go.transform.Find("Glasses").gameObject);
      go.AddComponent<PlayerMotor>();
      go.AddComponent<PlayerLook>();
      GameObject obj = go.transform.Find("Arms").gameObject;
      obj.AddComponent<Camera>();
      obj.AddComponent<AudioListener>();
      obj.AddComponent<CameraLook>();
      obj.AddComponent<PlayerAttackController>();
      obj.AddComponent<PlayerAttack>();
      obj.transform.parent = go.transform;
      GameObject.Find("Canvas").SetActive(false);
      if (isDeveloper) {
        Instantiate(devCanvasPrefab);
        GameObject.Find("LockCamera").GetComponent<Button>().onClick.AddListener(ToggleCamera);
        GameObject.Find("LockMovement").GetComponent<Button>().onClick.AddListener(ToggleMovement);
        GameObject.Find("LockAttacks").GetComponent<Button>().onClick.AddListener(ToggleAttacks);
      }

      GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(Quit);
      GameObject.Find("MyCanvas").transform.Find("HealthBar").GetComponent<Slider>().value = p.currentHealth;
      isStarted = true;
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    p.isLerpingPosition = false;
    p.isLerpingRotation = false;
    p.realPosition = p.avatar.transform.position;
    p.realRotation = p.avatar.transform.rotation;
    p.avatar.transform.Find("PlayerCanvas").GetComponentInChildren<Text>().text = playerName;
    p.avatar.transform.Find("PlayerCanvas").GetComponentInChildren<Slider>().value = p.currentHealth;
    players.Add(cnnId, p);
    int[] myArgs = {p.connectionId, cnnId};
//    StartCoroutine("GetItemsForUser", myArgs);
  }

  private void PlayerDisconnected(int cnnId) {
    Destroy(players[cnnId].avatar);
    players.Remove(cnnId);
  }

  private void Send(string message, int channelId) {
//    Debug.Log("Sending : " + message);
    byte[] msg = Encoding.Unicode.GetBytes(message);
    NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
  }

  public void SendReliable(string message) {
    if (isConnected) {
      Send(message, reliableChannel);
    }
  }

  private void FixedUpdate() {
    NetworkLerp();
  }

  private void NetworkLerp() {
    foreach (KeyValuePair<int, Player> player in players) {
      if (player.Value.playerName != playerName) {
        if (player.Value.isLerpingPosition) {
          float lerpPercentage = (Time.time - player.Value.timeStartedLerping) / player.Value.timeToLerp;

          player.Value.avatar.transform.position =
            Vector3.Lerp(player.Value.lastRealPosition, player.Value.realPosition, lerpPercentage);
        }

        if (player.Value.isLerpingRotation) {
          float lerpPercentage = (Time.time - player.Value.timeStartedLerping) / player.Value.timeToLerp;

          player.Value.avatar.transform.rotation =
            Quaternion.Lerp(player.Value.lastRealRotation, player.Value.realRotation, lerpPercentage);
        }
      }
    }
  }

  public void ToggleCamera() {
    if (players[ourClientId].avatar.GetComponent<PlayerLook>().isActiveAndEnabled &&
        players[ourClientId].avatar.GetComponentInChildren<CameraLook>().isActiveAndEnabled) {
      LockCamera();
    } else {
      UnlockCamera();
    }
  }

  public void ToggleMovement() {
    if (players[ourClientId].avatar.GetComponent<PlayerMotor>().isActiveAndEnabled) {
      LockMovement();
    } else {
      UnlockMovement();
    }
  }

  public void ToggleAttacks() {
    if (players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().isActiveAndEnabled &&
        players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().isActiveAndEnabled) {
      LockAttacks();
    } else {
      UnlockAttacks();
    }
  }

  private void LockCamera() {
    players[ourClientId].avatar.GetComponent<PlayerLook>().enabled = false;
    players[ourClientId].avatar.GetComponentInChildren<CameraLook>().enabled = false;
    if (isDeveloper) {
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Unlock Camera";
    }
  }

  private void LockMovement() {
    players[ourClientId].avatar.GetComponent<PlayerMotor>().enabled = false;
    if (isDeveloper) {
      GameObject.Find("LockMovement").GetComponentInChildren<Text>().text = "(o) Unlock Movement";
    }
  }

  private void LockAttacks() {
    players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().enabled = false;
    players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().enabled = false;
    if (isDeveloper) {
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Unlock Attacks";
    }
  }

  private void UnlockCamera() {
    players[ourClientId].avatar.GetComponent<PlayerLook>().enabled = true;
    players[ourClientId].avatar.GetComponentInChildren<CameraLook>().enabled = true;
    if (isDeveloper) {
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Lock Camera";
    }
  }

  private void UnlockMovement() {
    players[ourClientId].avatar.GetComponent<PlayerMotor>().enabled = true;
    if (isDeveloper) {
      GameObject.Find("LockMovement").GetComponentInChildren<Text>().text = "(o) Lock Movement";
    }
  }

  private void UnlockAttacks() {
    players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().enabled = true;
    players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().enabled = true;
    if (isDeveloper) {
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Lock Attacks";
    }
  }

  public void TogglePause() {
    if (paused) {
      paused = false;
      if (isDeveloper) {
        GameObject.Find("DevCanvas(Clone)").GetComponent<Canvas>().enabled = true;
      }

      GameObject.Find("PauseCanvas").GetComponent<Canvas>().enabled = false;
      GameObject.Find("MyCanvas").GetComponent<Canvas>().enabled = true;
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      UnlockCamera();
      UnlockMovement();
      UnlockAttacks();
    } else {
      if (isDeveloper) {
        GameObject.Find("DevCanvas(Clone)").GetComponent<Canvas>().enabled = false;
      }

      paused = true;
      GameObject.Find("PauseCanvas").GetComponent<Canvas>().enabled = true;
      GameObject.Find("MyCanvas").GetComponent<Canvas>().enabled = false;
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
      LockCamera();
      LockMovement();
      LockAttacks();
    }
  }

  public void ToggleMenu() {
    if (inMenu) {
      inMenu = false;
      if (isDeveloper) {
        GameObject.Find("DevCanvas(Clone)").GetComponent<Canvas>().enabled = true;
      }

      GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = false;
      GameObject.Find("MyCanvas").GetComponent<Canvas>().enabled = true;
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      UnlockCamera();
      UnlockMovement();
      UnlockAttacks();
      GameObject.Find("MenuCanvas").GetComponent<MenuController>().CloseMenu();
    } else {
      inMenu = true;
      if (isDeveloper) {
        GameObject.Find("DevCanvas(Clone)").GetComponent<Canvas>().enabled = false;
      }

      GameObject.Find("MenuCanvas").GetComponent<Canvas>().enabled = true;
      GameObject.Find("MyCanvas").GetComponent<Canvas>().enabled = false;
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
      LockCamera();
      LockMovement();
      LockAttacks();
      GameObject.Find("MenuCanvas").GetComponent<MenuController>().OpenMenu();
    }
  }

  public void Quit() {
    Application.Quit();
  }
}