using Menus;

namespace Characters.Bodies {
  using UnityEngine;
  using Animation;
  using Lerocia.Characters;

  public class BodyFactory : MonoBehaviour {
    public GameObject BodyPrefab;
    public GameObject DeathCameraPrefab;

    public void Spawn(string[] data) {
      Character character = ConnectedCharacters.Characters[int.Parse(data[1])];
      Spawn(
        int.Parse(data[2]),
        character.CharacterName + " (Dead)",
        character.CharacterPersonality,
        character.Avatar.transform.position.x,
        character.Avatar.transform.position.y,
        character.Avatar.transform.position.z,
        character.Avatar.transform.eulerAngles.x,
        character.Avatar.transform.eulerAngles.y,
        character.Avatar.transform.eulerAngles.z,
        character.MaxHealth,
        character.CurrentHealth,
        character.MaxStamina,
        character.CurrentStamina,
        character.Gold,
        character.BaseWeight,
        character.BaseDamage,
        character.BaseArmor,
        character.WeaponId,
        character.ApparelId,
        0
      );
      for (int j = 3; j < data.Length; j++) {
        ConnectedCharacters.Characters[int.Parse(data[2])].Inventory.Add(int.Parse(data[j]));
      }
      if (character.CharacterId == ConnectedCharacters.MyPlayer.CharacterId) {
        GameObject deathCamera = Instantiate(DeathCameraPrefab);
        deathCamera.name = "DeathCamera";
        deathCamera.transform.position = character.Avatar.transform.position;
        deathCamera.transform.rotation = character.Avatar.transform.rotation;
        deathCamera.transform.position += new Vector3(0, 2.5f, 2.5f);
        deathCamera.transform.Rotate(20, 0, 0);
        CanvasSettings.ActivateDeathMenu();
      }
      character.Death();
    }
    
    public void SpawnAll(string[] data) {
      // Spawn all Bodies
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
          int.Parse(d[19])
        );
        for (int j = 20; j < d.Length; j++) {
          ConnectedCharacters.Characters[int.Parse(d[0])].Inventory.Add(int.Parse(d[j]));
        }
      }
    }
    
    private void Spawn(
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
      int dialogueId
    ) {
      GameObject avatar = Instantiate(BodyPrefab);
      avatar.name = characterName;
      avatar.tag = "Body";
      avatar.transform.position = new Vector3(px, py, pz);
      avatar.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
      ClientBody body = new ClientBody(
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
      );
      body.Avatar.GetComponent<CharacterReference>().CharacterId = characterId;
      body.Avatar.GetComponent<CharacterAnimator>().Die();
      ConnectedCharacters.Characters.Add(characterId, body);
      ConnectedCharacters.Bodies.Add(characterId, body);
    }
  }
}