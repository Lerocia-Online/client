namespace Items.Controllers {
  using UnityEngine;
  using Characters.Players.Controllers;

  public class WeaponController : MonoBehaviour {
    public PlayerController PlayerController;

    private void Start() {
      PlayerController = transform.root.GetComponent<PlayerController>();
    }
    
    private void OnTriggerEnter(Collider col) {
      if (PlayerController.CanHit) {
        if (col.transform.CompareTag("Player") || col.transform.CompareTag("NPC")) {
          PlayerController.Hit(col.gameObject);
        }
      }
    }
  }
}