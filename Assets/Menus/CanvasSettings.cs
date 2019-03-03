namespace Menus {
  using UnityEngine;
  using UnityStandardAssets.Characters.FirstPerson;
  using Lerocia.Characters;
  using Characters.Players.Controllers;
  using Controllers;

  public static class CanvasSettings {
    public static GameObject LoginMenu;
    public static GameObject PlayerHud;
    public static GameObject PauseMenu;
    public static GameObject InventoryMenu;
    public static InventoryMenuController InventoryMenuController;
    public static PlayerHUDController PlayerHudController;

    public static void InitializeCanvases() {
      LoginMenu = GameObject.Find("LoginMenu");
      LoginMenu.SetActive(true);
      PlayerHud = GameObject.Find("PlayerHUD");
      PlayerHudController = PlayerHud.GetComponent<PlayerHUDController>();
      PlayerHud.SetActive(false);
      PauseMenu = GameObject.Find("PauseMenu");
      PauseMenu.SetActive(false);
      InventoryMenu = GameObject.Find("InventoryMenu");
      InventoryMenuController = InventoryMenu.GetComponent<InventoryMenuController>();
      InventoryMenu.SetActive(false);
    }

    public static void ToggleInventoryMenu(Character character, string interaction) {
      if (!PauseMenu.activeSelf) {
        if (InventoryMenu.activeSelf) {
          DeactivateMenu();
        } else {
          ToggleControl(false);
          PlayerHud.SetActive(false);
          PauseMenu.SetActive(false);
          InventoryMenu.SetActive(true);
          InventoryMenuController.OpenMenu(character, interaction);
        }
      }
    }
    
    public static void TogglePauseMenu() {
      if (PauseMenu.activeSelf) {
        DeactivateMenu();
      } else {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ToggleControl(false);
        PlayerHud.SetActive(false);
        if (InventoryMenu.activeSelf) {
          InventoryMenuController.CloseMenu();
          InventoryMenu.SetActive(false);
        }
        PauseMenu.SetActive(true);
      }
    }

    private static void DeactivateMenu() {
      ToggleControl(true);
      PauseMenu.SetActive(false);
      if (InventoryMenu.activeSelf) {
        InventoryMenuController.CloseMenu();
        InventoryMenu.SetActive(false);
      }
      PlayerHud.SetActive(true);
    }

    public static void ToggleControl(bool state) {
      ConnectedCharacters.MyPlayer.Avatar.GetComponent<FirstPersonController>().enabled = state;
      ConnectedCharacters.MyPlayer.Avatar.transform.Find("FirstPersonCharacter").GetComponent<PlayerCameraController>().enabled = state;
    }
  }
}