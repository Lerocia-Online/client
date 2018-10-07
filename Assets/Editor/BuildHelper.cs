using UnityEditor;
using UnityEngine;

class BuildHelper {
    static void build() {
        string[] scenes = {"Assets/Scenes/Main.unity"};
        string pathToDeploy = "Builds/ServerBuild";
        Debug.Log("**********PRE-BUILD**********");
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinuxUniversal, BuildOptions.EnableHeadlessMode);
        Debug.Log("**********POST-BUILD**********");
    }
}