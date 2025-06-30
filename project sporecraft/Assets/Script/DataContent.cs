using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public ObjectData prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // 아이콘을 찍을 카메라
    public RenderTexture renderTexture; // 카메라의 출력
    public Sprite baseIcon; // UI에 표시할 이미지
    public ObjectDataManager dataManager;
    public GameObject nameinputfield;
    public InputField namefield;
    public ScrollView scrollView;
    public BodyClick bk;
    string myname;
    void Awake()
    {
        text = GetComponentInChildren<Text>();
        icon = GetComponentsInChildren<Image>()[1];
        dataManager = CreateManager.instance.objectDataManager;
        scrollView = GetComponentInParent<ScrollView>();
        if (CreateManager.instance != null)
        {
            bk = CreateManager.instance.bodyClick;
        }
        else if (GameManager.instance != null)
        {
            
        }
    }

    public void Init(ObjectData prefabData, Sprite newSprite)
    {
        prefab = prefabData;
        if (prefab != null)
        {
            text.text = prefab.name;
            myname = prefab.name;
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
        bk.ResetClick();
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
        bk.ResetClick();
        dataManager.LoadCapsule(myname);
    }

    public void DeleteButton()
    {
        if (myname == null)
            return;
        dataManager.DeleteCapsule(myname);
        scrollView.DataInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
