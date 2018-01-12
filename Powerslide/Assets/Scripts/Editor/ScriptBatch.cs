using UnityEditor;
using System.Diagnostics;

public class ScriptBatch
{
    [MenuItem("MyTools/Windows Build With Postprocess")]

    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        string[] levels = new string[] { "Assets/Scenes/Song Select (PC).unity", "Assets/Scenes/Playground (PC).unity", "Assets/Scenes/Results Screen (PC).unity" };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/Powerslide-win-0.3.0.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Resources/Beatmaps", path + "/Powerslide-win-0.3.0_Data/Beatmaps");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Powerslide-win-0.3.0.exe";
        proc.Start();
    }
}
