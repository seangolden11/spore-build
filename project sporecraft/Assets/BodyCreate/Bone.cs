using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Bone : MonoBehaviour
{
    public GameObject mainbody;
    public int bonenum;
    public float blendvalue;
    public float speed = 100f;
    ProceduralCapsule capsule;
    // Start is called before the first frame update

    private void Start()
    {
        blendvalue = 0f;
        capsule = mainbody.GetComponent<ProceduralCapsule>();
        this.enabled = false;
    }

    private void Update()
    {
        float wheelinput = Input.GetAxis("Mouse ScrollWheel");
        if(wheelinput > 0 && blendvalue < 100)
        {
            blendvalue += wheelinput * speed;
            bonenum = capsule.returnboneint(this.transform);
            capsule.sRenderer.SetBlendShapeWeight(bonenum, blendvalue);
        }
        else if(wheelinput < 0 && blendvalue > 0)
        {
            blendvalue += wheelinput * speed;
            bonenum = capsule.returnboneint(this.transform);
            capsule.sRenderer.SetBlendShapeWeight(bonenum, blendvalue);
        }

       
    }

}
