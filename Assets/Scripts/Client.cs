using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player {
  public string playerName;
  public GameObject avatar;
  public int connectionId;
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

  public void Connect() {
    Debug.Log("Logging in...");
    StartCoroutine("RequestLogin");
  }

  public IEnumerator RequestLogin() {
    string email = GameObject.Find("EmailInput").GetComponent<InputField>().text;
    string password = GameObject.Find("PasswordInput").GetComponent<InputField>().text;

    form = new WWWForm();
    form.AddField("email", email);
    form.AddField("password", password);
    
    WWW w = new WWW("http://localhost:8888/tmp/action_login.php", form);
    yield return w;

    if (string.IsNullOrEmpty(w.error)) {
      // success
      if (w.text.Contains("Invalid email or password")) {
        Debug.Log("Invalid email or password");
      } else {
        Debug.Log("Login success");
        JoinGame();
      }
    } else {
      // error
      Debug.Log("Login failure");
    }
  }

  public void JoinGame() {
    playerName = "MyName";
    
    NetworkTransport.Init();
    ConnectionConfig cc = new ConnectionConfig();

    reliableChannel = cc.AddChannel(QosType.Reliable);
    unreliableChannel = cc.AddChannel(QosType.Unreliable);
    
    HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

    hostId = NetworkTransport.AddHost(topo, 0);
    connectionId = NetworkTransport.Connect(hostId, NetworkSettings.ADDRESS, NetworkSettings.PORT, 0, out error);

    connectionTime = Time.time;
    isConnected = true;
  }
  
  private void Update() {
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
        Debug.Log("Receiving : " + msg);
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
        players[int.Parse(d[0])].avatar.transform.position = position;
      }
    }
    
    // Send our own position
    Vector3 myPosition = players[ourClientId].avatar.transform.position;
    string m = "MYPOSITION|" + myPosition.x.ToString() + '|' + myPosition.y.ToString() + '|' + myPosition.z.ToString();
    Send(m, unreliableChannel);
  }

  private void SpawnPlayer(string playerName, int cnnId) {
    GameObject go = Instantiate(playerPrefab) as GameObject;
    
    // Is this ours?
    if (cnnId == ourClientId) {

      go.AddComponent<PlayerMotor>();
      GameObject.Find("Canvas").SetActive(false);
      isStarted = true;
    }
    
    Player p = new Player();
    p.avatar = go;
    p.playerName = playerName;
    p.connectionId = cnnId;
    p.avatar.GetComponentInChildren<TextMesh>().text = playerName;
    players.Add(cnnId, p);
  }

  private void PlayerDisconnected(int cnnId) {
    Destroy(players[cnnId].avatar);
    players.Remove(cnnId);
  }
  
  private void Send(string message, int channelId) {
    Debug.Log("Sending : " + message);
    byte[] msg = Encoding.Unicode.GetBytes(message);
    NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
  }
}