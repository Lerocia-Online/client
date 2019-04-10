namespace Characters.NPCs {
  using UnityEditor;
  using UnityEngine;

  [CustomEditor(typeof(NPCCreator))]
  public class NPCCreatorInspector : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      NPCCreator npcCreator = (NPCCreator) target;
      if (GUILayout.Button("Create NPC")) {
        npcCreator.Create();
      }
      GUILayout.Label(npcCreator.ErrorText);
    }
  }
}