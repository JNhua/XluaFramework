using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Image progressValue;
    public Text progressText;
    public Text progressDesc;
    [SerializeField]
    GameObject progressBar;

    private float m_Max;

    internal void InitProgress(float max, string desc)
    {
        m_Max = max;
        progressBar.SetActive(true);
        progressDesc.gameObject.SetActive(true);
        progressDesc.text = desc;
        progressValue.fillAmount = max > 0 ? 0 : 100;
        progressText.gameObject.SetActive(max > 0);
    }

    internal void UpdateProgress(float m_DownloadCount)
    {
        progressValue.fillAmount = m_DownloadCount / m_Max;
        progressText.text = string.Format("{0:0}%", progressValue.fillAmount * 100);
    }
}
