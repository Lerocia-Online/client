using UnityEditor;

namespace Editor
{
    class BuildHelper {
        static void build() {
            string[] scenes = {"Assets/Scenes/Main.unity"};
            string pathToDeploy = "Builds/ServerBuild";
            BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.StandaloneLinuxUniversal, BuildOptions.EnableHeadlessMode);
        }
    }
}