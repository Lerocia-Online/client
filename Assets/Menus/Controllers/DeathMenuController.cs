namespace Menus.Controllers {
  using Lerocia.Characters;
  using UnityEngine;
  using UnityEngine.UI;
  using Networking;

  public class DeathMenuController : MonoBehaviour {
    void Start() {
      transform.Find("RespawnButton").GetComponent<Button>().onClick.AddListener(Respawn);
    }

    private static void Respawn() {
      CanvasSettings.DeactivateMenu();
      Destroy(GameObject.Find("DeathCamera"));
      ConnectedCharacters.MyPlayer.Respawn();
      NetworkSend.Reliable("RESPAWN|");
    }
  }
}