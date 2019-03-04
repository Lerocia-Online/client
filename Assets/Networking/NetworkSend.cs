using UnityEngine;

namespace Networking {
  using System.Text;
  using UnityEngine.Networking;
  
  public static class NetworkSend {
    public static void Reliable(string message) {
      Send(message, NetworkSettings.ReliableChannel);
    }

    public static void Unreliable(string message) {
      Send(message, NetworkSettings.UnreliableChannel);
    }

    private static void Send(string message, int channelId) {
      byte[] msg = Encoding.Unicode.GetBytes(message);
//      Debug.Log("Sending: " + message);
      NetworkTransport.Send(NetworkSettings.HostId, NetworkSettings.ConnectionId, channelId, msg, message.Length * sizeof(char), out NetworkSettings.Error);
    }
  }
}