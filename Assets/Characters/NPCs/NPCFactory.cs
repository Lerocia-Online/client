namespace Characters.NPCs {
  using UnityEngine;
  using Controllers;
  using Lerocia.Characters;
  using Networking;

  public class NPCFactory : MonoBehaviour {
    public GameObject NPCPrefab;

    public void Spawn(string[] data) {
      // Spawn all NPCs
      for (int i = 1; i < data.Length; i++) {
        string[] d = data[i].Split('%');
        Spawn(
          int.Parse(d[0]), 
          d[1], 
          d[2],
          float.Parse(d[3]), float.Parse(d[4]), float.Parse(d[5]), 
          float.Parse(d[6]), float.Parse(d[7]), float.Parse(d[8]), 
          int.Parse(d[9]),  
          int.Parse(d[10]), 
          int.Parse(d[11]), 
          int.Parse(d[12]), 
          int.Parse(d[13]), 
          int.Parse(d[14]), 
          int.Parse(d[15]), 
          int.Parse(d[16]), 
          int.Parse(d[17]),
          int.Parse(d[18]),
          int.Parse(d[19]),
          float.Parse(d[20]), float.Parse(d[21]), float.Parse(d[22]),
          bool.Parse(d[23]),
          float.Parse(d[24]),
          float.Parse(d[25])
        );
      }
    }

    public void Spawn(
      int characterId, 
      string characterName, 
      string characterPersonality,
      float px, float py, float pz, 
      float rx, float ry, float rz,  
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
      int dialogueId,
      float ox, float oy, float oz,
      bool isDead,
      float respawnTime,
      float lookRadius
    ) {
      GameObject npcObject = Instantiate(NPCPrefab);
      npcObject.name = characterName;
      npcObject.transform.position = new Vector3(px, py, pz);
      npcObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
      npcObject.GetComponent<CharacterReference>().CharacterId = characterId;
      ClientNPC npc = new ClientNPC(
        characterId, 
        characterName, 
        characterPersonality, 
        npcObject, 
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
        dialogueId,
        new Vector3(ox, oy, oz), 
        respawnTime,
        lookRadius
      );
      if (isDead) {
        npc.Death();
      }
      ConnectedCharacters.Characters.Add(characterId, npc);
      ConnectedCharacters.NPCs.Add(characterId, npc);
      NetworkSend.Reliable("NPCITEMS|" + characterId);
      ConnectedCharacters.NPCs[characterId].Avatar.GetComponent<CharacterLerpController>().Character = npc;
    }
  }
}