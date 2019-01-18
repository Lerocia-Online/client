using UnityEngine;
using System.Collections;

public class PlayerLook : MonoBehaviour {
  public float sensitivityX = 15F;
  public float minimumX = -360F;
  public float maximumX = 360F;
  float rotationX = 0F;
  float rotationY = 0F;
  Quaternion originalRotation;

  void Update() {
    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
    rotationX = ClampAngle(rotationX, minimumX, maximumX);
    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
    transform.localRotation = originalRotation * xQuaternion;
  }

  void Start() {
    originalRotation = transform.localRotation;
  }

  public static float ClampAngle(float angle, float min, float max) {
    if (angle < -360F)
      angle += 360F;
    if (angle > 360F)
      angle -= 360F;
    return Mathf.Clamp(angle, min, max);
  }
}