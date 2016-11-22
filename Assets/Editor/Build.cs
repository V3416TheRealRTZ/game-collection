using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


[assembly: System.Reflection.AssemblyVersion("1.0.*")]
public class Build
{

    [MenuItem("MyTools/Win32 Build")]
    private static void BuildWin32()
    {
        string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string path = "\\Builds\\Build ver " + version;
        var files = Directory.GetFiles("\\Assets\\Scenes");
        var levels = files
                     .Where(fileName => !fileName.Contains(".meta"))
                     .Select(fileName => fileName)
                     .ToArray();
        BuildPipeline.BuildPlayer(levels, path + "\\BuiltGame ver " + version + ".exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
        Debug.Log(version);
    }

    
}
