using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    public static CreateManager instance;
    public PartManager partManager;
    public BodyClick bodyClick;
    public AddMode addMode;
    public Camera boneCamera;
    public GameObject mainBody;
    public Outline outline;
    public Transform camerholder;
    public ObjectDataManager objectDataManager;
    

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

    public void SwitchClickMode()
    {
        StartCoroutine(SwitchAfterDelay());
        
    }

    IEnumerator SwitchAfterDelay()
    {
        // 1√  ¥Î±‚
        yield return new WaitForSeconds(0.5f);

        if (bodyClick.enabled)
        {
            bodyClick.enabled = false;
        }
        else
        {
            bodyClick.enabled = true;
        }
    }

    public void StartPlay()
    {
        if (isplaymode)
            return;
        bodyClick.enabled=false;
        addMode.enabled=false;
        boneCamera.enabled=false;
        camerholder.transform.parent = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
        camerholder.transform.localPosition = Vector3.zero;
        camerholder.GetComponent<MoveCamera>().ChangeToPlay();
        mainBody.GetComponent<Player>().enabled = true;
        mainBody.GetComponent<Player>().enterplaymode();
        isplaymode=true;
    }
}
