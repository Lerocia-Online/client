using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class CustomNetworkManager : NetworkManager {
    // detect headless mode (which has graphicsDeviceType Null)
    bool IsHeadless() {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }

    private void Awake() {
        if (IsHeadless()) {
            Debug.Log("starting server");
            StartServer();
        } else {
            Debug.Log("connecting to server");
            StartClient();
        }
    }
}
