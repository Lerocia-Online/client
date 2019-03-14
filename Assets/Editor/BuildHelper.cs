using UnityEditor;

class BuildHelper {
  static void build() {
    string[] scenes = {"Assets/Scenes/Server.unity"};
    string pathToDeploy = "Builds/ServerBuild";
    BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinuxUniversal,
      BuildOptions.EnableHeadlessMode);
  }

  static void buildWindowsClient() {
    string[] scenes = { "Assets/Scenes/Splash.unity", "Assets/Scenes/Lerocia.unity" };
    string pathToDeploy = "Builds/WindowsClientBuild.exe";
    BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneWindows64, BuildOptions.None);
  }

  static void buildMacClient() {
    string[] scenes = { "Assets/Scenes/Splash.unity", "Assets/Scenes/Lerocia.unity" };
    string pathToDeploy = "Builds/MacClientBuild";
    BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneOSX, BuildOptions.None);
  }
}