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
    public ObjectDataManager dataManager;
    public GameObject nameinputfield;
    public InputField namefield;
    public ScrollView scrollView;
    string myname;
    void Awake()
    {
        text = GetComponentInChildren<Text>();
        icon = GetComponentsInChildren<Image>()[1];
        dataManager = CreateManager.instance.objectDataManager;
        scrollView = GetComponentInParent<ScrollView>();
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

    public void Savebutton()
    {
        nameinputfield.SetActive(true);
        
    }

    public void Setname()
    {
        myname = namefield.text;
        dataManager.SaveCapsule(myname);
        scrollView.DataInit();
        nameinputfield.SetActive(false);
    }

    public void revertbutton()
    {
        nameinputfield.SetActive(false);
    }

    public void Loadbutton()
    {
        dataManager.LoadGameObject(myname);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
