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

  private int port = NetworkSettings.Port;

  private int hostId;
  private int webHostId;

  private int reliableChannel;
  private int unreliableChannel;

  private int clientId;
  private int connectionId;

  private float connectionTime;
  private bool isConnected = false;
  private bool isStarted = false;
  private byte error;

  private string playerName;

  public GameObject playerPrefab;
  public List<Player> players = new List<Player>();

  public void Connect() {
    // Does the player have a name?
    string pName = GameObject.Find("NameInput").GetComponent<InputField>().text;
    if (pName == "") {
      Debug.Log("You must enter a name!");
      return;
    }

    playerName = pName;
    
    NetworkTransport.Init();
    ConnectionConfig cc = new ConnectionConfig();

    reliableChannel = cc.AddChannel(QosType.Reliable);
    unreliableChannel = cc.AddChannel(QosType.Unreliable);
    
    HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

    hostId = NetworkTransport.AddHost(topo, 0);
    connectionId = NetworkTransport.Connect(hostId, NetworkSettings.Address, port, 0, out error);

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
    clientId = int.Parse(data[1]);
    
    // Send our name to the server
    Send("NAMEIS|" + playerName, reliableChannel);
    
    // Create all the other players
    for (int i = 2; i < data.Length - 1; i++) {
      string[] d = data[i].Split('%');
      SpawnPlayer(d[0], int.Parse(d[1]));
    }
  }

  private void SpawnPlayer(string playerName, int cnnId) {
    GameObject go = Instantiate(playerPrefab) as GameObject;
    
    // Is this ours?
    if (cnnId == clientId) {

      go.AddComponent<PlayerMotor>();
      GameObject.Find("Canvas").SetActive(false);
      isStarted = true;
    }
    
    Player p = new Player();
    p.avatar = go;
    p.playerName = playerName;
    p.connectionId = cnnId;
    p.avatar.GetComponentInChildren<TextMesh>().text = playerName;
    players.Add(p);
  }
  
  private void Send(string message, int channelId) {
    Debug.Log("Sending : " + message);
    byte[] msg = Encoding.Unicode.GetBytes(message);
    NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
  }
}