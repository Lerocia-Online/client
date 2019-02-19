namespace Characters.Players.Controllers {
  using UnityEngine;
  using Menus;
  using Networking;
  using Animation;
  using NPCs;

  public class PlayerController : MonoBehaviour {
    private CharacterAnimator _characterAnimator;
    private const float Range = 10f;

    private void Start() {
      _characterAnimator = gameObject.GetComponent<CharacterAnimator>();
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.F)) {
        CanvasSettings.ToggleInventoryMenu();
      }

      if (Input.GetKeyDown(KeyCode.Escape)) {
        CanvasSettings.TogglePauseMenu();
      }

      if (!_characterAnimator.Attacking) {
        if (Input.GetButtonDown("Fire1")) {
          NetworkSend.Reliable("ATK|");
          _characterAnimator.Attacking = true;
          _characterAnimator.Attack();
          Attack();
        }
      }
    }

    private void Attack() {
      RaycastHit hit;
      if (Physics.Raycast(gameObject.transform.position, transform.forward, out hit, Range)) {
        if (hit.transform.CompareTag("Player")) {
          int connectionId = hit.transform.gameObject.GetComponent<PlayerReference>().ConnectionId;
          CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.Players[connectionId]);
          NetworkSend.Reliable("HIT|" + connectionId + "|" + ConnectedCharacters.MyPlayer.Damage);
        }

        if (hit.transform.CompareTag("NPC")) {
          int npcId = hit.transform.gameObject.GetComponent<NPCReference>().NPCId;
          CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.NPCs[npcId]);
          NetworkSend.Reliable("HITNPC|" + npcId + "|" + ConnectedCharacters.MyPlayer.Damage);
        }
      }
    }
  }
}