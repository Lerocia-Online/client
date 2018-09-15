using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class CustomNetworkManager : NetworkManager {
    
    // detect headless mode (which has graphicsDeviceType Null)
    bool IsHeadless() {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }

    private void Awake() {
        networkAddress = "34.207.71.203";
        networkPort = 7777;
        if (IsHeadless()) {
            Debug.Log("starting server");
            StartServer();
        } else {
            Debug.Log("connecting to server");
            StartClient();
        }
    }
}
