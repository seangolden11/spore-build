using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;

public class ObjectDataManager : MonoBehaviour
{

    public static ObjectDataManager instance;

    private string saveBasePath;
    string meshesPath;
    string iconsPath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 경로 설정
        saveBasePath = Application.persistentDataPath + "/CreatureData/";


       
        meshesPath = saveBasePath + "CreatureMeshs/";
        iconsPath = saveBasePath + "CreatureIcons/";

        // 필요한 디렉토리 생성
        EnsureDirectoryExists(saveBasePath);
        EnsureDirectoryExists(meshesPath);
        EnsureDirectoryExists(iconsPath);
    }

    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    public void SaveCapsule(string name)
    {
        GameObject capsule = CreateManager.instance.mainBody;

        // 메시 데이터 추출
        MeshFilter meshFilter = capsule.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("메시 필터가 없거나 메시가 없습니다.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        ProceduralCapsule dummycapsule = capsule.GetComponent<ProceduralCapsule>();

        /*
        Quaternion rotationOffset = Quaternion.Euler(-90f, 0f, 0f);

        // 회전 적용하여 정점 변환
        Vector3[] worldVertices = mesh.vertices
            .Select(v => capsule.transform.TransformPoint(v)) // 월드 좌표로 변환
            .Select(v => rotationOffset * (v - capsule.transform.position) + capsule.transform.position) // -90도 회전 적용
            .ToArray();
        */
        
        // 캡슐 데이터 생성
        ObjectData capsuleData = new ObjectData
        {
            name = name,
            vertices = mesh.vertices,
            triangles = mesh.triangles,
            normals = mesh.normals,
            uv = mesh.uv,
            position = capsule.transform.position,
            rotation = capsule.transform.eulerAngles,
            scale = capsule.transform.localScale,
            subdivisionHeight = dummycapsule.subdivisionHeight,
            subdivisionAround = dummycapsule.subdivisionAround,
            radius = dummycapsule.radius,
            height = dummycapsule.height,
            cylinderDivision = dummycapsule.cylinderDivision,
            topOffest = dummycapsule.topOffest,
            numberOfCylinder = dummycapsule.numberOfCylinder,
            botOffset = dummycapsule.botOffset,
            listBonesPos = dummycapsule.listBones.Where(t => t != null).Select(t => t.position).ToList(),
            listBonesRot = dummycapsule.listBones.Where(t => t != null).Select(t => t.eulerAngles).ToList(),
            temptransPos = dummycapsule.tempTrans.Where(t => t != null).Select(t => t.position).ToList(),
            listLocalBones = dummycapsule.listLocalBones,
            materialNum = MaterialManager.instance.materials.FindIndex(f => f == dummycapsule.mat),
            
        };
        /*
        // 머테리얼 색상이 있으면 저장
        Renderer renderer = capsule.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            capsuleData.color = renderer.material.color;
        }*/

        // JSON으로 직렬화하여 저장
        string jsonData = JsonUtility.ToJson(capsuleData, true);
        string filePath = meshesPath + name + ".json";
        File.WriteAllText(filePath, jsonData);

       

        // 아이콘 생성 및 저장
        GenerateIcon(capsule, name);

        Debug.Log("캡슐 데이터가 저장되었습니다: " + filePath);
    }

    public void DeleteCapsule(string name)
    {
        string meshPath = meshesPath + name + ".json";
        string iconPath = iconsPath + name + ".png";

        bool deleted = false;

        // 메시 파일 삭제
        if (File.Exists(meshPath))
        {
            File.Delete(meshPath);
            deleted = true;
            Debug.Log("메시 파일이 삭제되었습니다: " + meshPath);
        }


        // 아이콘 이미지 삭제
        if (File.Exists(iconPath))
        {
            File.Delete(iconPath);
            deleted = true;
            Debug.Log("아이콘 파일이 삭제되었습니다: " + iconPath);
        }

        if (deleted)
        {
            Debug.Log("캡슐 데이터가 성공적으로 삭제되었습니다: " + name);
        }
        else
        {
            Debug.LogWarning("삭제할 캡슐 데이터를 찾을 수 없습니다: " + name);
        }
    }

    public GameObject LoadCapsule(string name)
    {
        if (CreateManager.instance != null)
            CreateManager.instance.bodyClick.ResetClick();

        string filePath = meshesPath + name + ".json";
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("캡슐 데이터 파일이 존재하지 않습니다: " + filePath);
            return null;
        }

        // 메시 데이터 로드
        string jsonData = File.ReadAllText(filePath);
        ObjectData capsuleData = JsonUtility.FromJson<ObjectData>(jsonData);

        // 기본 게임오브젝트 생성
        GameObject capsule = new GameObject(name);

        // Transform 정보 로드 및 적용
        capsule.transform.position = capsuleData.position;
        capsule.transform.rotation = Quaternion.Euler(capsuleData.rotation);
        capsule.transform.localScale = capsuleData.scale;

        // 메시 생성
        Mesh mesh = new Mesh();

        mesh.vertices = capsuleData.vertices;
        mesh.triangles = capsuleData.triangles;
        mesh.normals = capsuleData.normals;
        mesh.uv = capsuleData.uv;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        ProceduralCapsule procap = capsule.AddComponent<ProceduralCapsule>();
        
        procap.subdivisionHeight = capsuleData.subdivisionHeight;
        procap.subdivisionAround = capsuleData.subdivisionAround;
        procap.height = capsuleData.height;
        procap.radius = capsuleData.radius;
        procap.cylinderDivision = capsuleData.cylinderDivision;
        procap.topOffest = capsuleData.topOffest;
        procap.numberOfCylinder = capsuleData.numberOfCylinder;
        procap.botOffset = capsuleData.botOffset;
        
        procap.listLocalBones = capsuleData.listLocalBones;

        
        procap.mat = MaterialManager.instance.materials[capsuleData.materialNum];




        

        procap.LoadCapsule(mesh, capsuleData.listBonesPos,capsuleData.listBonesRot, capsuleData.temptransPos);

        capsule.AddComponent<Player>().enabled = false;
        
        if (CreateManager.instance != null)
        {
            Destroy(CreateManager.instance.mainBody);
            CreateManager.instance.mainBody = capsule;
        }
        else if(GameManager.instance != null)
        {
            GameManager.instance.mainBody = capsule;
        }
        

        return capsule;
    }

    public void GenerateIcon(GameObject prefab, string iconName)
    {

        CreateManager.instance.outline.Hideoutline();
        CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>().otherCilceked();
        
        // 렌더 텍스처 설정
        RenderTexture renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);

        // 임시 카메라 생성
        GameObject cameraObj = new GameObject("IconCamera");
        Camera iconCamera = cameraObj.AddComponent<Camera>();

        // 카메라 설정
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = Color.clear;
        iconCamera.orthographic = true;

        //bound 계산
        Bounds bounds = GetBounds(prefab);
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        iconCamera.nearClipPlane = size * 0.1f;
        iconCamera.farClipPlane = size * 3f;
        


        // 조명 추가
        GameObject lightObj = new GameObject("IconLight");
        Light iconLight = lightObj.AddComponent<Light>();
        iconLight.type = LightType.Directional;
        iconLight.intensity = 0.5f;

        // 조명을 카메라 위치에 배치
        lightObj.transform.position = iconCamera.transform.position;
        lightObj.transform.rotation = iconCamera.transform.rotation;
        lightObj.transform.parent = iconCamera.transform;

        // 프리팹 크기 계산
        
        
        iconCamera.orthographicSize = size * 0.6f;

        // 카메라를 오른쪽에서 왼쪽을 바라보도록 배치
        float distance = size * 2.0f;
        iconCamera.transform.position = bounds.center + new Vector3(distance, 0, 0);
        iconCamera.transform.LookAt(bounds.center);
        Debug.Log(iconCamera.transform.position);
        Debug.Log(bounds.center);

       
        




        // 렌더 타겟 설정
        iconCamera.targetTexture = renderTexture;
        
        iconCamera.Render();

        // 렌더 텍스처를 텍스처로 변환
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // PNG로 저장
        byte[] bytes = iconTexture.EncodeToPNG();
        string iconPath = iconsPath + iconName + ".png";
        File.WriteAllBytes(iconPath, bytes);

        // 임시 객체 정리
        Destroy(cameraObj);
        Destroy(lightObj);
        renderTexture.Release();

        Debug.Log("아이콘이 저장되었습니다: " + iconPath);
    }

    // 객체의 경계(바운드) 계산
    private Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        return bounds;
    }



}

