using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class ObjectDataManager : MonoBehaviour
{
    public void SaveCapsule(string name)
    {
       

        MeshFilter meshFilter = CreateManager.instance.mainBody.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            string meshPath = "Assets/CreatureData/CreatureMeshs/" + name + ".mesh";
            Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (existingMesh == null)
            {
                AssetDatabase.CreateAsset(meshFilter.sharedMesh, meshPath);
                AssetDatabase.SaveAssets();
            }
            meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        }

        string localpath = "Assets/CreatureData/CreaturePrefabs/" + name + ".prefab";

        localpath = AssetDatabase.GenerateUniqueAssetPath(localpath);

        PrefabUtility.SaveAsPrefabAssetAndConnect(CreateManager.instance.mainBody, localpath, InteractionMode.UserAction);

        GenerateIcon(CreateManager.instance.mainBody);

        Debug.Log("ĸ�� �����Ͱ� ����Ǿ����ϴ�: " + localpath);
    }

    public void LoadGameObject(string name)
    {
        string prefabFolder = "Assets/CreatureData/CreaturePrefabs";
        string[] prefabPaths = AssetDatabase.FindAssets($"{name} t:Prefab", new[] { prefabFolder });

        if (prefabPaths.Length > 0)
        {
            // ù ��° ������ �ε�
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabPaths[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                // ���� ��ü ����
                if (CreateManager.instance.mainBody != null)
                {
                    Destroy(CreateManager.instance.mainBody);
                }

                // ������ �ν��Ͻ�ȭ
                GameObject prefabInstance = Instantiate(prefab);
                CreateManager.instance.mainBody = prefabInstance;
                prefabInstance.GetComponent<ProceduralCapsule>().CallOnLoad();

                Debug.Log("�������� �ν��Ͻ�ȭ�Ǿ����ϴ�: " + prefabInstance.name);
            }
            else
            {
                Debug.LogWarning("������ �ε忡 �����߽��ϴ�: " + prefabPath);
            }
        }
        else
        {
            Debug.LogWarning("������ �������� �����ϴ�.");
        }
    }



   

    public void GenerateIcon(GameObject prefab)
    {

        Camera iconCamera;  // �������� ���� ī�޶�
        RenderTexture renderTexture;  // ī�޶��� ���
        

        // ������ �̸� ��������
        string prefabName = prefab.name;

        iconCamera = Camera.main;
        renderTexture =  new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);


        // ������ �ν��Ͻ� ����
        GameObject instance = Instantiate(prefab, iconCamera.transform.position + Vector3.forward * 2, Quaternion.identity);

        // ī�޶� ������ ����
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // RenderTexture�� Texture2D�� ��ȯ
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // Texture2D�� Sprite�� ��ȯ
        Sprite iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));

       

        // PNG�� ����
        SaveTextureAsPNG(iconTexture, prefabName);

        // ����� ���� ������Ʈ ����
        Destroy(instance);
        renderTexture.Release();
    }

    void SaveTextureAsPNG(Texture2D texture, string fileName)
    {
        //  ��Ÿ�ӿ����� ���� ������ ���� ���� ����
        string folderPath;

#if UNITY_EDITOR
        folderPath = "Assets/CreatureData/CreatureIcon";  // �����Ϳ����� ���� ��� ����
#else
    folderPath = Path.Combine(Application.persistentDataPath, "CreatureIcon");  // ��Ÿ�ӿ����� ���� ���� ��� ���
#endif

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName + ".png");

        // Texture2D�� PNG�� ��ȯ �� ����
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);

        Debug.Log($"������ ���� �Ϸ�: {filePath}");



#if UNITY_EDITOR
        //  �����Ϳ����� AssetDatabase ���
        AssetDatabase.ImportAsset(filePath);
        AssetDatabase.Refresh();
#endif


    }



}

