namespace Characters.NPCs {
  using System.Collections.Generic;
  using UnityEngine;
  using Animation;
  using Characters.Controllers;
  using Controllers;
  using Lerocia.Characters;
  using Lerocia.Characters.NPCs;

  public class NPCFactory : MonoBehaviour {
    public GameObject NPCPrefab;

    public void Spawn(string[] data) {
      // Spawn all NPCs
      for (int i = 1; i < data.Length; i++) {
        string[] d = data[i].Split('%');
        Spawn(int.Parse(d[0]), d[1], float.Parse(d[2]), float.Parse(d[3]), float.Parse(d[4]), float.Parse(d[5]),
          float.Parse(d[6]), float.Parse(d[7]), d[8], int.Parse(d[9]));
      }
    }

    public void Spawn(int npcId, string npcName, float px, float py, float pz, float rx, float ry, float rz,
      string type, int dialogueId) {
      GameObject npcObject = Instantiate(NPCPrefab);
      npcObject.name = npcName;
      npcObject.transform.position = new Vector3(px, py, pz);
      npcObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
      npcObject.AddComponent<NPCController>();
      if (type == "friendly") {
        npcObject.GetComponent<NPCController>().TargetTypes = new List<string> {"enemy"};
      } else if (type == "enemy") {
        npcObject.GetComponent<NPCController>().TargetTypes = new List<string> {"friendly", "passive"};
      } else if (type == "passive") {
        // Do nothing, passive does not target
      } else {
        Debug.Log("Invalid Type");
      }

      npcObject.AddComponent<CharacterLerpController>();
      npcObject.AddComponent<NPCReference>();
      npcObject.GetComponent<NPCReference>().NPCId = npcId;
      npcObject.AddComponent<CharacterAnimator>();
      ClientNPC npc = new ClientNPC(npcName, npcObject, type, 100, 100, 100, 100, 0, 5, 0, -1, -1, dialogueId);
      ConnectedCharacters.Characters.Add(npc);
      ConnectedCharacters.NPCs.Add(npcId, npc);
      ConnectedCharacters.NPCs[npcId].Avatar.GetComponent<CharacterLerpController>().Character = npc;
    }
  }
}