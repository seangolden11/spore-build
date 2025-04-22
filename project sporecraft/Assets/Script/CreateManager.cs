using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CreateManager : MonoBehaviour
{
    public static CreateManager instance;
    public PartManager partManager;
    public BodyClick bodyClick;
    public AddMode addMode;
    public EditMode editMode;
    
    public GameObject mainBody;
    public Outline outline;
    public Transform camerholder;
    public ObjectDataManager objectDataManager;
    public GameObject bodybone;
    public GameObject arrow;
    

    

    bool isplaymode;
    private void Awake()
    {
        instance = this;
        isplaymode = false;
    }
    private void Start()
    {
        mainBody.GetComponent<ProceduralCapsule>().make();
    }

    public async void SwitchClickMode(int mode)
    {
        await Task.Delay(500); // 밀리초 단위

        switch (mode)
        {
            case 1:
                bodyClick.enabled = true;
                break;
            case 2:
                editMode.enabled = true;
                break;
            default:
                break;
        }

    }

    

    public void StartPlay()
    {
        if (isplaymode)
            return;
        bodyClick.enabled=false;
        addMode.enabled=false;
        
        //camerholder.transform.parent = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
        
        camerholder.GetComponent<MoveCamera>().ChangeToPlay();
        mainBody.GetComponent<Player>().enabled = true;
        mainBody.GetComponent<Player>().enterplaymode();
        isplaymode=true;
    }
}
