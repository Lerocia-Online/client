namespace Characters.Players.Controllers {
  using UnityEngine;
  using Menus;
  using Networking;
  using Animation;
  using NPCs;
  using Lerocia.Characters;

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
          int characterId = hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId;
          CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.Players[characterId]);
          NetworkSend.Reliable("HIT|" + characterId + "|" + ConnectedCharacters.MyPlayer.Damage);
          ConnectedCharacters.Players[characterId].TakeDamage(ConnectedCharacters.MyPlayer.Damage);
        }

        if (hit.transform.CompareTag("NPC")) {
          int npcId = hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId;
          CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.NPCs[npcId]);
          NetworkSend.Reliable("HITNPC|" + npcId + "|" + ConnectedCharacters.MyPlayer.Damage);
          ConnectedCharacters.NPCs[npcId].TakeDamage(ConnectedCharacters.MyPlayer.Damage);
        }
      }
    }
  }
}