namespace Characters.NPCs {
  using UnityEngine;
  using Animation;
  using Characters.Controllers;
  using Controllers;

  public class NPCFactory : MonoBehaviour {
    public GameObject NPCPrefab;

    public void Spawn(string[] data) {
      // Spawn all NPCs
      for (int i = 1; i < data.Length; i++) {
        string[] d = data[i].Split('%');
        Spawn(int.Parse(d[0]), d[1], float.Parse(d[2]), float.Parse(d[3]), float.Parse(d[4]), float.Parse(d[5]),
          float.Parse(d[6]), float.Parse(d[7]));
      }
    }

    public void Spawn(int npcId, string npcName, float px, float py, float pz, float rx, float ry, float rz) {
      GameObject npcObject = Instantiate(NPCPrefab);
      npcObject.name = npcName;
      npcObject.transform.position = new Vector3(px, py, pz);
      npcObject.transform.rotation = Quaternion.Euler(new Vector3(rx, ry, rz));
      npcObject.AddComponent<NPCController>();
      npcObject.AddComponent<CharacterLerpController>();
      npcObject.AddComponent<NPCReference>();
      npcObject.GetComponent<NPCReference>().NPCId = npcId;
      npcObject.AddComponent<CharacterAnimator>();
      NPC npc = new NPC(npcName, npcObject, 100, 100, 5, 0, DialogueList.Dialogues[npcName]);
      ConnectedCharacters.NPCs.Add(npcId, npc);
      ConnectedCharacters.NPCs[npcId].Avatar.GetComponent<CharacterLerpController>().Character = npc;
    }
  }
}