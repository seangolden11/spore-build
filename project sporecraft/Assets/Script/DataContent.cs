using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public GameObject prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // 아이콘을 찍을 카메라
    public RenderTexture renderTexture; // 카메라의 출력
    public Sprite baseIcon; // UI에 표시할 이미지
    void Awake()
    {
        text = GetComponentInChildren<Text>();
        icon = GetComponentsInChildren<Image>()[1];
    }

    public void Init(GameObject newprefab, Sprite newSprite)
    {
        prefab = newprefab;
        if (prefab != null)
        {
            text.text = prefab.name;
            icon.sprite = newSprite;
        }
        else 
        { 
            text.text = "No Data";
            icon.sprite = baseIcon;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
