using UnityEditor;

class BuildHelper {
    static void build() {
        string[] scenes = {"Assets/Scenes/Main.unity"};
        string pathToDeploy = "Builds/ServerBuild";
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinuxUniversal, BuildOptions.EnableHeadlessMode);
    }
    static void buildWindowsClient() {
        string[] scenes = {"Assets/Scenes/Main.unity"};
        string pathToDeploy = "Builds/WindowsClientBuild";
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneWindows, BuildOptions.EnableHeadlessMode);
    }
    static void buildMacClient() {
        string[] scenes = {"Assets/Scenes/Main.unity"};
        string pathToDeploy = "Builds/MacClientBuild";
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneOSX, BuildOptions.EnableHeadlessMode);
    }
}