namespace Characters.Players.Controllers {
  using UnityEngine;
  using Menus;
  using Networking;
  using Lerocia.Characters;
  using Lerocia.Items;

  public class PlayerCameraController : MonoBehaviour {
    private RaycastHit _hit;
    private const float Range = 3f;
    private int _lastItemHit = -1;
    private int _lastNPCHit = -1;
    private int _lastPlayerHit = -1;
    private int _lastBodyHit = -1;

    private void OnEnable() {
      _lastItemHit = -1;
      _lastNPCHit = -1;
      _lastPlayerHit = -1;
      _lastBodyHit = -1;
    }

    private void Update() {
      if (Physics.Raycast(gameObject.transform.position, transform.forward, out _hit, Range)) {
        if (_hit.transform.CompareTag("Item")) {
          if (_hit.transform.gameObject.GetComponent<ItemReference>().ItemId != _lastItemHit) {
            _lastItemHit = _hit.transform.gameObject.GetComponent<ItemReference>().ItemId;
            CanvasSettings.PlayerHudController.ActivateInteractableView();
            CanvasSettings.PlayerHudController.SetItemView(ItemList.Items[_lastItemHit]);
          }

          if (Input.GetKeyDown(KeyCode.E)) {
            NetworkSend.Reliable("PICKUP|" + _hit.transform.gameObject.GetComponent<ItemReference>().WorldId);
          }
        }

        if (_hit.transform.CompareTag("NPC")) {
          if (_hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId != _lastNPCHit) {
            _lastNPCHit = _hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId;
            CanvasSettings.PlayerHudController.ActivateInteractableView();
            CanvasSettings.PlayerHudController.SetNPCView(ConnectedCharacters.NPCs[_lastNPCHit]);
          }

          if (ConnectedCharacters.NPCs[_lastNPCHit].DialogueId >= 0 && Input.GetKeyDown(KeyCode.E)) {
//            NetworkSend.Reliable("NPCITEMS|" + _lastNPCHit);
            CanvasSettings.PlayerHudController.SetCurrentInteractingCharacter(ConnectedCharacters.NPCs[_lastNPCHit]);
            CanvasSettings.PlayerHudController.Interact("INTERACT");
          }
        }

        if (_hit.transform.CompareTag("Player")) {
          if (_hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId != _lastPlayerHit) {
            _lastPlayerHit = _hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId;
            CanvasSettings.PlayerHudController.ActivateInteractableView();
            CanvasSettings.PlayerHudController.SetPlayerView(ConnectedCharacters.Players[_lastPlayerHit]);
          }
          
          if (ConnectedCharacters.Players[_lastPlayerHit].DialogueId >= 0 && Input.GetKeyDown(KeyCode.E)) {
            CanvasSettings.PlayerHudController.SetCurrentInteractingCharacter(ConnectedCharacters.Players[_lastPlayerHit]);
            CanvasSettings.PlayerHudController.Interact("INTERACT");
          }
        }

        if (_hit.transform.CompareTag("Body")) {
          if (_hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId != _lastBodyHit) {
            _lastBodyHit = _hit.transform.gameObject.GetComponent<CharacterReference>().CharacterId;
            CanvasSettings.PlayerHudController.ActivateInteractableView();
            CanvasSettings.PlayerHudController.SetBodyView(ConnectedCharacters.Bodies[_lastBodyHit]);
          }
          
          if (ConnectedCharacters.Bodies[_lastBodyHit].DialogueId >= 0 && Input.GetKeyDown(KeyCode.E)) {
            CanvasSettings.PlayerHudController.SetCurrentInteractingCharacter(ConnectedCharacters.Bodies[_lastBodyHit]);
            CanvasSettings.PlayerHudController.Interact("INTERACT");
          }
        }
      } else {
        _lastItemHit = -1;
        _lastNPCHit = -1;
        _lastPlayerHit = -1;
        _lastBodyHit = -1;
        CanvasSettings.PlayerHudController.DeactivateInteractableView();
      }
    }
  }
}