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

        Debug.Log("캡슐 데이터가 저장되었습니다: " + localpath);
    }

    public void LoadGameObject(string name)
    {
        string prefabFolder = "Assets/CreatureData/CreaturePrefabs";
        string[] prefabPaths = AssetDatabase.FindAssets($"{name} t:Prefab", new[] { prefabFolder });

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
                prefabInstance.GetComponent<ProceduralCapsule>().CallOnLoad();

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



   

    public void GenerateIcon(GameObject prefab)
    {

        Camera iconCamera;  // 아이콘을 찍을 카메라
        RenderTexture renderTexture;  // 카메라의 출력
        

        // 프리팹 이름 가져오기
        string prefabName = prefab.name;

        iconCamera = Camera.main;
        renderTexture =  new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);


        // 프리팹 인스턴스 생성
        GameObject instance = Instantiate(prefab, iconCamera.transform.position + Vector3.forward * 2, Quaternion.identity);

        // 카메라 렌더링 실행
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // RenderTexture를 Texture2D로 변환
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // Texture2D를 Sprite로 변환
        Sprite iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));

       

        // PNG로 저장
        SaveTextureAsPNG(iconTexture, prefabName);

        // 사용이 끝난 오브젝트 제거
        Destroy(instance);
        renderTexture.Release();
    }

    void SaveTextureAsPNG(Texture2D texture, string fileName)
    {
        //  런타임에서도 접근 가능한 저장 폴더 설정
        string folderPath;

#if UNITY_EDITOR
        folderPath = "Assets/CreatureData/CreatureIcon";  // 에디터에서는 기존 경로 유지
#else
    folderPath = Path.Combine(Application.persistentDataPath, "CreatureIcon");  // 런타임에서는 저장 가능 경로 사용
#endif

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName + ".png");

        // Texture2D를 PNG로 변환 후 저장
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);

        Debug.Log($"아이콘 저장 완료: {filePath}");



#if UNITY_EDITOR
        //  에디터에서만 AssetDatabase 사용
        AssetDatabase.ImportAsset(filePath);
        AssetDatabase.Refresh();
#endif


    }



}

