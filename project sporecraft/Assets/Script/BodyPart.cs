using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public BodyPartData partData;
    public GameObject targetObject;
    int vertexIndex;
    Vector3[] vertices;
    SkinnedMeshRenderer skRender;
    Mesh bakedMesh;
    
    
    private void Start()
    {
        radius = 5;
        capsule = CreateManager.instance.mainBody.GetComponent<ProceduralCapsule>();
        bakedMesh = new Mesh();
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
        targetObject = short_bone.transform.root.gameObject;
        capsule.GetComponent<ProceduralCapsule>().listBodyParts.Add(gameObject);
        skRender = capsule.GetComponent<SkinnedMeshRenderer>();
        
        InitTargetMesh();
        
        this.GetComponent<Collider>().enabled = true;
        /*if(partData.itemType == BodyPartData.ItemType.eye)
        {
            CreateManager.instance.mainBody.GetComponent<EyePos>().AddEyePos(this.transform);
        }*/
                    
            

    }

    public void Changed(Mesh mesh)
    {
        //vertices = targetObject.GetComponent<MeshFilter>().sharedMesh.vertices;

        bakedMesh = mesh;

        InitTargetMesh();

        transform.position = skRender.transform.TransformPoint(vertices[vertexIndex]);
    }

    void InitTargetMesh()
    {
        
        vertices = bakedMesh.vertices;
        vertexIndex = GetClosetIndex();
    }

    int GetClosetIndex()
    {

        int closestIndex = 0;
        float minDistance = 100;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, targetObject.transform.TransformPoint(vertices[i]));
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }


        return closestIndex;
    }
    /*private void OnDestroy()
    {
        if (partData.itemType == BodyPartData.ItemType.eye)
        {
            CreateManager.instance.mainBody.GetComponent<EyePos>().DeleteEyePos(this.transform);
        }
    }
    */

    private void OnDestroy()
    {
        if(capsule != null)
            capsule.GetComponent<ProceduralCapsule>().listBodyParts.Remove(gameObject);
    }
}
