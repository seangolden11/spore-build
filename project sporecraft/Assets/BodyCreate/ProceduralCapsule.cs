using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.WSA;

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
    public GameObject topArrow;
    public GameObject bottomArrow;

    private List<Vector3> vertices;
    
    private List<int> triangles;
    MeshFilter meshFilter;
    public SkinnedMeshRenderer sRenderer;
    public GameObject topBone;
    public GameObject bottomBone;
    int weightrange = 4;
    MeshCollider mc;
    Mesh bakedMesh;
    int threshold;
    Arrow toparrowSc;
    Arrow botArrowSc;


    public List<Transform> tempTrans;
    public List<Transform> listBones;
    public List<Vector3> listLocalBones;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateCapsuleMesh(subdivisionHeight, subdivisionAround, radius, height);
        sRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        sRenderer.material = new Material(Shader.Find("Standard"));
        mc = GetComponent<MeshCollider>();
        bakedMesh = new Mesh();
        UpdateMeshCollider();
        listBones = new List<Transform>();
        listLocalBones = new List<Vector3>();
        CreateBones(1,Vector3.zero);
        
        toparrowSc = topArrow.GetComponent<Arrow>();
        botArrowSc = bottomArrow.GetComponent<Arrow>();
        topArrow.SetActive(false);
        bottomArrow.SetActive(false);



    }

    public void UpdateMeshCollider()
    {
        
        sRenderer.BakeMesh(bakedMesh);
        mc.sharedMesh = bakedMesh;
        
    }

    public void AppendCapsule(int mode)
    {
        int liftAmount = 2;
        Mesh mesh = meshFilter.mesh;
        threshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1);
        List<int> tempTriangles = new List<int>();



        if (mode == 1)
        {
            topOffest += liftAmount;
            int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1); // 위쪽 반구의 시작점

            for (int i = 0; i < topThreshold; i++)
            {
                Vector3 tempVector3 = vertices[i];
                tempVector3.y += liftAmount; // liftAmount 만큼 y좌표 증가
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
                    vertices.Insert(++topThreshold - 1, new Vector3(x, y, z));
                }
            }

            numberOfCylinder++;


            //삼각형 계산
            tempTriangles = CalculateTriangles(tempTriangles);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = tempTriangles.ToArray();
            mesh.RecalculateNormals(); // 노멀을 재계산하여 라이팅을 조정
            meshFilter.mesh = mesh; // 메쉬 변경사항 적용

            Vector3 newVector = Vector3.zero;
            newVector.y += topOffest;
            
            CreateBones(1, newVector);
        }

        else if(mode == 2 && numberOfCylinder > 1)
        {
            topOffest -= liftAmount;
            int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1); // 위쪽 반구의 시작점

            for (int i = 0; i < topThreshold; i++)
            {
                Vector3 tempVector3 = vertices[i];
                tempVector3.y -= liftAmount; // liftAmount 만큼 y좌표 감소
                vertices[i] = tempVector3;
            }

            for (int i = 1; i < cylinderDivision + 1; i++)
            {
                
                for (int j = 0; j <= subdivisionAround; j++)
                {
                    vertices.RemoveAt(topThreshold);
                    
                }
            }

            numberOfCylinder--;


            //삼각형 계산
            tempTriangles = CalculateTriangles(tempTriangles);
            mesh.triangles = tempTriangles.ToArray();
            mesh.vertices = vertices.ToArray();
            
            mesh.RecalculateNormals(); // 노멀을 재계산하여 라이팅을 조정
            meshFilter.mesh = mesh; // 메쉬 변경사항 적용

            
            RemoveBones(2);
        }

        else if (mode == 3)
        {
            botOffset -= liftAmount;
            int botThreshold = mesh.vertexCount - ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1); // 위쪽 반구의 시작점

            for (int i = botThreshold; i < mesh.vertexCount; i++)
            {
                Vector3 tempVector3 = vertices[i];
                tempVector3.y -= liftAmount; // liftAmount 만큼 y좌표 감소
                vertices[i] = tempVector3;
            }

            for (int i = 1; i < cylinderDivision + 1; i++)
            {
                float y = Mathf.Lerp((height / 2) + botOffset, ((height / 2) + botOffset) - height, i / (float)cylinderDivision);
                for (int j = 0; j <= subdivisionAround; j++)
                {
                    float theta = 2 * Mathf.PI * j / subdivisionAround;
                    float x = radius * Mathf.Cos(theta);
                    float z = radius * Mathf.Sin(theta);
                    vertices.Insert(++botThreshold - 1, new Vector3(x, y, z));
                }
            }

            numberOfCylinder++;


            //삼각형 계산
            tempTriangles = CalculateTriangles(tempTriangles);


            mesh.vertices = vertices.ToArray();
            mesh.triangles = tempTriangles.ToArray();
            mesh.RecalculateNormals(); // 노멀을 재계산하여 라이팅을 조정
            meshFilter.mesh = mesh; // 메쉬 변경사항 적용

            Vector3 newVector = Vector3.zero;
            newVector.y += botOffset;
            CreateBones(3, newVector);
        }

        else if (mode == 4 && numberOfCylinder > 1)
        {
            botOffset += liftAmount;
            int botThreshold = mesh.vertexCount - ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1); // 위쪽 반구의 시작점
            

            for (int i = botThreshold; i < mesh.vertexCount; i++)
            {
                Vector3 tempVector3 = vertices[i];
                tempVector3.y += liftAmount; // liftAmount 만큼 y좌표 감소
                vertices[i] = tempVector3;
            }

            for (int i = 1; i < cylinderDivision + 1; i++)
            {

                for (int j = 0; j <= subdivisionAround; j++)
                {
                    vertices.RemoveAt(botThreshold - threshold);

                }
            }

            numberOfCylinder--;


            //삼각형 계산
            tempTriangles = CalculateTriangles(tempTriangles);
            mesh.triangles = tempTriangles.ToArray();
            mesh.vertices = vertices.ToArray();

            mesh.RecalculateNormals(); // 노멀을 재계산하여 라이팅을 조정
            meshFilter.mesh = mesh; // 메쉬 변경사항 적용


            RemoveBones(4);
        }
    }

    List<int> CalculateTriangles(List<int> tempTriangles)
    {
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

        return tempTriangles;
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

    void RemoveBones(int mode)
    {
        int index =0;
        if (mode == 2)
        {
            Destroy(topBone.gameObject);
            topBone = listBones[1].gameObject;
            Destroy(topBone.GetComponent<HingeJoint>());
            index = 0;
        }
        else if(mode == 4)
        {
            Destroy(bottomBone.gameObject);
            bottomBone = listBones[listBones.Count-2].gameObject;
            index =  listBones.Count-1;

        }




        listBones.RemoveAt(index);
        listLocalBones.RemoveAt(index);
        Destroy(tempTrans[index].gameObject);
        tempTrans.RemoveAt(index);

        SetupSkinnedMeshRenderer(listBones.ToArray());
    }

    void CreateBones(int mode,Vector3 bonePos)
    {
        if (mode == 1 || mode == 3)
        {
            GameObject newBone = new GameObject("Bone" + numberOfCylinder); //본추가
            GameObject newBoneMesh = new GameObject("BoneMesh" + numberOfCylinder);
            GameObject ts = new GameObject("bonepoint" + numberOfCylinder);

            newBoneMesh.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            newBoneMesh.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            //newBoneMesh.GetComponent<MeshRenderer>();


            CapsuleCollider newBoneCollider = newBone.AddComponent<CapsuleCollider>();
            newBoneCollider.isTrigger = true;

            newBone.layer = LayerMask.NameToLayer("Bone Layer");
            newBoneMesh.layer = LayerMask.NameToLayer("Bone Mesh Layer");


            // Rigidbody 추가
            Rigidbody rb = newBone.AddComponent<Rigidbody>();
            rb.drag = float.PositiveInfinity;
            rb.useGravity = false;
            rb.isKinematic = false;
            //rb.constraints |= RigidbodyConstraints.FreezePositionX;
            //rb.constraints |= RigidbodyConstraints.FreezeRotationX;


            newBone.transform.parent = sRenderer.rootBone; //부모설정
            ts.transform.parent = transform;

            newBone.transform.localPosition = bonePos;
            ts.transform.localPosition = bonePos;
            newBone.transform.rotation = transform.rotation;
            ts.transform.rotation = transform.rotation;


            newBoneMesh.transform.parent = newBone.transform;
            newBoneMesh.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            newBoneMesh.transform.localPosition = Vector3.zero;
            newBoneMesh.transform.rotation = transform.rotation;

            Bone  thenewbone= newBone.AddComponent<Bone>();
            thenewbone.mainbody = this.gameObject;
            

            

            if (mode == 3)
            {
                listBones.Add(newBone.transform);
                listLocalBones.Add(newBone.transform.localPosition);
                tempTrans.Add(ts.transform);
                if (listBones.Count > 1)
                {
                    AddHingeJoint(newBone);

                    newBone.transform.position = bottomBone.transform.position + (2 * bottomBone.transform.TransformDirection(Vector3.down));
                    newBone.transform.rotation = bottomBone.transform.rotation;
                    newBone.GetComponent<HingeJoint>().connectedBody = bottomBone.GetComponent<Rigidbody>();

                    bottomBone = newBone;
                }
                else
                {
                    topBone = newBone;
                    bottomBone = newBone;
                }
            }
            else if (mode == 1)
            {
                
                listBones.Insert(0, newBone.transform);
                tempTrans.Insert(0, ts.transform);
                listLocalBones.Insert(0, newBone.transform.localPosition);

                if (listBones.Count > 1)
                {
                    AddHingeJoint(topBone);
                    

                    newBone.transform.position =  topBone.transform.position + (2*topBone.transform.TransformDirection(Vector3.up));
                    newBone.transform.rotation = topBone.transform.rotation;
                    topBone.GetComponent<HingeJoint>().connectedBody = rb;
                    topBone = newBone;
                     

                }
                else
                {
                    topBone = newBone;
                    bottomBone = newBone;

                }



            }
           

        }




        
        SetupSkinnedMeshRenderer(listBones.ToArray());
    }



    void SetupSkinnedMeshRenderer(Transform[] bones)
    {
        


        sRenderer.bones = bones;
        sRenderer.sharedMesh = meshFilter.mesh;
        // 바인드 포즈 설정
        Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];
        
        for (int i = 0; i < bones.Length; i++)
        {
            
            bindPoses[i] = tempTrans[i].worldToLocalMatrix * transform.localToWorldMatrix;
            
        }
        sRenderer.sharedMesh.bindposes = bindPoses;

       

        // 가중치 할당
        //AssignBoneWeights(sRenderer.sharedMesh, bones.Length);
        AssignBoneWeights(sRenderer.sharedMesh, sRenderer.bones);
    }


    /*void AssignBoneWeights(Mesh mesh, Transform[] bones)
    {
        BoneWeight[] weights = new BoneWeight[mesh.vertexCount];

        int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1);
        int i;

        // 상단 반구에 가중치 할당
        for (i = 0; i < topThreshold; i++)
        {
            weights[i].boneIndex0 = 0; // 최상단 본
            weights[i].weight0 = 1;
        }
        Debug.Log(i);
        int count = (int)(cylinderDivision) * (subdivisionAround + 1);
        int firstcount = (int)(cylinderDivision -1) * (subdivisionAround + 1);
        // 실린더 부분에 가중치 할당
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

                    // 각 본에 대해 거리를 계산하고 가중치를 할당
                    for (int l = 0; l < bones.Length; l++)
                    {
                        float distance = Vector3.Distance(bones[l].position, vertex) - radius;
                        boneWeights.Add(new KeyValuePair<int,float>(l,distance)); // 거리가 가까울수록 높은 가중치
                    }

                    boneWeights.Sort((x, y) => x.Value.CompareTo(y.Value));

                    // 가중치 정규화
                    float totalWeight = 0.0f;
                    for(int l=0;l< Mathf.Min(weightrange,boneWeights.Count);l++)
                    {
                        totalWeight += boneWeights[l].Value;
                    }
                    for (int l = 0; l <= subdivisionAround; l++,i++)
                    {
                        // BoneWeight 설정
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

        // 하단 반구에 가중치 할당
        for (int l =0; l < topThreshold; l++,i++)
        {
            weights[i].boneIndex0 = numberOfCylinder-1;
            weights[i].weight0 = 1;
        }
        Debug.Log(i);

        mesh.boneWeights = weights;
    }*/

    void AddHingeJoint(GameObject newBone)
    {
        // HingeJoint 추가
        HingeJoint hinge = newBone.AddComponent<HingeJoint>();
        hinge.axis = Vector3.zero;  // 회전 축 설정
        hinge.useLimits = true;  // 회전 제한 사용 설정
        JointLimits limits = new JointLimits();
        limits.min = -30.0f;
        limits.max = 30.0f;
        hinge.limits = limits;
        //hinge.anchor = Vector3.zero + new Vector3(0,0,-height/2);
    }

    void AssignBoneWeights(Mesh mesh, Transform[] bones)
    {
        BoneWeight[] weights = new BoneWeight[mesh.vertexCount];

        int topThreshold = ((subdivisionHeight / 2) + 1) * (subdivisionAround + 1);
        int i;

        AddBlendShape();
        for (i = 0; i < mesh.vertexCount;i++)
        {

           
                List<KeyValuePair<int, float>> boneWeights = new List<KeyValuePair<int, float>>();
                Vector3 vertex = vertices[i];

                // 각 본에 대해 거리를 계산하고 가중치를 할당
                for (int l = 0; l < bones.Length; l++)
                {
                    float distance = Vector3.Distance(listLocalBones[l], vertex) - 1;
                    distance = Mathf.Exp(-distance * 1);
                    //distance = Mathf.Max(0.00001f, distance);
                    boneWeights.Add(new KeyValuePair<int, float>(l, distance)); // 거리가 가까울수록 높은 가중치
                }

                boneWeights.Sort((x, y) => y.Value.CompareTo(x.Value));

                // 가중치 정규화
                float totalWeight = 0.0f;
                for (int l = 0; l < Mathf.Min(weightrange, boneWeights.Count); l++)
                {
                    totalWeight += boneWeights[l].Value;
                }
                
                    // BoneWeight 설정
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
                    if (bones.Length > 3)
                    {
                        weights[i].boneIndex3 = boneWeights[3].Key;
                        weights[i].weight3 = boneWeights[3].Value / totalWeight;
                
            }






        }



        mesh.boneWeights = weights;
        
        UpdateMeshCollider();

       
        
    }

    void AddBlendShape()
    {
        sRenderer.sharedMesh.ClearBlendShapes();
        Vector3[] deltaVertices = new Vector3[vertices.Count];
        Vector3 vertex;

        for (int j = 0; j < listBones.Count; j++) {
            deltaVertices = new Vector3[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                vertex = vertices[i];
                List<KeyValuePair<int, float>> blendWeights = new List<KeyValuePair<int, float>>();

                for (int l = 0; l < listBones.Count; l++)
                {
                    float distance = Vector3.Distance(listLocalBones[l], vertex) - 1;
                    distance = Mathf.Exp(-distance * 1);
                    //distance = Mathf.Max(0, 1 - distance / 2);
                    blendWeights.Add(new KeyValuePair<int, float>(l, distance)); // 거리가 가까울수록 높은 가중치
                }

                blendWeights.Sort((x, y) => y.Value.CompareTo(x.Value));

                float totalWeight = 0.0f;
                for (int l = 0; l < Mathf.Min(weightrange, blendWeights.Count); l++)
                {
                    totalWeight += blendWeights[l].Value;
                }

                if (blendWeights[0].Key == j)
                {
                    

                    deltaVertices[i] = vertex;
                    deltaVertices[i].x *= (blendWeights[0].Value/totalWeight);
                    deltaVertices[i].y = 0;
                    deltaVertices[i].z *= (blendWeights[0].Value / totalWeight);
                }
                else if(numberOfCylinder > 1)
                {

                    
                    for (int z=1;z<numberOfCylinder;z++)
                    {
                        if (blendWeights[z].Key == j)
                        {
                            deltaVertices[i] = vertex;
                            deltaVertices[i].x *= (blendWeights[z].Value / totalWeight);
                            deltaVertices[i].y = 0;
                            deltaVertices[i].z *= (blendWeights[z].Value / totalWeight);
                            break;
                        }
                    }
                }
               

                else
                {
                    deltaVertices[i] = Vector3.zero;
                }
            }
            sRenderer.sharedMesh.AddBlendShapeFrame("blend" + j, 100, deltaVertices, null, null);
        }

        for(int i = 0; i < numberOfCylinder; i++)
        {
            sRenderer.SetBlendShapeWeight(i, 0);
        }
        

        sRenderer.sharedMesh.RecalculateNormals();
        sRenderer.sharedMesh.RecalculateTangents();
    }

    public void Cilcked()
    {
        topArrow.SetActive(true);
        bottomArrow.SetActive(true);
    }

    public void otherCilceked()
    {
        if (toparrowSc.isclicked || botArrowSc.isclicked)
            return;
        bottomArrow.SetActive(false);
        topArrow.SetActive(false);
    }



    public int returnboneint(Transform trans)
    {
        int num = listBones.FindIndex(trans.Equals);
        return num;
    }

    }
