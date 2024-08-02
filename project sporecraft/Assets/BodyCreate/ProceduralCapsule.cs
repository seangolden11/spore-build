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
    GameObject topBone;
    GameObject bottomBone;
    int weightrange = 3;
    

    List<Transform> listBones;
    List<Vector3> listLocalBones;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateCapsuleMesh(subdivisionHeight, subdivisionAround, radius, height);
        sRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        sRenderer.material = new Material(Shader.Find("Standard"));
        listBones = new List<Transform>();
        listLocalBones = new List<Vector3>();
        CreateBones(0,Vector3.zero);
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
        
        for (int i = 1; i < cylinderDivision+1; i++)
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
        for (int i = 0; i < subdivisionHeight + (cylinderDivision * numberOfCylinder); i++)
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

        Vector3 newVector = Vector3.zero;
        newVector.y += topOffest;
        CreateBones(0, newVector);
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
        for (int i = 1; i < cylinderDivision; i++)
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
                float y = radius * Mathf.Cos(phi) - (height / 2);
                float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                vertices.Add(new Vector3(x, y, z));
                
            }
        }


        // Triangles
        
        for (int i = 0; i < verticalSubdivisions + cylinderDivision; i++)
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


    void CreateBones(int mode,Vector3 bonePos)
    {
        GameObject newBone = new GameObject("Bone" + numberOfCylinder); //���߰�
        

        // Rigidbody �߰�
        Rigidbody rb = newBone.AddComponent<Rigidbody>();
        rb.isKinematic = true;  // ���� ����� �޵�, �ڵ����� ���������� �ʰ� ����

        newBone.transform.parent = transform; //�θ���

        // HingeJoint �߰�
        HingeJoint hinge = newBone.AddComponent<HingeJoint>();
        hinge.axis = Vector3.up;  // ȸ�� �� ����
        hinge.useLimits = true;  // ȸ�� ���� ��� ����
        JointLimits limits = new JointLimits();
        limits.min = -90.0f;
        limits.max = 90.0f;
        hinge.limits = limits;

        
        newBone.transform.localPosition = bonePos;

        // �� �迭�� ����
        if (mode == 2)
        {
            listBones.Add(newBone.transform);
            listLocalBones.Add(newBone.transform.localPosition);
        }
        else
        {
            listBones.Insert(0, newBone.transform);
            listLocalBones.Insert(0, newBone.transform.localPosition);
            if (listBones.Count > 1)
            {
                topBone.GetComponent<HingeJoint>().connectedBody = rb;
            }
            else
            {
                topBone = newBone;
                bottomBone = newBone;
            }
        }


        


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
        //AssignBoneWeights(sRenderer.sharedMesh, bones.Length);
        AssignBoneWeights(sRenderer.sharedMesh, sRenderer.bones);
    }


    /*void AssignBoneWeights(Mesh mesh, Transform[] bones)
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
        int count = (int)(cylinderDivision) * (subdivisionAround + 1);
        int firstcount = (int)(cylinderDivision -1) * (subdivisionAround + 1);
        // �Ǹ��� �κп� ����ġ �Ҵ�
        if (numberOfCylinder == 1)
        {
            for (int k = 0; k < firstcount; k++, i++)
            {

                weights[i].boneIndex0 = 0;
                weights[i].weight0 = 1;


            }
        }
        if (numberOfCylinder > 1)
        {
            

            for (int k = 0; k < firstcount; k++, i++)
            {

                weights[i].boneIndex0 = 0;
                weights[i].weight0 = 1;


            }
            for (int j = 1; j < numberOfCylinder; j++)
            {

                for (int k = 0; k < cylinderDivision; k++)
                {
                    List<KeyValuePair<int, float>> boneWeights = new List<KeyValuePair<int, float>>();
                    Vector3 vertex = vertices[i];

                    // �� ���� ���� �Ÿ��� ����ϰ� ����ġ�� �Ҵ�
                    for (int l = 0; l < bones.Length; l++)
                    {
                        float distance = Vector3.Distance(bones[l].position, vertex) - radius;
                        boneWeights.Add(new KeyValuePair<int,float>(l,distance)); // �Ÿ��� �������� ���� ����ġ
                    }

                    boneWeights.Sort((x, y) => x.Value.CompareTo(y.Value));

                    // ����ġ ����ȭ
                    float totalWeight = 0.0f;
                    for(int l=0;l< Mathf.Min(weightrange,boneWeights.Count);l++)
                    {
                        totalWeight += boneWeights[l].Value;
                    }
                    for (int l = 0; l <= subdivisionAround; l++,i++)
                    {
                        // BoneWeight ����
                        weights[i].boneIndex0 = boneWeights[0].Key;
                        weights[i].weight0 = boneWeights[0].Value / totalWeight;
                        if (bones.Length > 1)
                        {
                            weights[i].boneIndex1 = boneWeights[1].Key;
                            weights[i].weight1 = boneWeights[1].Value / totalWeight;
                        }
                        if (bones.Length > 2)
                        {
                            weights[i].boneIndex2 = boneWeights[2].Key;
                            weights[i].weight2 = boneWeights[2].Value / totalWeight;
                        }
                        Debug.Log(boneWeights[0].Value/totalWeight);
                        Debug.Log(boneWeights[1].Value / totalWeight);
                    }
                    
                }


            }
        }
        Debug.Log(i);

        // �ϴ� �ݱ��� ����ġ �Ҵ�
        for (int l =0; l < topThreshold; l++,i++)
        {
            weights[i].boneIndex0 = numberOfCylinder-1;
            weights[i].weight0 = 1;
        }
        Debug.Log(i);

        mesh.boneWeights = weights;
    }*/

    void AssignBoneWeights(Mesh mesh, Transform[] bones)
    {
        BoneWeight[] weights = new BoneWeight[mesh.vertexCount];

        int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1);
        int i;


        for (i = 0; i < mesh.vertexCount;i++)
        {

           
                List<KeyValuePair<int, float>> boneWeights = new List<KeyValuePair<int, float>>();
                Vector3 vertex = vertices[i];

                // �� ���� ���� �Ÿ��� ����ϰ� ����ġ�� �Ҵ�
                for (int l = 0; l < bones.Length; l++)
                {
                    float distance = Vector3.Distance(listLocalBones[l], vertex);
                    distance = Mathf.Max(0.00001f, distance);
                    boneWeights.Add(new KeyValuePair<int, float>(l, 1/distance)); // �Ÿ��� �������� ���� ����ġ
                }

                boneWeights.Sort((x, y) => y.Value.CompareTo(x.Value));

                // ����ġ ����ȭ
                float totalWeight = 0.0f;
                for (int l = 0; l < Mathf.Min(weightrange, boneWeights.Count); l++)
                {
                    totalWeight += boneWeights[l].Value;
                }
                
                    // BoneWeight ����
                    weights[i].boneIndex0 = boneWeights[0].Key;
                    weights[i].weight0 = boneWeights[0].Value / totalWeight;
                    if (bones.Length > 1)
                    {
                        weights[i].boneIndex1 = boneWeights[1].Key;
                        weights[i].weight1 = boneWeights[1].Value / totalWeight;
                    }
                    if (bones.Length > 2)
                    {
                        weights[i].boneIndex2 = boneWeights[2].Key;
                        weights[i].weight2 = boneWeights[2].Value / totalWeight;
                    }







        }



        mesh.boneWeights = weights;
    }

    }
