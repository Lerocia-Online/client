namespace Characters.NPCs {
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(NPCDestinationCreator))]
    public class NPCDestinationCreatorInspector : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            NPCDestinationCreator npcDestinationCreator = (NPCDestinationCreator) target;
            if (GUILayout.Button("Create Destination")) {
                npcDestinationCreator.Create();
            }
            GUILayout.Label(npcDestinationCreator.ErrorText);
        }
    }
}