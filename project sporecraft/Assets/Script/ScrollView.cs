using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System.IO;

public class ScrollView : MonoBehaviour
{
    private ScrollRect scrollRect;

    public float space = 50f;
    public int maxSlot = 10;

    public GameObject uiPrefab;

    public List<GameObject> prefabs = new List<GameObject>();
    public List<Sprite> sprites = new List<Sprite>();
    
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
        string prefabFolder = "Assets/CreatureData/CreaturePrefabs";
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
        prefabs.Clear();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        }

        string iconFolder = "Assets/CreatureData/CreatureIcon";
        guids = AssetDatabase.FindAssets("t:Texture2D", new[] { iconFolder });
        sprites.Clear();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            


            sprites.Add(LoadSpriteFromFile(path));
        }

        for (int i = 0; i<maxSlot; i++)
        {
            if (prefabs.Count > i)
                uiObjects[i].GetComponent<DataContent>().Init(prefabs[i], sprites[i]);
            else
                uiObjects[i].GetComponent<DataContent>().Init(null,null);
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

    public Sprite LoadSpriteFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            // ������ byte[]�� �ε� �� Texture2D�� ��ȯ
            byte[] pngData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2); // ũ��� �ڵ� ������
            texture.LoadImage(pngData);
            texture.Apply();

            // Texture2D�� Sprite�� ��ȯ
            Sprite iconSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // UI Image�� ����
           
           
            

            Debug.Log("�������� UI�� �����߽��ϴ�.");
            return iconSprite;
        }
        else
        {
            Debug.LogWarning($"������ ������ ã�� �� �����ϴ�: {filePath}");
        }

        return null;
    }




}
