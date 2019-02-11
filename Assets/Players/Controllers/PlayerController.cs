namespace Players.Controllers {
  using UnityEngine;
  using Animation;
  using Menus;
  using Networking;

  public class PlayerController : MonoBehaviour {
    private PlayerAnimator _playerAnimator;
    private float _chargeStartTime;
    private float _chargeEndTime;
    private const float Range = 10f;

    private void Start() {
      _playerAnimator = gameObject.GetComponent<PlayerAnimator>();
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.F)) {
        CanvasSettings.ToggleInventoryMenu();
      }

      if (Input.GetKeyDown(KeyCode.Escape)) {
        CanvasSettings.TogglePauseMenu();
      }

      if (!_playerAnimator.Attacking) {
        if (Input.GetButtonDown("Fire1")) {
          NetworkSend.Reliable("CHARGE|");
          _chargeStartTime = Time.time;
          _playerAnimator.Charge();
        }
      }

      if (_playerAnimator.Charging) {
        if (Input.GetButtonUp("Fire1")) {
          NetworkSend.Reliable("ATK|");
          _chargeEndTime = Time.time;
          _playerAnimator.Attack();
          Attack(_chargeEndTime - _chargeStartTime);
        }
      }
    }

    private void Attack(float chargeTime) {
      int damageBoost = Mathf.FloorToInt(chargeTime);
      RaycastHit hit;
      if (Physics.Raycast(gameObject.transform.position, transform.forward, out hit, Range)) {
        if (hit.transform.CompareTag("Player")) {
          int connectionId = hit.transform.gameObject.GetComponent<PlayerReference>().ConnectionId;
          CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedClients.Players[connectionId]);
          NetworkSend.Reliable("HIT|" + connectionId + "|" + (ConnectedClients.MyPlayer.Damage + damageBoost));
        }
      }
    }
  }
}