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
    private void Awake()
    {
        instance = this;
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
}
