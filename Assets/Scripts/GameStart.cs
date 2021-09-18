using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    public bool OpenLog;

    void Start()
    {
        // 订阅Lua脚本加载初始化完成的事件，加载成功后再执行
        Manager.Event.Subscribe((int)GameEvent.StartLua, OnLuaInit);
        Manager.Event.Subscribe((int)GameEvent.GameInit, GameInit);

        AppConst.GameMode = this.GameMode;
        AppConst.OpenLog = this.OpenLog;

        // 匿名委托替换为订阅事件触发
        /*Manager.Lua.Init(
            () =>
            {

                Manager.Lua.RunLua("test");
            }
            );*/
        DontDestroyOnLoad(this);

        if (AppConst.GameMode == GameMode.UpdateMode)
        {
            this.gameObject.AddComponent<HotUpdate>();
        }
        else
        {
            Manager.Event.Notify((int)GameEvent.GameInit);
        }
    }

    private void GameInit(object args)
    {
        if (AppConst.GameMode != GameMode.EditorMode)
        {
            Manager.Resource.ParseVersionFile();
        }
        Manager.Lua.Init();
    }

    void OnLuaInit(object args)
    {
        Manager.Lua.RunLua("test");
        //Manager.Lua.RunLua("testJson");

        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Effect", 120);
        Manager.Pool.CreateAssetPool("AssetBundle", 10);
    }

    public void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe((int)GameEvent.StartLua, OnLuaInit);
        Manager.Event.UnSubscribe((int)GameEvent.GameInit, GameInit);
    }
}
