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
        BuildPipeline.BuildPlayer(levels, path + "\\BuiltGame ver " + version + ".exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
        using (var reader = new StreamReader(projDir + "\\.git\\FETCH_HEAD"))
        using (var writer = new StreamWriter(path + "\\build info.txt"))
        {
            string str = reader.ReadLine();
            str = str.Split(new char[] { '\t', ' ' }).First();
            Debug.Log(str);
            writer.WriteLine("build version: " + version);
            writer.WriteLine("commit hash: " + str);
        }
        Debug.Log(version);
    }
}
