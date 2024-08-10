using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public float radius = 0f;
    public LayerMask layer;
    public Collider[] colliders;
    public Collider short_bone;
    private void Start()
    {
        
    }
    public Transform FindNearestBone()
    {



        short_bone = null;
        colliders = Physics.OverlapSphere(transform.position, radius, layer);
        if(colliders.Length == 0)
        {
            CreateManager.instance.addMode.nullreturn = true;
            
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
            return colliders[0].transform;



        
    }

}
