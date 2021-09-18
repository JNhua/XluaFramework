using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;
    void Start()
    {
        // 订阅Lua脚本加载初始化完成的事件，加载成功后再执行
        Manager.Event.Subscribe(1, OnLuaInit);
        AppConst.GameMode = this.GameMode;
        Manager.Resource.ParseVersionFile();
        // 匿名委托替换为订阅事件触发
        /*Manager.Lua.Init(
            () =>
            {

                Manager.Lua.RunLua("test");
            }
            );*/
        Manager.Lua.Init();
        DontDestroyOnLoad(this);
    }

    void OnLuaInit(object args)
    {
        //Manager.Lua.RunLua("test");
        Manager.Lua.RunLua("testJson");

        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Effect", 120);
        Manager.Pool.CreateAssetPool("AssetBundle", 10);
    }

    public void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe(1, OnLuaInit);
    }
}
