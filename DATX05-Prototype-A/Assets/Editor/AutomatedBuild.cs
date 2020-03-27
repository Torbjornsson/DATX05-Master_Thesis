using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AutomatedBuild
{
    private static string filePath = "D:/Builds/";

    public static void BuildTutorial1()
    {
        List<string> enabledScenes = new List<string>();
        List<string> names = new List<string>()
        {"Tutorial1AO", "Tutorial1A",
        "Tutorial1BO", "Tutorial1B",
        "Tutorial1CO", "Tutorial1C"};

        for (int i = 0; i < names.Count; i++)
        {
            enabledScenes.Clear();
            var buildSettingScene = EditorBuildSettings.scenes[i];

            if (buildSettingScene.enabled)
            {
                enabledScenes.Add(buildSettingScene.path);
            }

            //Get the name of the tutorial
            string executableBuildPath = filePath + names[i].Substring(0,9);
            Debug.Log(executableBuildPath);

            if (!Directory.Exists(executableBuildPath))
            {
                Directory.CreateDirectory(executableBuildPath);
            }

            string executableName = names[i];
            string locationPathName = executableBuildPath + "/" + executableName + ".exe";

            WindowsBuild(enabledScenes, locationPathName);
        }
    }

    private static void WindowsBuild(List<string> enabledScenes, string locationPathName)
    {
        Debug.Log("Starting Windows Build");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = enabledScenes.ToArray();
        buildPlayerOptions.locationPathName = locationPathName;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(buildPlayerOptions);

    }

    public static void HelloWorld()
    {
        Debug.Log("Hello World");
    }
}

