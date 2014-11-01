using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 命令行的资源发布
/// </summary>
public class CommandBuild 
{
    
    //发布安卓
    [MenuItem("Publish/发布安卓开发版")]
    public static void PublishAndroidDevelopment()
    {
        PublishAndroid_impl(true);
    }
    //发布安卓
    [MenuItem("Publish/发布安卓")]
    public static void PublishAndroid()
    {

       PublishAndroid_impl(false);
    }
    public static void PublishAndroid_impl(bool develepment)
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene edscene in EditorBuildSettings.scenes)
        {
            if (!scenes.Contains(edscene.path))
                scenes.Add(edscene.path);
        }

        BuildOptions options = BuildOptions.AcceptExternalModificationsToPlayer;
        if (develepment)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.allowDebugging = true;
            EditorUserBuildSettings.connectProfiler = true;
            options |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }
        string path = Application.dataPath + "/../../Platform/Android/";
        if (File.Exists(path))
            File.Delete(path);


        BuildPipeline.BuildPlayer(scenes.ToArray(), path, BuildTarget.Android, options);
    }

    /// <summary>
    /// 发布ios
    /// </summary>
    public static void PublishIPhone()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene edscene in EditorBuildSettings.scenes)
        {
            if (!scenes.Contains(edscene.path))
                scenes.Add(edscene.path);
        }
        string path = Application.dataPath + "/../../../Bin/Client/Client.apk";
        if (File.Exists(path))
            File.Delete(path);
        BuildPipeline.BuildPlayer(scenes.ToArray(), path, BuildTarget.iPhone, BuildOptions.None);

    }

}
