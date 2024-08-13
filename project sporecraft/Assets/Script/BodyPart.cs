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
    Vector3 fussionPos;
    
    
    private void Start()
    {
        radius = 5;
        capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
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
        fussionPos = transform.position;
        List<Transform> listbones = capsule.listBones;
        List<Vector3> listlocalbones = capsule.listLocalBones;
           
            
         Vector3 vertex = transform.localPosition;

           
               
               
         List<KeyValuePair<int, float>> blendWeights = new List<KeyValuePair<int, float>>();

         for (int l = 0; l < listbones.Count; l++)
         {
                        float distance = Vector3.Distance(listlocalbones[l], vertex) - 1;
                        distance = Mathf.Exp(-distance * 1);
                        //distance = Mathf.Max(0, 1 - distance / 2);
                        blendWeights.Add(new KeyValuePair<int, float>(l, distance)); // 거리가 가까울수록 높은 가중치
         }

                    blendWeights.Sort((x, y) => y.Value.CompareTo(x.Value));

                    float totalWeight = 0.0f;
                    for (int l = 0; l < Mathf.Min(3, blendWeights.Count); l++)
                    {
                        totalWeight += blendWeights[l].Value;
                    }

                    


                       
           deltaValue = (blendWeights[0].Value / totalWeight);
                    
            

    }

    public void changed(float inputvalue)
    {
        //transform.position = vertextrans.position;
    }

}
