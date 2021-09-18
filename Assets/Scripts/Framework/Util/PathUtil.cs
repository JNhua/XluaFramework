using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    // RootDirectory
    public static readonly string AssetsPath = Application.dataPath;

    // The bundle directory
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/"; //take care of the last '/'

    // The output directory
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    // The readonly directory
    public static readonly string ReadonlyPath = Application.streamingAssetsPath;

    // The read and write directory
    public static readonly string ReadWritePath = Application.persistentDataPath;

    // The Lua scripts directory
    public static readonly string LuaPath = "Assets/BuildResources/Lua/";

    // The bundle resource directory
    public static string BundleResourcePath
    {
        get {
            if (AppConst.GameMode == GameMode.UpdateMode)
            {
                return ReadWritePath;
            }    
            return ReadonlyPath; 
        }
    }

    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// Replace the "\\" of the path to "/"
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Trim().Replace("\\", "/");
    }

    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/Lua/{0}.bytes", name);
    }

    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", name);
    }
    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }
    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    }
    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    }
    public static string GetModelPath(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefabs/{0}.prefab", name);
    }
    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scenes/Scenes/{0}.unity", name);
    }
    public static string GetSpritePath(string name)
    {
        return string.Format("Assets/BuildResources/Sprites/{0}", name);
    }
}
