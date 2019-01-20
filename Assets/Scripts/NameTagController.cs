using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTagController : MonoBehaviour {
  void Update() {
    // Rotate the camera every frame so it keeps looking at the target
    transform.LookAt(GameObject.Find("PlayerCamera(Clone)").transform);
  }
}