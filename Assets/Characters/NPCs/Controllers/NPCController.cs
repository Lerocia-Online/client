namespace Characters.NPCs.Controllers {
  using UnityEngine;

  public class NPCController : MonoBehaviour{
    public void Destroy() {
      Destroy(gameObject);
    }
  }
}