namespace Characters.Players.Controllers {
  using UnityEngine;
  using Menus;
  using Networking;
  using Animation;
  using Lerocia.Characters;

  public class PlayerController : MonoBehaviour {
    private CharacterAnimator _characterAnimator;
    public bool CanHit;

    private void Start() {
      _characterAnimator = gameObject.GetComponent<CharacterAnimator>();
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.F)) {
        CanvasSettings.ToggleInventoryMenu(ConnectedCharacters.MyPlayer, "INVENTORY");
      }

      if (Input.GetKeyDown(KeyCode.Escape)) {
        CanvasSettings.TogglePauseMenu();
      }

      if (Input.GetButtonDown("Fire1") && !_characterAnimator.Attacking) {
        NetworkSend.Reliable("ATK|");
        _characterAnimator.Attack();
      }

      if (!_characterAnimator.Attacking) {
        CanHit = true;
      }
    }

    public void Hit(GameObject hit) {
      CanHit = false;
      int characterId = hit.GetComponent<CharacterReference>().CharacterId;
      CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.Characters[characterId]);
      NetworkSend.Reliable("HIT|" + characterId + "|" + ConnectedCharacters.MyPlayer.Damage);
      ConnectedCharacters.Characters[characterId].TakeDamage(ConnectedCharacters.MyPlayer.Damage);
    }
  }
}