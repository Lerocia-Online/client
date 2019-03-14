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

    public void HitPlayer(GameObject hit) {
      CanHit = false;
      int characterId = hit.GetComponent<CharacterReference>().CharacterId;
      CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.Players[characterId]);
      NetworkSend.Reliable("HIT|" + characterId + "|" + ConnectedCharacters.MyPlayer.Damage);
      ConnectedCharacters.Players[characterId].TakeDamage(ConnectedCharacters.MyPlayer.Damage);
    }

    public void HitNPC(GameObject hit) {
      CanHit = false;
      int npcId = hit.GetComponent<CharacterReference>().CharacterId;
      CanvasSettings.PlayerHudController.ActivateEnemyView(ConnectedCharacters.NPCs[npcId]);
      NetworkSend.Reliable("HITNPC|" + npcId + "|" + ConnectedCharacters.MyPlayer.Damage);
      ConnectedCharacters.NPCs[npcId].TakeDamage(ConnectedCharacters.MyPlayer.Damage);
    }
  }
}