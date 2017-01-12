using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;


[assembly: System.Reflection.AssemblyVersion("1.0.*")]
public class Build
{

    [MenuItem("MyTools/Win32 Build")]
    private static void BuildWin32()
    {
        string projDir = Environment.CurrentDirectory;
        string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string path = projDir + "\\Builds\\Build ver " + version;
        Debug.Log(projDir);
        var files = Directory.GetFiles(projDir + "\\Assets\\Scenes");
        var levels = files
                     .Where(fileName => !fileName.Contains(".meta"))
                     .Select(fileName => fileName)
                     .ToArray();
        PlayerSettings.productName = PlayerSettings.productName + " ver " + version;
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.defaultScreenWidth = 1024;
        BuildPipeline.BuildPlayer(levels, path + "\\BuiltGame ver " + version + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        using (var writer = new StreamWriter(path + "\\build info.txt"))
        {
            writer.WriteLine("build version: " + version);
        }
        using (var writer = new StreamWriter(projDir + "\\last ver.txt"))
        {
            writer.WriteLine(version);
        }
        Debug.Log(version);
    }
}
