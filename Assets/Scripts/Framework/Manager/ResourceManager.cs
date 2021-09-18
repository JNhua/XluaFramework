using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UObject = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    internal class BundleInfo
    {
        public string AssetsName;
        public string BundleName;
        public List<string> Dependences;
    }

    internal class BundleData
    {
        public AssetBundle bundle;
        public int Count;
        public BundleData(AssetBundle ab)
        {
            bundle = ab;
            Count = 1;
        }
    }
    // BundleInfo 的集合
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    // 已经加载的bundle
    private Dictionary<string, BundleData> m_AssetBundles = new Dictionary<string, BundleData>();

    /// <summary>
    /// 解析版本文件
    /// </summary>
    public void ParseVersionFile()
    {
        // 获得版本文件的路径
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);
        // 解析filelist中的文件信息
        for(int i = 0; i < data.Length; ++i)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetsName = info[0];
            bundleInfo.BundleName = info[1];
            bundleInfo.Dependences = new List<string>(info.Length - 2);
            for(int j = 2; j < info.Length; ++j)
            {
                bundleInfo.Dependences.Add(info[j]);
            }
            m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);
            // 提取Lua脚本
            if (info[0].IndexOf("Lua") > 0)
            {
                Manager.Lua.LuaNames.Add(info[0]);
            }
        }
    }

    IEnumerator LoadBundleAsync(string assetName,Action<UObject> action=null)
    {
        
        if (m_BundleInfos.ContainsKey(assetName))
        {
            string bundleName = m_BundleInfos[assetName].BundleName;
            string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
            List<string> dependences = m_BundleInfos[assetName].Dependences;
            BundleData bundle = GetBundle(bundleName);
            if (bundle == null)
            {
                UObject obj = Manager.Pool.Spawn("AssetBundle", bundleName);
                if (obj != null)
                {
                    AssetBundle ab = obj as AssetBundle;
                    bundle = new BundleData(ab);
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                    yield return request;
                    bundle = new BundleData(request.assetBundle);
                }
                m_AssetBundles.Add(bundleName, bundle);
            }

            if (dependences != null && dependences.Count > 0)
            {
                for (int i = 0; i < dependences.Count; ++i)
                {
                    yield return LoadBundleAsync(dependences[i]);
                }
            }

            // 场景加载不可以使用bundle.LoadAssetAsync
            if (assetName.EndsWith(".unity"))
            {
                action?.Invoke(null);
                yield break;
            }

            // 当加载的是依赖资源时没有回调，直接退出
            if (action == null)
            {
                yield break;
            }

            AssetBundleRequest bundleRequest = bundle.bundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            /*if (action != null && bundleRequest != null)
            {
                action.Invoke(bundleRequest.asset);
            }*/
            action?.Invoke(bundleRequest?.asset);
        }
        else
        {
            Debug.LogError("缺少文件: "+assetName);
        }
    }

    BundleData GetBundle(string bundleName)
    {
        BundleData bundle = null;
        if(m_AssetBundles.TryGetValue(bundleName, out bundle))
        {
            ++(bundle.Count);
        }
        return bundle;
    }

    private void DeleteOneBundleCount(string bundleName)
    {
        BundleData bundle;
        if (m_AssetBundles.TryGetValue(bundleName,out bundle))
        {
            if (bundle.Count > 0)
            {
                --(bundle.Count);
                Debug.Log("bundle引用计数: " + bundleName + " count: " + bundle.Count);
            }
            if (bundle.Count <= 0)
            {
                Debug.Log("放入bundle对象池: " + bundleName);
                Manager.Pool.UnSpawn("AssetBundle", bundleName, bundle.bundle);
                m_AssetBundles.Remove(bundleName);
            }
        }
    }

    public void DeleteBundleCount(string assetName)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        DeleteOneBundleCount(bundleName);
        List<string> dependences = m_BundleInfos[assetName].Dependences;
        if (dependences != null)
        {
            foreach(string dependence in dependences)
            {
                string name = m_BundleInfos[dependence].BundleName;
                DeleteOneBundleCount(name);
            }
        }
    }

    /// <summary>
    /// 编辑器环境下加载资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
#if UNITY_EDITOR
    void EditorLoadAsset(string assetName, Action<UObject> action = null)
    {
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
        if (obj == null)
        {
            Debug.LogError("assets name is not exist: " + assetName);
        }
        action?.Invoke(obj);

    }
#endif

    private void LoadAsset(string assetName,Action<UObject> action)
    {
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            Debug.Log("EditorMode");
            EditorLoadAsset(assetName, action);
        }
        else
#endif
        {
            Debug.Log("PackageMode");
            StartCoroutine(LoadBundleAsync(assetName, action));
        }
        
    }

    public void LoadUI(string assetName,Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetUIPath(assetName), action);
    }
    /// <summary>
    /// 加载音效
    /// </summary>
    /// <param name="assetName">带后缀的资源名，以适应不同格式的音效</param>
    /// <param name="action"></param>
    public void LoadSound(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }
    /// <summary>
    /// 加载音乐资源
    /// </summary>
    /// <param name="assetName">带后缀的资源名，以适应不同格式的音乐</param>
    /// <param name="action"></param>
    public void LoadMusic(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }
    public void LoadScene(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetScenePath(assetName), action);
    }
    public void LoadSprite(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetSpritePath(assetName), action);
    }
    public void LoadEffect(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }
    public void LoadModel(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(PathUtil.GetModelPath(assetName), action);
    }
    public void LoadLua(string assetName, Action<UObject> action = null)
    {
        this.LoadAsset(assetName, action);
    }

    public void LoadPrefab(string assetName,Action<UObject> action = null)
    {
        this.LoadAsset(assetName, action);
    }

    public void UnloadBundle(UObject obj)
    {
        AssetBundle ab = obj as AssetBundle;
        ab.Unload(true);
    }
}
