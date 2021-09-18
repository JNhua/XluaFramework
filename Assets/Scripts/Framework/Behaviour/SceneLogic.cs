using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLogic : LuaBehaviour
{
    public string sceneName;

    Action m_luaOnActive;
    Action m_luaOnUnactive;
    Action m_luaOnEnter;
    Action m_luaOnQuit;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnActive", out m_luaOnActive);
        m_ScriptEnv.Get("OnUnactive", out m_luaOnUnactive);
        m_ScriptEnv.Get("OnEnter", out m_luaOnEnter);
        m_ScriptEnv.Get("OnQuit", out m_luaOnQuit);
    }

    public void OnActive()
    {
        m_luaOnActive?.Invoke();
    }
    public void OnUnactive()
    {
        m_luaOnUnactive?.Invoke();
    }
    public void OnEnter()
    {
        m_luaOnEnter?.Invoke();
    }
    public void OnQuit()
    {
        m_luaOnQuit?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        m_luaOnActive = null;
        m_luaOnUnactive = null;
        m_luaOnEnter = null;
        m_luaOnQuit = null;
    }
}
