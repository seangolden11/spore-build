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

        GenerateIcon(CreateManager.instance.mainBody, name);

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

    public void GenerateIcon(GameObject prefab, string iconname)
    {
        // 프리팹 이름 가져오기
        string prefabName = iconname;

        // 전용 카메라 생성
        GameObject cameraObj = new GameObject("IconCamera");
        Camera iconCamera = cameraObj.AddComponent<Camera>(); // 아이콘을 찍을 카메라

        // 카메라 설정
        iconCamera.clearFlags = CameraClearFlags.SolidColor; // 배경 제거
        iconCamera.backgroundColor = Color.clear;
        iconCamera.orthographic = true;  // 정사각형 렌더링
        iconCamera.nearClipPlane = 0.1f;
        iconCamera.farClipPlane = 10f;

        // **새로운 Light 추가**
        GameObject lightObj = new GameObject("IconLight");
        Light iconLight = lightObj.AddComponent<Light>();
        iconLight.type = LightType.Directional;  // 방향광 (전체를 밝히는 조명)
        iconLight.intensity = 0.5f;  // 빛의 세기 조정
        //iconLight.color = Color.white; // 빛 색상


        // 프리팹 인스턴스 생성
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3 (100,100,100); // 중앙에 배치

        // 프리팹 크기 측정 후 카메라 위치 조정
        Bounds bounds = GetBounds(instance);
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        iconCamera.orthographicSize = size * 0.6f;  // 크기 조정 (여백을 남기기 위해 0.6배)
        /*
        // 프리팹의 **오른쪽에서 왼쪽을 바라보도록 카메라 배치**
        iconCamera.transform.position = instance.transform.position + new Vector3(bounds.max.x + size, 0, 0);
        iconCamera.transform.LookAt(instance.transform.position); // 프리팹 중심을 바라봄
        */
        float distance = size * 2.0f;
        iconCamera.transform.position = bounds.center + new Vector3(-distance, 0, 0);
        iconCamera.transform.LookAt(bounds.center);


        // **Light를 카메라 앞에 배치**
        lightObj.transform.position = iconCamera.transform.position;
        lightObj.transform.rotation = iconCamera.transform.rotation;
        lightObj.transform.parent = iconCamera.transform;

        // 렌더 텍스처 설정
        RenderTexture renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        iconCamera.targetTexture = renderTexture;

        // 카메라 렌더링 실행
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
        Destroy(cameraObj);
        renderTexture.Release();
    }

    // 오브젝트의 전체 크기(Bounds)를 가져오는 함수
    private Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
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

