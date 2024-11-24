using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectData
{
    public int subdivisionHeight;
    public int subdivisionAround;
    public float radius;
    public float height;
    public float cylinderDivision;
    public float topOffset;
    public int numberOfCylinder;
    public int botOffset;
    public GameObject bone;
    public Vector3 position;
    public Quaternion rotation;

    public List<Transform> tempTrans;
    public List<Transform> listBones;
    public List<Vector3> listLocalBones;



    // 필요한 경우 자식 오브젝트의 정보도 추가 가능
    public string objectName;

    public ObjectData(ProceduralCapsule capsule)
    {
        /*
        subdivisionHeight = capsule.subdivisionHeight;
        subdivisionAround = capsule.subdivisionAround;
        radius = capsule.radius;
        height = capsule.height;
        cylinderDivision = capsule.cylinderDivision;
        topOffset = capsule.topOffest;
        numberOfCylinder = capsule.numberOfCylinder;
        botOffset = capsule.botOffset;
        objectName = capsule.name;
        bone = capsule.bone;
        tempTrans = capsule.tempTrans;
        listBones = capsule.listBones;
        listLocalBones = capsule.listLocalBones;
        position = capsule.transform.position;
        rotation = capsule.transform.rotation;
        */

        
    }
}
