using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public GameObject prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // �������� ���� ī�޶�
    public RenderTexture renderTexture; // ī�޶��� ���
    public RawImage uiIcon; // UI�� ǥ���� �̹���
    void Awake()
    {
        text = GetComponentInChildren<Text>();
        icon = GetComponentsInChildren<Image>()[1];
    }

    public void Init(GameObject newprefab)
    {
        prefab = newprefab;
        if (prefab != null)
        {
            text.text = prefab.name;
            
        }
        else 
        { 
            text.text = "No Data";
            //icon.sprite = ;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void GenerateIcon(GameObject prefab)
    {
        // ������ �ν��Ͻ� ����
        GameObject instance = Instantiate(prefab, iconCamera.transform.position + Vector3.forward * 2, Quaternion.identity);

        // ī�޶� ������
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // RenderTexture�� UI���� ����� �� �ֵ��� ����
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // UI ���������� ����
        uiIcon.texture = iconTexture;

        // ����� ������ ������Ʈ ����
        Destroy(instance);
    }
}
