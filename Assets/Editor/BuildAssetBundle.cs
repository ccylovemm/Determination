using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class BuildAssetBundle
{
    [MenuItem("BuildAsset/BuildForWindows")]
    static void BuildForWindows()
	{
        BuildAssets(BuildTarget.StandaloneWindows);
	}

    [MenuItem("BuildAsset/BuildForAndriod")]
    static void BuildForAndriod()
    {
        BuildAssets(BuildTarget.Android);
    }

    [MenuItem("BuildAsset/BuildForIOS")]
    static void BuildForIOS()
    {
        BuildAssets(BuildTarget.iOS);
    }

    static void BuildAssets(BuildTarget buildTarget)
    {
        string outPath = Application.streamingAssetsPath + "/AssetBundle";
        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, buildTarget);
        AssetDatabase.Refresh();
    }
}
