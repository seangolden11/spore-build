using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public float radius;
    public LayerMask layer;
    public Collider[] colliders;
    public Collider short_bone;
    public float deltaValue;
    ProceduralCapsule capsule;
    public Vector3 fussionPos;
    
    
    
    private void Start()
    {
        radius = 5;
        capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
        this.GetComponent<Collider>().enabled = false;
        
    }
    public Transform FindNearestBone()
    {



        short_bone = null;
        colliders = Physics.OverlapSphere(transform.position, radius, layer);
        if(colliders.Length == 0)
        {
            
            
            return transform;
        }


        if (colliders.Length > 1)
        {
            float short_distance = Vector3.Distance(transform.position, colliders[0].transform.position);
            short_bone = colliders[0];
            foreach (Collider col in colliders)
            {
                float short_distance2 = Vector3.Distance(transform.position, col.transform.position);
                if (short_distance > short_distance2)
                {
                    short_distance = short_distance2;
                    short_bone = col;
                }
            }
            return short_bone.transform;
        }
        else
        {
            short_bone = colliders[0];
            return short_bone.transform;
        }



        
    }

    public void Fussion()
    {
        if (short_bone == null)
            FindNearestBone();
        transform.parent = short_bone.transform;
        var tuple = short_bone.GetComponent<Bone>().Fussioned(this);
        deltaValue = tuple.dletavalue;
        fussionPos = tuple.tempPos;
        deltaValue = Mathf.Abs(deltaValue);
        this.GetComponent<Collider>().enabled = true;
                    
            

    }

    public void changed(float inputvalue)
    {
        Vector3 nextPos;
        nextPos.x = fussionPos.x + (fussionPos.x * deltaValue) * (inputvalue/100);
        nextPos.y = fussionPos.y;
        nextPos.z = fussionPos.z + (fussionPos.z * deltaValue) * (inputvalue/100);
        transform.localPosition = nextPos;
        
    }

}
