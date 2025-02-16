using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public GameObject prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // �������� ���� ī�޶�
    public RenderTexture renderTexture; // ī�޶��� ���
    public Sprite baseIcon; // UI�� ǥ���� �̹���
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
