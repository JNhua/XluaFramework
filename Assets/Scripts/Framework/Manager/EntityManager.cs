using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    Dictionary<string, GameObject> m_Entity = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> m_EntityGroups = new Dictionary<string, Transform>();

    private Transform m_EntityParent;

    private void Awake()
    {
        m_EntityParent = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroup(List<string> groups)
    {
        foreach(string group in groups)
        {
            GameObject go = new GameObject("Group-"+ group);
            go.transform.SetParent(m_EntityParent,false);
            m_EntityGroups.Add(group, go.transform);
        }
    }

    Transform GetEntityGroup(string groupName)
    {
        if (!m_EntityGroups.ContainsKey(groupName))
        {
            Debug.LogError("EntityGroup \"" + groupName + "\" is not exist");
        }
        return m_EntityGroups[groupName];
    }

    public void ShowEntity(string entityName,string groupName,string luaName)
    {
        GameObject go = null;
        if(m_Entity.TryGetValue(entityName,out go))
        {
            EntityLogic entityLogic = go.GetComponent<EntityLogic>();
            entityLogic.OnShow();
            return;
        }
        Manager.Resource.LoadPrefab(entityName,(UnityEngine.Object obj) =>
        {
            go = Instantiate(obj) as GameObject;
            m_Entity.Add(entityName, go);
            EntityLogic entityLogic = go.AddComponent<EntityLogic>();
            Transform parent = GetEntityGroup(groupName);
            go.transform.SetParent(parent, false);
            entityLogic.Init(luaName);
            entityLogic.OnShow();
        });
    }
}
