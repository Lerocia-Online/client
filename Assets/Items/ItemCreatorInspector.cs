namespace Items {
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ItemCreator))]
    public class ItemCreatorInspector : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            ItemCreator itemCreator = (ItemCreator) target;
            if (GUILayout.Button("Create Item")) {
                itemCreator.Create();
            }
            GUILayout.Label(itemCreator.ErrorText);
        }
    }
}