using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

// This user class defines a networked player and contains data on their connection.
[Serializable]
public class User {
  public bool success;
  public string error;

  public string username;
  // Add username, etc....
}

// This player class defines a player character that each client keeps a list of (as well as the server).
// This is where each players physical gameobject is referenced.
public class Player {
  public string playerName;
  public GameObject avatar;
  public int connectionId;

  public bool isLerpingPosition;
  public bool isLerpingRotation;
  public Vector3 realPosition;
  public Quaternion realRotation;
  public Vector3 lastRealPosition;
  public Quaternion lastRealRotation;
  public float timeStartedLerping;
  public float timeToLerp;
}

public class Client : MonoBehaviour {
  private const int MAX_CONNECTION = 100;

  private int hostId;
  private int webHostId;

  private int reliableChannel;
  private int unreliableChannel;

  private int ourClientId;
  private int connectionId;

  private float connectionTime;
  private bool isConnected = false;
  private bool isStarted = false;
  private byte error;

  private string playerName;

  public GameObject playerPrefab;
  public Dictionary<int, Player> players = new Dictionary<int, Player>();

  private WWWForm form;
  private string loginEndpoint = "login.php";

  public float timeBetweenMovementStart;
  public float timeBetweenMovementEnd;

  public void Connect() {
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
          Debug.Log(user.error);
        } else {
          playerName = user.username;
          JoinGame();
        }
      } else {
        Debug.Log(user.error);
      }
    } else {
      Debug.Log(w.error);
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

  private void Update() {
    if (Application.isEditor && isStarted) {
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
        string[] splitData = msg.Split('|');
        switch (splitData[0]) {
          case "ASKNAME":
            OnAskName(splitData);
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
          case "ATK":
            OnAttack(int.Parse(splitData[1]), splitData[2]);
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
    Send("NAMEIS|" + playerName, reliableChannel);

    // Create all the other players
    for (int i = 2; i < data.Length - 1; i++) {
      string[] d = data[i].Split('%');
      SpawnPlayer(d[0], int.Parse(d[1]));
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

  private void OnAttack(int cnnId, string time) {
    float timeToReceive = Time.time - float.Parse(time);
    if (cnnId != ourClientId) {
      Debug.Log("Received message for client " + cnnId + " to attack after time " + timeToReceive);
      players[cnnId].avatar.transform.Find("Arms").GetComponent<PlayerSwing>().Attack();
    }
  }

  private void SpawnPlayer(string playerName, int cnnId) {
    GameObject go = Instantiate(Resources.Load("Player")) as GameObject;
    Player p = new Player();

    // Is this ours?
    if (cnnId == ourClientId) {
      Destroy(go.transform.Find("Glasses").gameObject);
      Destroy(go.transform.Find("NameTag").gameObject);
      go.AddComponent<PlayerMotor>();
      go.AddComponent<PlayerLook>();
      go.tag = "Player";
      GameObject obj = go.transform.Find("Arms").gameObject;
      obj.AddComponent<Camera>();
      obj.AddComponent<AudioListener>();
      obj.AddComponent<CameraLook>();
      obj.AddComponent<PlayerAttackController>();
      GameObject.Find("Canvas").SetActive(false);
      if (Application.isEditor) {
        Instantiate(Resources.Load("DevCanvas"));
      }
      GameObject.Find("LockCamera").GetComponent<Button>().onClick.AddListener(ToggleCamera);
      GameObject.Find("LockMovement").GetComponent<Button>().onClick.AddListener(ToggleMovement);
      GameObject.Find("LockAttacks").GetComponent<Button>().onClick.AddListener(ToggleAttacks);
      isStarted = true;
    }

    go.name = playerName;
    p.avatar = go;
    p.playerName = playerName;
    p.connectionId = cnnId;
    p.isLerpingPosition = false;
    p.isLerpingRotation = false;
    p.realPosition = p.avatar.transform.position;
    p.realRotation = p.avatar.transform.rotation;
    p.avatar.GetComponentInChildren<TextMesh>().text = playerName;
    players.Add(cnnId, p);
  }

  private void PlayerDisconnected(int cnnId) {
    Destroy(players[cnnId].avatar);
    players.Remove(cnnId);
  }

  private void Send(string message, int channelId) {
    byte[] msg = Encoding.Unicode.GetBytes(message);
    NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
  }

  public void SendReliable(string message) {
    if (isConnected) {
      Send(message, unreliableChannel);
    }
  }

  public void SendUnreliable(string message) {
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
    if (players[ourClientId].avatar.GetComponent<PlayerLook>().isActiveAndEnabled) {
      players[ourClientId].avatar.GetComponent<PlayerLook>().enabled = false;
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Unlock Camera";
    } else {
      players[ourClientId].avatar.GetComponent<PlayerLook>().enabled = true;
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Lock Camera";
    }

    if (players[ourClientId].avatar.GetComponentInChildren<CameraLook>().isActiveAndEnabled) {
      players[ourClientId].avatar.GetComponentInChildren<CameraLook>().enabled = false;
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Unlock Camera";
    } else {
      players[ourClientId].avatar.GetComponentInChildren<CameraLook>().enabled = true;
      GameObject.Find("LockCamera").GetComponentInChildren<Text>().text = "(i) Lock Camera";
    }
  }

  public void ToggleMovement() {
    if (players[ourClientId].avatar.GetComponent<PlayerMotor>().isActiveAndEnabled) {
      players[ourClientId].avatar.GetComponent<PlayerMotor>().enabled = false;
      GameObject.Find("LockMovement").GetComponentInChildren<Text>().text = "(o) Unlock Movement";
    } else {
      players[ourClientId].avatar.GetComponent<PlayerMotor>().enabled = true;
      GameObject.Find("LockMovement").GetComponentInChildren<Text>().text = "(o) Lock Movement";
    }
  }

  public void ToggleAttacks() {
    if (players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().isActiveAndEnabled) {
      players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().enabled = false;
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Unlock Attacks";
    } else {
      players[ourClientId].avatar.GetComponentInChildren<PlayerAttackController>().enabled = true;
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Lock Attacks";
    }

    if (players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().isActiveAndEnabled) {
      players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().enabled = false;
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Unlock Attacks";
    } else {
      players[ourClientId].avatar.GetComponentInChildren<PlayerSwing>().enabled = true;
      GameObject.Find("LockAttacks").GetComponentInChildren<Text>().text = "(p) Lock Attacks";
    }
  }
}