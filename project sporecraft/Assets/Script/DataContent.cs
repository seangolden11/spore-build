using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public GameObject prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // 아이콘을 찍을 카메라
    public RenderTexture renderTexture; // 카메라의 출력
    public RawImage uiIcon; // UI에 표시할 이미지
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
        // 프리팹 인스턴스 생성
        GameObject instance = Instantiate(prefab, iconCamera.transform.position + Vector3.forward * 2, Quaternion.identity);

        // 카메라 렌더링
        iconCamera.targetTexture = renderTexture;
        iconCamera.Render();

        // RenderTexture를 UI에서 사용할 수 있도록 적용
        Texture2D iconTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        iconTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        iconTexture.Apply();
        RenderTexture.active = null;

        // UI 아이콘으로 적용
        uiIcon.texture = iconTexture;

        // 사용이 끝나면 오브젝트 제거
        Destroy(instance);
    }
}
