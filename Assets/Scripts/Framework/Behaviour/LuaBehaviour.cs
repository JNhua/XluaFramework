using System;
using UnityEngine;
using XLua;



public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv m_LuaEnv = Manager.Lua.luaEnv;
    protected LuaTable m_ScriptEnv;
    private Action m_LuaInit;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestroy;

    // 要绑定的脚本名,但是每次在Unity中给定脚本名不是很好，不用这种方法
    // public string luaName;

    private void Awake()
    {
        m_ScriptEnv = m_LuaEnv.NewTable();
        // 绑定部分参考XLua的example
        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = m_LuaEnv.NewTable();
        meta.Set("__index", m_LuaEnv.Global);
        m_ScriptEnv.SetMetaTable(meta);
        meta.Dispose();

        m_ScriptEnv.Set("self", this);
    }

    public virtual void Init(string luaName)
    {
        m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, m_ScriptEnv);
        m_ScriptEnv.Get("Update", out m_LuaUpdate);
        m_ScriptEnv.Get("OnInit", out m_LuaInit);
        m_LuaInit?.Invoke();
    }

    private void Update()
    {
        m_LuaUpdate?.Invoke();
    }

    protected virtual void Clear()
    {
        m_LuaOnDestroy = null;
        m_LuaInit = null;
        m_LuaUpdate = null;
        m_ScriptEnv?.Dispose();
        m_ScriptEnv = null;
    }

    private void OnDestroy()
    {
        m_LuaOnDestroy?.Invoke();
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
