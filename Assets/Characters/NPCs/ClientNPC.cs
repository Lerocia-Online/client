namespace Characters.NPCs {
  using UnityEngine;
  using Lerocia.Characters.NPCs;
  using Menus;
  using Networking;

  public class ClientNPC : NPC {
    public ClientNPC(string name, GameObject avatar, string type, int maxHealth, int currentHealth, int maxStamina,
      int currentStamina, int gold, int baseDamage, int baseArmor, int weapon, int apparel,
      int dialogueId) : base(name, avatar, type, maxHealth, currentHealth, maxStamina,
      currentStamina, gold, baseDamage, baseArmor, weapon, apparel, dialogueId) { }

    public override string[] Interact(string prompt) {
      Dialogue dialogue;
      if (_dialogues.TryGetValue(prompt, out dialogue)) {
        // "Say" response
        CanvasSettings.PlayerHudController.ActivateCaptionView(dialogue.response);
        // Return options
        return dialogue.options;
      }

      return null;
    }

    protected override void Kill() {
      //TODO Handle ClientNPC death
      IsDead = true;
      _dialogues = DialogueList.Dialogues[0];
      NetworkSend.Reliable("NPCITEMS|" + Avatar.GetComponent<NPCReference>().NPCId);
    }

    public override void StartMerchant() {
      //TODO Handle ClientNPC Start Merchant
    }

    public override void LootBody() {
      //TODO Handle ClientNPC Loot Body
    }
  }
}