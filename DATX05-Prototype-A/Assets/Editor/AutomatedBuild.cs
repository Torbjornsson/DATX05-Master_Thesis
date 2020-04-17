using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AutomatedBuild
{
    private static string filePath = "C:/Users/thommy/Documents/MasterThesis-Presence_in_VR/Builds/";
    private static string serverFilePath = "E:/Drive/ID2 - DATX05 - Master's thesis in Computer science and engineering/Build/";

    public static void BuildOnServer()
    {
        Console.WriteLine("Starting BuildOnServer");
        List<string> enabledScenes = new List<string>();
        List<string> names = new List<string>()
        {"Tutorial1AO", "Tutorial1A",
        "Tutorial1BO", "Tutorial1B",
        "Tutorial1CO", "Tutorial1C",
        "Tutorial2AO", "Tutorial2A",
        "Tutorial2BO", "Tutorial2B",
        "Tutorial2CO", "Tutorial2C",
        "Tutorial3AO", "Tutorial3A",
        "Tutorial3BO", "Tutorial3B",
        "Tutorial3CO", "Tutorial3C"};

        for (int i = 0; i < names.Count; i++)
        {
            enabledScenes.Clear();
            var buildSettingScene = EditorBuildSettings.scenes[i];

            if (buildSettingScene.enabled)
            {
                enabledScenes.Add(buildSettingScene.path);
            }

            //Get the name of the tutorial
            string executableBuildPath = serverFilePath + names[i].Substring(0,9);

            Console.WriteLine("Building " + names[i]);

            if (!Directory.Exists(executableBuildPath))
            {
                Directory.CreateDirectory(executableBuildPath);
            }

            string executableName = names[i];
            string locationPathName = executableBuildPath + "/" + executableName + ".exe";

            WindowsBuild(enabledScenes, locationPathName);
        }
 
    }
    public static void BuildTutorial1()
    {
        List<string> enabledScenes = new List<string>();
        List<string> names = new List<string>()
        {"T1AO", "T1A",
        "T1BO", "T1B",
        "T1CO", "T1C",
        "T2AO", "T2A",
        "T2BO", "T2B",
        "T2CO", "T2C",
        "T3AO", "T3A",
        "T3BO", "T3B",
        "T3CO", "T3C"};

        for (int i = 0; i < names.Count; i++)
        {
            enabledScenes.Clear();
            var buildSettingScene = EditorBuildSettings.scenes[i];

            if (buildSettingScene.enabled)
            {
                enabledScenes.Add(buildSettingScene.path);
            }

            //Get the name of the tutorial
            string executableBuildPath = filePath + names[i].Substring(0,2);
            Debug.Log(executableBuildPath);

            if (!Directory.Exists(executableBuildPath))
            {
                Directory.CreateDirectory(executableBuildPath);
            }

            string executableName = names[i];
            string locationPathName = executableBuildPath + "/" + executableName + ".apk";

            // WindowsBuild(enabledScenes, locationPathName);
            AndroidBuild(enabledScenes, locationPathName);
        }
    }

    private static void AndroidBuild(List<string> enabledScenes, string locationPathName)
    {
        Debug.Log("Starting Android Build");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = enabledScenes.ToArray();
        buildPlayerOptions.locationPathName = locationPathName;
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        BuildPipeline.BuildPlayer(buildPlayerOptions);
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

