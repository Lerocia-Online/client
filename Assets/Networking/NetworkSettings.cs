namespace Networking {
  using UnityEngine.Networking;
  using Constants;

  public static class NetworkSettings {
    public static int HostId;
    public static int ReliableChannel;
    public static int UnreliableChannel;
    public static int ConnectionId;
    public static byte Error;
    public static bool IsLoggingIn;
    public static bool IsRegistering;
    public static bool IsConnected;
    public static bool IsStarted;
    
    public static void InitializeNetworkTransport() {
      NetworkTransport.Init();
      ConnectionConfig cc = new ConnectionConfig();

      ReliableChannel = cc.AddChannel(QosType.Reliable);
      UnreliableChannel = cc.AddChannel(QosType.Unreliable);

      HostTopology topo = new HostTopology(cc, NetworkConstants.MaxConnection);

      HostId = NetworkTransport.AddHost(topo, 0);
      ConnectionId = NetworkTransport.Connect(HostId, NetworkConstants.Address, NetworkConstants.Port, 0, out Error);

      IsConnected = true;
    }
  }
}