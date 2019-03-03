namespace Characters.Players {
  using UnityEngine;
  using Lerocia.Characters.Players;
  using Menus;

  public class ClientPlayer : Player {
    public ClientPlayer(
      int characterId, 
      string characterName, 
      string characterPersonality,
      GameObject avatar, 
      int maxHealth, 
      int currentHealth, 
      int maxStamina,
      int currentStamina,
      int gold, 
      int baseWeight,
      int baseDamage, 
      int baseArmor, 
      int weaponId, 
      int apparelId,
      int dialogueId
    ) : base(
      characterId, 
      characterName, 
      characterPersonality,
      avatar, 
      maxHealth, 
      currentHealth, 
      maxStamina, 
      currentStamina, 
      gold, 
      baseWeight,
      baseDamage, 
      baseArmor, 
      weaponId,
      apparelId,
      dialogueId
    ) { }

    protected override void Kill() {
      //TODO Handle ClientPlayer death
      // Reset players health
      CurrentHealth = MaxHealth;
      // Move them back to "spawn" point
      Avatar.transform.position = new Vector3(0, 1, 0);
      CanvasSettings.PlayerHudController.DeactivateEnemyView();
    }
  }
}