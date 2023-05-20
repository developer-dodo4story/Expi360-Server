using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingCanvas : MonoBehaviour
{
    List<TMPro.TextMeshProUGUI> texts = new List<TMPro.TextMeshProUGUI>();
    public GameObject textPrefab;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void CreatePingTexts(int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            GameObject textGO = Instantiate(textPrefab, gameObject.transform);
            textGO.transform.localPosition = Vector3.zero;
            textGO.transform.localRotation = Quaternion.identity;
            textGO.transform.localScale = Vector3.one;
            texts.Add(textGO.GetComponent<TMPro.TextMeshProUGUI>());
        }

    }
    public void SetPingTexts(string[] _texts)
    {
        int count = _texts.Length;
        if (count > texts.Count)
        {
            CreatePingTexts(count - texts.Count);
        }
        for(int i=0;i<texts.Count;i++)
        {
            texts[i].text = _texts[i];
        }
    }
}
