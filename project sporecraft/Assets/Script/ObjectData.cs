using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectData
{
    public string name;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uv;
    public Color color;
    
    public int subdivisionHeight;
    public int subdivisionAround;
    public float radius;
    public float height;
    public float cylinderDivision;
    public float topOffest;
    public int numberOfCylinder;
    public int botOffset;

    //몸체의 transform
    public Vector3 position;
    public Vector3 rotation; // 오일러 각도로 저장
    public Vector3 scale;


    public List<Vector3> listBonesPos;
    public List<Vector3> listBonesRot;

    public List<Vector3> temptransPos;

    

    public List<Vector3> listLocalBones;
    public int materialNum;

}
