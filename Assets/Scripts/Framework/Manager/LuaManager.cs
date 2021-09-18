using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    // 获取所有Lua文件的文件名
    public List<string> LuaNames = new List<string>();
    // 缓存lua脚本内容
    private Dictionary<string, byte[]> m_LuaScripts = new Dictionary<string, byte[]>();

    public LuaEnv luaEnv;

    // Action InitFinish;
    public void Init()
    {
        // InitFinish += init;
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(Loader);
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            EditorLoadLuaScript();
        }
        else
#endif
        {
            LoadLuaScript();
        }
    }

    private void OnDestroy()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }
    }
    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    public byte[] GetLuaScript(string name)
    {
        // 符合调用习惯，将路径中的"/"修改为"."
        name = name.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(name);
        byte[] luaScript = null;
        if(!m_LuaScripts.TryGetValue(fileName,out luaScript))
        {
            Debug.LogError("lua script is not exist:" + fileName);
        }
        return luaScript;
    }

    public void RunLua(string name)
    {
        luaEnv.DoString(string.Format("require '{0}'", name));
    }

    private void AddLuaScript(string assetName,byte[] luaScript)
    {
        m_LuaScripts[assetName] = luaScript;
    }

#if UNITY_EDITOR
    void EditorLoadLuaScript()
    {
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for(int i = 0; i < luaFiles.Length; ++i)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScript(PathUtil.GetUnityPath(fileName), file);
        }
        // InitFinish?.Invoke();
        Manager.Event.Notify(1);
    }
#endif

    void LoadLuaScript()
    {
        foreach(string name in LuaNames)
        {
            Manager.Resource.LoadLua(name, (UnityEngine.Object obj) =>
             {
                 AddLuaScript(name, (obj as TextAsset).bytes);
                 if (m_LuaScripts.Count >= LuaNames.Count)
                 {
                     Debug.Log("加载完成.");
                     // InitFinish?.Invoke();
                     Manager.Event.Notify(1);
                     LuaNames.Clear();
                     LuaNames = null;
                 }
             });
        }
    }


    // 每一帧调用xLua提供的接口回收内存
    private void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }
}

