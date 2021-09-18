using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    NetClient m_NetClient;
    Queue<KeyValuePair<int, string>> m_MessageQueue = new Queue<KeyValuePair<int, string>>();
    XLua.LuaFunction ReceiveMessage;

    public void Init()
    {
        m_NetClient = new NetClient();
        ReceiveMessage = Manager.Lua.luaEnv.Global.Get<XLua.LuaFunction>("ReceiveMessage");
    }

    public void SendMessage(int msgID, string message)
    {
        m_NetClient.SendMessage(msgID, message);
    }

    public void ConnectedServer(string post, int port)
    {
        m_NetClient.OnConnectServer(post, port);
    }

    public void OnNetConnected() { }

    public void OnNetDisConnected() { }

    public void Receive(int msgID, string message)
    {
        m_MessageQueue.Enqueue(new KeyValuePair<int, string>(msgID, message));
    }

    private void Update()
    {
        if (m_MessageQueue.Count > 0)
        {
            KeyValuePair<int, string> msg = m_MessageQueue.Dequeue();
            ReceiveMessage?.Call(msg.Key, msg.Value);
        }
    }
}
