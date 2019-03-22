namespace Menus.Controllers {
  using UnityEngine;
  using UnityEngine.UI;

  public class PauseMenuController : MonoBehaviour {
    void Start() {
      transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(Quit);
    }

    private static void Quit() {
      Application.Quit();
    }
  }
}