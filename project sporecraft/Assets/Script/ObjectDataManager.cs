using UnityEngine;
using System.IO;
using UnityEditor;

public class ObjectDataManager : MonoBehaviour
{
    public void SaveCapsule()
    {
       

        MeshFilter meshFilter = CreateManager.instance.mainBody.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            string meshPath = "Assets/CreatureMeshs/" +"name" + ".mesh";
            Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (existingMesh == null)
            {
                AssetDatabase.CreateAsset(meshFilter.sharedMesh, meshPath);
                AssetDatabase.SaveAssets();
            }
            meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        }

        string localpath = "Assets/CreaturePrefabs/" + CreateManager.instance.mainBody.name + ".prefab";

        localpath = AssetDatabase.GenerateUniqueAssetPath(localpath);

        PrefabUtility.SaveAsPrefabAssetAndConnect(CreateManager.instance.mainBody, localpath, InteractionMode.UserAction);

        Debug.Log("ĸ�� �����Ͱ� ����Ǿ����ϴ�: " + localpath);
    }

    public void LoadGameObject(string name)
    {
        string prefabFolder = "Assets/CreaturePrefabs";
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });

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


   


}

