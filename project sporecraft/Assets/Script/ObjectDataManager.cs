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

        Debug.Log("캡슐 데이터가 저장되었습니다: " + localpath);
    }

    public void LoadGameObject(string name)
    {

        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });

        if (prefabPaths.Length > 0)
        {
            // 첫 번째 프리팹 로드
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabPaths[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                // 기존 객체 삭제
                if (CreateManager.instance.mainBody != null)
                {
                    Destroy(CreateManager.instance.mainBody);
                }

                // 프리팹 인스턴스화
                GameObject prefabInstance = Instantiate(prefab);
                CreateManager.instance.mainBody = prefabInstance;

                Debug.Log("프리팹이 인스턴스화되었습니다: " + prefabInstance.name);
            }
            else
            {
                Debug.LogWarning("프리팹 로드에 실패했습니다: " + prefabPath);
            }
        }
        else
        {
            Debug.LogWarning("폴더에 프리팹이 없습니다.");
        }
    }


    /*
    // 새 오브젝트 생성
    GameObject newCapsuleObject = new GameObject(data.objectName);
    ProceduralCapsule newCapsule = newCapsuleObject.AddComponent<ProceduralCapsule>();

    // 데이터 복원

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



    // 필요한 컴포넌트를 추가 (예: MeshFilter, SkinnedMeshRenderer 등)



    CreateManager.instance.mainBody = newCapsuleObject;
    newCapsule.make();
    for (int i = 1; i < data.listBones.Count; i++) ;
    {
        //
    }
    */


}
}
