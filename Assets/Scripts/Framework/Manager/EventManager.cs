using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // 供Lua使用的回调，可以传入一个参数，多个参数可以使用table
    public delegate void EventHandler(object args);

    Dictionary<int, EventHandler> m_Events = new Dictionary<int, EventHandler>(); 

    public void Subscribe(int id,EventHandler e)
    {
        if (m_Events.ContainsKey(id))
        {
            m_Events[id] += e;
        }
        else
        {
            m_Events.Add(id, e);
        }
    }
    public void UnSubscribe(int id, EventHandler e)
    {
        if (m_Events.ContainsKey(id))
        {
            if(m_Events!=null)
                m_Events[id] += e;
            if (m_Events == null)
                m_Events.Remove(id);
        }
    }

    public void Notify(int id,object args = null)
    {
        EventHandler handler;
        if(m_Events.TryGetValue(id,out handler))
        {
            handler(args);
        }
    }
}
