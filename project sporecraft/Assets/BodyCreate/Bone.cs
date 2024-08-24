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

    public (Vector3 tempPos,float dletavalue) Fussioned(BodyPart temp)
    {
        
        childparts.Add(temp);
        
        bonenum = capsule.returnboneint(this.transform);
        
        return capsule.GetDeltaValue(temp.transform.localPosition, bonenum);
    }

    void ChildPartsTrans(float inputvalue)
    {
        for(int i = 0; i < childparts.Count; i++)
        {
            childparts[i].changed(inputvalue);
        }
    }

    public void MeshNormalAverage(Mesh mesh)
    {
        Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();

        #region bulid the map of vertex and triangles relation
        for (int v = 0; v < mesh.vertexCount; v++)
        {
            if (!map.ContainsKey(mesh.vertices[v]))
            {
                map.Add(mesh.vertices[v], new List<int>());
            }
            map[mesh.vertices[v]].Add(v);
        }
        #endregion

        Vector3[] normals = mesh.normals;
        Vector3 normal;

        #region the same vertex use the same normal(average)
        foreach (var p in map)
        {
            normal = Vector3.zero;

            foreach (var n in p.Value)
            {
                normal += mesh.normals[n];
            }

            normal /= p.Value.Count;

            foreach (var n in p.Value)
            {
                normals[n] = normal;
            }
        }
        #endregion

        mesh.normals = normals;

    }

}
