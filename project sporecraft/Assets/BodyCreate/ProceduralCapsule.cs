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
    public int numberOfCylinder = 1;

    private List<Vector3> vertices;
    private List<int> triangles;
    MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateCapsuleMesh(subdivisionHeight, subdivisionAround, radius, height);
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Standard"));
    }

    public void AppendCapsule(int liftAmount)
    {
        Mesh mesh = meshFilter.mesh;
        topOffest += liftAmount;

        int topThreshold = ((subdivisionHeight / 2)+1) * (subdivisionAround+1); // 위쪽 반구의 시작점

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
                vertices.Insert(++topThreshold-1, new Vector3(x, y, z));
            }
        }
        numberOfCylinder++;

        List<int> tempTriangles = new List<int>();
        //삼각형 계산
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
        mesh.RecalculateNormals(); // 노멀을 재계산하여 라이팅을 조정
        meshFilter.mesh = mesh; // 메쉬 변경사항 적용
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

        // Bottom Hemisphere
        for (int i = (verticalSubdivisions / 2) + 1; i <= verticalSubdivisions; i++)
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
}
