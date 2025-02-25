using UnityEngine;
using UnityEngine.UI;

public class DataContent : MonoBehaviour
{
    public ObjectData prefab;
    Text text;
    Image icon;
    public Camera iconCamera; // �������� ���� ī�޶�
    public RenderTexture renderTexture; // ī�޶��� ���
    public Sprite baseIcon; // UI�� ǥ���� �̹���
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
