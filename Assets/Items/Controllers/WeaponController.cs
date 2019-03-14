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
        if (col.transform.CompareTag("Player")) {
          Debug.Log("Hit player");
          PlayerController.HitPlayer(col.gameObject);
        }

        if (col.transform.CompareTag("NPC")) {
          Debug.Log("Hit NPC");
          PlayerController.HitNPC(col.gameObject);
        }
      }
    }
  }
}