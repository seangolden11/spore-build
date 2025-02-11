using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class ScrollView : MonoBehaviour
{
    private ScrollRect scrollRect;

    public float space = 50f;
    public int maxSlot = 10;

    public GameObject uiPrefab;

    public List<GameObject> prefabs = new List<GameObject>();
    public List<RectTransform> uiObjects = new List<RectTransform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        
        for(int i=0; i< maxSlot; i++)
        {
            AddNewUiObject();
        }

        DataInit();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DataInit()
    {
        string prefabFolder = "Assets/CreaturePrefabs";
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
        prefabs.Clear();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }

        for (int i = 0; i<maxSlot; i++)
        {
            if (prefabs.Count > i)
                uiObjects[i].GetComponent<DataContent>().Init(prefabs[i]);
            else
                uiObjects[i].GetComponent<DataContent>().Init(null);
        }
    }

    void AddNewUiObject()
    {
        var newUi = Instantiate(uiPrefab, scrollRect.content).GetComponent<RectTransform>();
        uiObjects.Add(newUi);

        float y = 0f;
        for(int i=0; i < uiObjects.Count; i++)
        {
            uiObjects[i].anchoredPosition = new Vector2(0f, -y);
            y += uiObjects[i].sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }

    
}
