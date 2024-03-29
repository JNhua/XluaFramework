﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadonlyPathFileListData;
    byte[] m_ServerFileListData;
    int m_DownloadCount;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }

    GameObject loadingObj;
    LoadingUI loadingUI;

    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("LoadingUI");
        loadingObj = Instantiate(go);
        loadingObj.transform.SetParent(this.transform);
        loadingUI = loadingObj.GetComponent<LoadingUI>();

        if (IsFirstInstall())
        {
            Debug.Log("IsFirstInstall");
            ReleaseResources();
        }
        else
        {
            Debug.Log("IsNotFirstInstall");
            CheckUpdate();
        }
    }

    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourceUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownloadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo info)
    {
        m_DownloadCount = 0;
        m_ServerFileListData = info.fileData.data;
        List<DownFileInfo> fileInfos = GetFileInfos(info.fileData.text, AppConst.ResourceUrl);
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for (int i = 0; i < fileInfos.Count; ++i)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);

            // 校验文件
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourceUrl, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownloadFiles(downListFiles, OnUpdateFileComplete, OnUpdateAllFilesComplete));
            loadingUI.InitProgress(downListFiles.Count, "正在更新...");
        }
        else
        {
            EnterGame();
        }
    }

    private void EnterGame()
    {
        //Manager.Resource.ParseVersionFile();
        //Manager.Resource.LoadUI("TestButton", OnComplete);
        Manager.Event.Notify((int)GameEvent.GameInit);
        Destroy(loadingObj);
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }

    private void OnUpdateAllFilesComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
        loadingUI.InitProgress(0, "正在载入...");
    }

    private void OnUpdateFileComplete(DownFileInfo info)
    {
        Debug.LogFormat("OnUpdateFileComplete: {0}", info.fileName);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(writeFile, info.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private void ReleaseResources()
    {
        m_DownloadCount = 0;
        string url = Path.Combine(PathUtil.ReadonlyPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownloadFile(info, OnDownloadReadonlyPathFilelistComplete));
    }

    private void OnDownloadReadonlyPathFilelistComplete(DownFileInfo info)
    {
        m_ReadonlyPathFileListData = info.fileData.data;
        List<DownFileInfo> fileInfos = GetFileInfos(info.fileData.text, PathUtil.ReadonlyPath);
        StartCoroutine(DownloadFiles(fileInfos, OnReleaseFileComplete, OnReleaseAllFilesComplete));
        loadingUI.InitProgress(fileInfos.Count, "释放资源，不消耗流量...");
    }

    private void OnReleaseAllFilesComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadonlyPathFileListData);
        CheckUpdate();
    }

    private void OnReleaseFileComplete(DownFileInfo info)
    {
        Debug.LogFormat("OnReleaseFileComplete: {0}", info.fileName);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(writeFile, info.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private bool IsFirstInstall()
    {
        // 检查streamingAssets文件夹中是否存在Filelist文件
        bool isReadonlyPathHasFilelist = FileUtil.IsExists(Path.Combine(PathUtil.ReadonlyPath, AppConst.FileListName));
        // 检查persistentData文件夹中是否存在Filelist文件
        bool isReadWritePathHasFilelist = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isReadonlyPathHasFilelist && !isReadWritePathHasFilelist;
    }


    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator DownloadFile(DownFileInfo fileInfo, Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(fileInfo.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError("下载文件出错: " + fileInfo.url);
            yield break;
            // 重试逻辑
            // DoSomething...
        }

        // for debug
        yield return new WaitForSeconds(0.2f);

        fileInfo.fileData = webRequest.downloadHandler;
        Complete?.Invoke(fileInfo);
        webRequest.Dispose();
    }

    IEnumerator DownloadFiles(List<DownFileInfo> fileInfos, Action<DownFileInfo> Complete, Action AllDownloadComplete)
    {
        foreach (DownFileInfo info in fileInfos)
        {
            yield return DownloadFile(info, Complete);
        }
        AllDownloadComplete?.Invoke();
    }

    private List<DownFileInfo> GetFileInfos(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downloadFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; ++i)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downloadFileInfos.Add(fileInfo);
        }
        return downloadFileInfos;
    }
}
