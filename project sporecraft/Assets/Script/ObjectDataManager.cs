using UnityEngine;
using System.IO;
using UnityEditor;

public class ObjectDataManager : MonoBehaviour
{
    public void SaveCapsule()
    {
        /*
        ProceduralCapsule capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
        ObjectData data = new ObjectData(capsule);
        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/" + data.objectName + "_data.json";
        File.WriteAllText(path, json);
        */

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


    /*
    // �� ������Ʈ ����
    GameObject newCapsuleObject = new GameObject(data.objectName);
    ProceduralCapsule newCapsule = newCapsuleObject.AddComponent<ProceduralCapsule>();

    // ������ ����

    newCapsule.subdivisionHeight = data.subdivisionHeight;
    newCapsule.subdivisionAround = data.subdivisionAround;
    newCapsule.radius = data.radius;
    newCapsule.height = data.height;
    newCapsule.cylinderDivision = data.cylinderDivision;
    newCapsule.topOffest = data.topOffset;
    newCapsule.numberOfCylinder = 0;
    newCapsule.botOffset = data.botOffset;
    newCapsule.bone = data.bone;
    newCapsule.tempTrans = data.tempTrans;
    newCapsule.isFirst = false;
    newCapsule.transform.position = data.position;
    newCapsule.transform.rotation = data.rotation;



    // �ʿ��� ������Ʈ�� �߰� (��: MeshFilter, SkinnedMeshRenderer ��)



    CreateManager.instance.mainBody = newCapsuleObject;
    newCapsule.make();
    for (int i = 1; i < data.listBones.Count; i++) ;
    {
        //
    }
    */


}
}
