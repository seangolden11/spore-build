using System.Collections.Generic;
using UnityEngine;

public class ProceduralCapsule : MonoBehaviour
{
    public int subdivisionHeight = 10;
    public int subdivisionAround = 20;
    public float radius = 1f;
    public float height = 2f;
    public float cylinderDivision = 6f;
    public float topOffest = 0;
    public int numberOfCylinder = 0;
    public int botOffset = 0;

    private List<Vector3> vertices;
    private List<int> triangles;
    MeshFilter meshFilter;
    SkinnedMeshRenderer sRenderer;

    List<Transform> listBones;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateCapsuleMesh(subdivisionHeight, subdivisionAround, radius, height);
        sRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        sRenderer.material = new Material(Shader.Find("Standard"));
        listBones = new List<Transform>();
        CreateBones(0);
    }

    public void AppendCapsule(int liftAmount)
    {
        Mesh mesh = meshFilter.mesh;
        topOffest += liftAmount;

        int topThreshold = ((subdivisionHeight / 2)+1) * (subdivisionAround+1); // ���� �ݱ��� ������

        for (int i = 0; i < topThreshold; i++)
        {
            Vector3 tempVector3 = vertices[i];
            tempVector3.y += liftAmount; // liftAmount ��ŭ y��ǥ ����
            vertices[i] = tempVector3;
        }

        for (int i = 1; i < cylinderDivision + 1; i++)
        {
            float y = Mathf.Lerp((height / 2) + topOffest, ((height / 2) + topOffest) - height, i / (float)cylinderDivision);
            for (int j = 0; j <= subdivisionAround; j++)
            {
                float theta = 2 * Mathf.PI * j / subdivisionAround;
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);
                vertices.Insert(++topThreshold-1, new Vector3(x, y, z));
            }
        }
        numberOfCylinder++;

        List<int> tempTriangles = new List<int>();
        //�ﰢ�� ���
        for (int i = 0; i <= subdivisionHeight + (cylinderDivision * numberOfCylinder); i++)
        {
            for (int j = 0; j < subdivisionAround; j++)
            {
                int current = i * (subdivisionAround + 1) + j;
                int next = current + subdivisionAround + 1;

                tempTriangles.Add(current);
                tempTriangles.Add(current + 1);
                tempTriangles.Add(next);

                tempTriangles.Add(current + 1);
                tempTriangles.Add(next + 1);
                tempTriangles.Add(next);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tempTriangles.ToArray();
        mesh.RecalculateNormals(); // ����� �����Ͽ� �������� ����
        meshFilter.mesh = mesh; // �޽� ������� ����

        CreateBones((int)topOffest);
    }

    private Mesh CreateCapsuleMesh(int verticalSubdivisions, int horizontalSubdivisions, float radius, float height)
    {
        Mesh mesh = new Mesh();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        
        // Top Hemisphere
        for (int i = 0; i <= verticalSubdivisions / 2; i++)
        {
            float phi = Mathf.PI * i / verticalSubdivisions;
            for (int j = 0; j <= horizontalSubdivisions; j++)
            {
                float theta = 2 * Mathf.PI * j / horizontalSubdivisions;
                float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = radius * Mathf.Cos(phi) + height / 2;
                float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                vertices.Add(new Vector3(x, y, z));
                
            }
        }
        

        // Cylinder
        for (int i = 1; i < cylinderDivision + 1; i++)
        {
            float y = Mathf.Lerp(height / 2, -height / 2, i / (float)cylinderDivision);
            for (int j = 0; j <= horizontalSubdivisions; j++)
            {
                float theta = 2 * Mathf.PI * j / horizontalSubdivisions;
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);
                vertices.Add(new Vector3(x, y, z));
                
            }
        }
        
        numberOfCylinder++;

        // Bottom Hemisphere
        for (int i = (verticalSubdivisions / 2); i <= verticalSubdivisions; i++)
        {
            float phi = Mathf.PI * i / verticalSubdivisions;
            for (int j = 0; j <= horizontalSubdivisions; j++)
            {
                float theta = 2 * Mathf.PI * j / horizontalSubdivisions;
                float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = radius * Mathf.Cos(phi) - height / 2;
                float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                vertices.Add(new Vector3(x, y, z));
                
            }
        }
        

        // Triangles
        
        for (int i = 0; i <= verticalSubdivisions + cylinderDivision; i++)
        {
            for (int j = 0; j < horizontalSubdivisions; j++)
            {
                int current = i * (horizontalSubdivisions + 1) + j;
                int next = current + horizontalSubdivisions + 1;

                triangles.Add(current);
                triangles.Add(current + 1);
                triangles.Add(next);

                triangles.Add(current + 1);
                triangles.Add(next + 1);
                triangles.Add(next);
                
            }
        }
        

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); // Automatically recalculates normals
        

        return mesh;
    }


    void CreateBones(int y)
    {
        GameObject newbone = new GameObject("Bone" + numberOfCylinder);

        newbone.transform.localPosition = new Vector3(0, y, 0);

        // �� �迭�� ����
        if(y < topOffest)
            listBones.Add(newbone.transform);
        else
            listBones.Insert(0,newbone.transform);
        
        SetupSkinnedMeshRenderer(listBones.ToArray());
    }


    void SetupSkinnedMeshRenderer(Transform[] bones)
    {
        sRenderer.bones = bones;
        sRenderer.sharedMesh = meshFilter.mesh;

        // ���ε� ���� ����
        Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }
        sRenderer.sharedMesh.bindposes = bindPoses;

        // ����ġ �Ҵ�
        AssignBoneWeights(sRenderer.sharedMesh, bones.Length);
    }

   
    void AssignBoneWeights(Mesh mesh, int numBones)
    {
        BoneWeight[] weights = new BoneWeight[mesh.vertexCount];

        int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1);
        int i;

        // ��� �ݱ��� ����ġ �Ҵ�
        for (i = 0; i < topThreshold; i++)
        {
            weights[i].boneIndex0 = 0; // �ֻ�� ��
            weights[i].weight0 = 1;
        }
        Debug.Log(i);
        // �Ǹ��� �κп� ����ġ �Ҵ�
        for (int j = 0; j < numberOfCylinder; j++)
        {
            int count = (int)(cylinderDivision) * (subdivisionAround + 1);
            for (int k = 0; k < count; k++, i++)
            {
                weights[i].boneIndex0 = j;
                weights[i].weight0 = 1;
            }
        }
        Debug.Log(i);
        // �ϴ� �ݱ��� ����ġ �Ҵ�
        for (int l = 0; l < topThreshold; l++, i++)
        {
            weights[i].boneIndex0 = numberOfCylinder-1;
            weights[i].weight0 = 1;
        }
        Debug.Log(i);
        mesh.boneWeights = weights;
    }



}
