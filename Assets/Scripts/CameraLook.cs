using UnityEngine;
using System.Collections;

public class CameraLook : MonoBehaviour {
  public float sensitivityY = 15F;
  public float minimumY = -60F;
  public float maximumY = 60F;
  float rotationX = 0F;
  float rotationY = 0F;
  Quaternion originalRotation;

  private void Start() {
    transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y + 0.5f,
      transform.parent.position.z);
    originalRotation = transform.localRotation;
  }

  void Update() {
    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
    rotationY = ClampAngle(rotationY, minimumY, maximumY);
    Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
    transform.localRotation = originalRotation * yQuaternion;
  }

  public static float ClampAngle(float angle, float min, float max) {
    if (angle < -360F)
      angle += 360F;
    if (angle > 360F)
      angle -= 360F;
    return Mathf.Clamp(angle, min, max);
  }
}