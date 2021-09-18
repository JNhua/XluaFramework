using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private string m_LogicName = "[SceneLogic]";
    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    public void LoadScene(string sceneName,string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
         {
             StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
         });
    }
    private void OnActiveSceneChanged(Scene s1,Scene s2)
    {
        if (!s1.isLoaded || !s2.isLoaded)
            return;
        SceneLogic logic1 = GetSceneLogic(s1);
        SceneLogic logic2 = GetSceneLogic(s2);

        logic1?.OnUnactive();
        logic2?.OnActive();
    }

    public void SetActive(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        Debug.LogFormat("SetActiveScene: {0}",sceneName);
        SceneManager.SetActiveScene(scene);
    }

    public bool IsLoadedScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }
    /// <summary>
    ///  切换场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="luaName"></param>
    public void ChangeScene(string sceneName,string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) => {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
        });
    }

    public void UnLoadSceneAsync(string sceneName)
    {
        StartCoroutine(UnLoadScene(sceneName));
    }

    IEnumerator StartLoadScene(string sceneName,string luaName,LoadSceneMode mode)
    {
        if (IsLoadedScene(sceneName))
            yield break;
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation = true;
        yield return async;
        Scene scene = SceneManager.GetSceneByName(sceneName);
        GameObject go = new GameObject(m_LogicName);
        SceneManager.MoveGameObjectToScene(go, scene);
        Debug.Log(sceneName);
        SceneLogic sceneLogic = go.AddComponent<SceneLogic>();
        sceneLogic.sceneName = sceneName;
        sceneLogic.Init(luaName);
        sceneLogic.OnEnter();
    }

    IEnumerator UnLoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogError("Scene " + sceneName + " is not loaded");
            yield break;
        }
        SceneLogic sceneLogic = GetSceneLogic(scene);
        sceneLogic.OnQuit();
        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }
    
    private SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if (go.name.CompareTo(m_LogicName) == 0)
            {
                SceneLogic sceneLogic = go.GetComponent<SceneLogic>();
                return sceneLogic;
            }
        }
        return null;
    }
}
