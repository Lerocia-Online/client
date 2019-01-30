using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTagController : MonoBehaviour {
  private Client client;
  void Start() {
    client = GameObject.Find("Client").GetComponent<Client>();
  }
  
  void Update() {
    // Rotate the camera every frame so it keeps looking at the target
    transform.LookAt(client.players[client.ourClientId].avatar.transform);
  }
}