namespace Characters.Players {
  using UnityEngine;
  using Menus;

  public class Player : Character {
    public Player(string name, GameObject avatar, int maxHealth, int maxStamina, int baseDamage, int baseArmor) : base(
      name, avatar, maxHealth, maxStamina, baseDamage, baseArmor) { }

    protected override void Kill() {
      // Reset players health
      CurrentHealth = MaxHealth;
      // Move them back to "spawn" point
      Avatar.transform.position = new Vector3(0, 1, 0);
      CanvasSettings.PlayerHudController.DeactivateEnemyView();
    }
  }
}