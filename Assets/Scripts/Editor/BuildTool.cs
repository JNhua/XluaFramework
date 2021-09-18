using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("Tools/BuildBundleForWindows")]
    static void BundleBuildWindows()
    {
        Build(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/BuildBundleForAndroid")]
    static void BundleBuildAndroid()
    {
        Build(BuildTarget.Android);
    }
    [MenuItem("Tools/BuildBundleForiOS")]
    static void BundleBuildiOS()
    {
        Build(BuildTarget.iOS);
    }
    static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        List<string> bundleInfos = new List<string>();

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);

        for(int i = 0; i < files.Length; ++i)
        {
            if (files[i].EndsWith(".meta"))
                continue;
            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.Log("files:" + fileName);
            string assetName = PathUtil.GetUnityPath(fileName);
            
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            assetBundle.assetNames = new string[] { assetName };
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";
            assetBundleBuilds.Add(assetBundle);

            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + bundleName+".ab";

            if(dependenceInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);
            }

            bundleInfos.Add(bundleInfo);
        }

        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Get the dependence of the file
    /// </summary>
    /// <param name="curFile"></param>
    /// <returns></returns>
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        dependence = files.Where(File => !File.EndsWith(".cs") && !File.Equals(curFile)).ToList();
        return dependence;
    }
}
