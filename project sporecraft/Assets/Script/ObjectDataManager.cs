using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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

        // ��� ����
        saveBasePath = Application.persistentDataPath + "/CreatureData/";


       
        meshesPath = saveBasePath + "CreatureMeshs/";
        iconsPath = saveBasePath + "CreatureIcons/";

        // �ʿ��� ���丮 ����
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

        // �޽� ������ ����
        MeshFilter meshFilter = capsule.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("�޽� ���Ͱ� ���ų� �޽ð� �����ϴ�.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        ProceduralCapsule dummycapsule = capsule.GetComponent<ProceduralCapsule>();

        // ĸ�� ������ ����
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
};
        /*
        // ���׸��� ������ ������ ����
        Renderer renderer = capsule.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            capsuleData.color = renderer.material.color;
        }*/

        // JSON���� ����ȭ�Ͽ� ����
        string jsonData = JsonUtility.ToJson(capsuleData, true);
        string filePath = meshesPath + name + ".json";
        File.WriteAllText(filePath, jsonData);

       

        // ������ ���� �� ����
        GenerateIcon(capsule, name);

        Debug.Log("ĸ�� �����Ͱ� ����Ǿ����ϴ�: " + filePath);
    }

    public void DeleteCapsule(string name)
    {
        string meshPath = meshesPath + name + ".json";
        string iconPath = iconsPath + name + ".png";

        bool deleted = false;

        // �޽� ���� ����
        if (File.Exists(meshPath))
        {
            File.Delete(meshPath);
            deleted = true;
            Debug.Log("�޽� ������ �����Ǿ����ϴ�: " + meshPath);
        }


        // ������ �̹��� ����
        if (File.Exists(iconPath))
        {
            File.Delete(iconPath);
            deleted = true;
            Debug.Log("������ ������ �����Ǿ����ϴ�: " + iconPath);
        }

        if (deleted)
        {
            Debug.Log("ĸ�� �����Ͱ� ���������� �����Ǿ����ϴ�: " + name);
        }
        else
        {
            Debug.LogWarning("������ ĸ�� �����͸� ã�� �� �����ϴ�: " + name);
        }
    }

    public GameObject LoadCapsule(string name)
    {
        string filePath = meshesPath + name + ".json";
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("ĸ�� ������ ������ �������� �ʽ��ϴ�: " + filePath);
            return null;
        }

        // �޽� ������ �ε�
        string jsonData = File.ReadAllText(filePath);
        ObjectData capsuleData = JsonUtility.FromJson<ObjectData>(jsonData);

        // �޽� ����
        Mesh mesh = new Mesh();
        mesh.vertices = capsuleData.vertices;
        mesh.triangles = capsuleData.triangles;
        mesh.normals = capsuleData.normals;
        mesh.uv = capsuleData.uv;
        mesh.RecalculateBounds();

        // �⺻ ���ӿ�����Ʈ ����
        GameObject capsule = new GameObject(name);

        ProceduralCapsule procap = capsule.AddComponent<ProceduralCapsule>();
        
        procap.subdivisionHeight = capsuleData.subdivisionHeight;
        procap.subdivisionAround = capsuleData.subdivisionAround;
        procap.height = capsuleData.height;
        procap.radius = capsuleData.radius;
        procap.cylinderDivision = capsuleData.cylinderDivision;
        procap.topOffest = capsuleData.topOffest;
        procap.numberOfCylinder = capsuleData.numberOfCylinder;
        procap.botOffset = capsuleData.botOffset;



        // Transform ���� �ε� �� ����
        capsule.transform.position = capsuleData.position;
        capsule.transform.eulerAngles = capsuleData.rotation;
        capsule.transform.localScale = capsuleData.scale;

        procap.make();

        return capsule;
    }

    public void GenerateIcon(GameObject prefab, string iconName)
    {

        CreateManager.instance.outline.Hideoutline();
        
        // ���� �ؽ�ó ����
        RenderTexture renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);

        // �ӽ� ī�޶� ����
        GameObject cameraObj = new GameObject("IconCamera");
        Camera iconCamera = cameraObj.AddComponent<Camera>();

        // ī�޶� ����
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = Color.clear;
        iconCamera.orthographic = true;

        //bound ���
        Bounds bounds = GetBounds(prefab);
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        iconCamera.nearClipPlane = size * 0.1f;
        iconCamera.farClipPlane = size * 3f;
        


        // ���� �߰�
        GameObject lightObj = new GameObject("IconLight");
        Light iconLight = lightObj.AddComponent<Light>();
        iconLight.type = LightType.Directional;
        iconLight.intensity = 0.5f;

        // ������ ī�޶� ��ġ�� ��ġ
        lightObj.transform.position = iconCamera.transform.position;
        lightObj.transform.rotation = iconCamera.transform.rotation;
        lightObj.transform.parent = iconCamera.transform;

        // ������ ũ�� ���
        
        
        iconCamera.orthographicSize = size * 0.6f;

        // ī�޶� �����ʿ��� ������ �ٶ󺸵��� ��ġ
        float distance = size * 2.0f;
        iconCamera.transform.position = bounds.center + new Vector3(distance, 0, 0);
        iconCamera.transform.LookAt(bounds.center);
        Debug.Log(iconCamera.transform.position);
        Debug.Log(bounds.center);

       
        




        // ���� Ÿ�� ����
        iconCamera.targetTexture = renderTexture;
        
        iconCamera.Render();

        // ���� �ؽ�ó�� �ؽ�ó�� ��ȯ
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // PNG�� ����
        byte[] bytes = iconTexture.EncodeToPNG();
        string iconPath = iconsPath + iconName + ".png";
        File.WriteAllBytes(iconPath, bytes);

        // �ӽ� ��ü ����
        Destroy(cameraObj);
        Destroy(lightObj);
        renderTexture.Release();

        Debug.Log("�������� ����Ǿ����ϴ�: " + iconPath);
    }

    // ��ü�� ���(�ٿ��) ���
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

