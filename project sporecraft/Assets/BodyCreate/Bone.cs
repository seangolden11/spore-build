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
    public List<BodyPart> childparts;
    public float wheelinput;
    bool isScrolling;
    float lastScrollTime;
    float scrollThreshold = 0.1f;
    // Start is called before the first frame update

    private void Awake()
    {
        wheelinput = 0;
        childparts = new List<BodyPart>();
        blendvalue = 0f;
        capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
        //Mesh mesh = GetComponentInChildren<MeshFilter>().mesh;
        //MeshNormalAverage(mesh);
        //GetComponentInChildren<MeshFilter>().mesh = mesh;
        this.enabled = false;
    }

    private void OnEnable()
    {
        capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
    }

    private void Update()
    {
        wheelinput = Input.GetAxis("Mouse ScrollWheel");
        if(wheelinput > 0 && blendvalue < 100)
        {
            isScrolling = true;
            lastScrollTime = Time.time;
            blendvalue += wheelinput * speed;
            bonenum = capsule.returnboneint(this.transform);
            capsule.sRenderer.SetBlendShapeWeight(bonenum, blendvalue);
            //capsule.UpdateMeshCollider();
            ChildPartsTrans(blendvalue);
        }
        else if(wheelinput < 0 && blendvalue > 0)
        {
            isScrolling = true;
            lastScrollTime = Time.time;
            blendvalue += wheelinput * speed;
            bonenum = capsule.returnboneint(this.transform);
            capsule.sRenderer.SetBlendShapeWeight(bonenum, blendvalue);
            ChildPartsTrans(blendvalue);
        }

        if (isScrolling && Time.time - lastScrollTime > scrollThreshold)
        {
            isScrolling = false;
            // 마우스 휠 돌리기가 끝났을 때 실행할 동작
            capsule.UpdateMeshCollider();
        }

    }

    public (Vector3 tempPos, float deltaValue) Fussioned(BodyPart temp)
    {
        
        childparts.Add(temp);

        
        bonenum = capsule.returnboneint(this.transform);

        return capsule.GetDeltaValue(temp.gameObject.transform.position, bonenum);
        
        
    }

    void ChildPartsTrans(float inputvalue)
    {
        for(int i = 0; i < childparts.Count; i++)
        {
            childparts[i].changed(inputvalue);
        }
    }

    

}
