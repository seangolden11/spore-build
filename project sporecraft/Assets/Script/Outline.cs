using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outline : MonoBehaviour
{

    public Material outlineMaterial;
    private GameObject outlineObj;
    private bool isShowing = false;
    public string outlineLayerName = "Outline";
    GameObject mainBody;

    public void Start()
    {
        outlineMaterial = new Material(Shader.Find("Custom/OutlineShader"));
        

    }

    public void ShowOutline(GameObject target)
    {

        if (isShowing) Hideoutline();
        isShowing = true;


        if (CreateManager.instance != null)
            mainBody = CreateManager.instance.mainBody;
        else if (GameManager.instance != null)
            mainBody = GameManager.instance.mainBody;

        // �ܰ����� ���� ������Ʈ ����
        outlineObj = new GameObject(target.name + "_Outline");
        outlineObj.layer = LayerMask.NameToLayer(outlineLayerName);
        outlineObj.transform.SetParent(target.transform);
        outlineObj.transform.localPosition = Vector3.zero;
        outlineObj.transform.localRotation = Quaternion.identity;
        outlineObj.transform.localScale = Vector3.one;

        MeshFilter sourceMF = target.GetComponent<MeshFilter>();

        if (sourceMF == null)
        {
            sourceMF = target.GetComponentInChildren<MeshFilter>();
        }

        MeshFilter mf = outlineObj.AddComponent<MeshFilter>();

        if (target == mainBody)
        {
            Mesh bakedMesh = new Mesh();
            target.GetComponent<SkinnedMeshRenderer>().BakeMesh(bakedMesh);
            mf.sharedMesh = bakedMesh;
        }
        else
        {
            
            mf.sharedMesh = sourceMF.sharedMesh;
        }
        

        MeshRenderer sourceMR = target.GetComponent<MeshRenderer>();
        
        if (sourceMR == null)
        {
            sourceMR = target.GetComponentInChildren<MeshRenderer>();
            if (target != mainBody)
            {
                
                outlineObj.transform.localPosition = sourceMR.transform.localPosition;
                outlineObj.transform.localRotation = sourceMR.transform.localRotation;
                outlineObj.transform.localScale = sourceMR.transform.localScale * 1.02f;

                MeshRenderer mr = outlineObj.AddComponent<MeshRenderer>();

                Material[] outlineMats = new Material[sourceMR.sharedMaterials.Length];
                for (int i = 0; i < outlineMats.Length; i++)
                {
                    outlineMats[i] = outlineMaterial;
                }
                mr.materials = outlineMats;
            }
            else if (target == mainBody)
            {
                outlineObj.transform.localScale = target.transform.localScale * 1.02f;

                MeshRenderer mr = outlineObj.AddComponent<MeshRenderer>();

                Material[] outlineMats = new Material[target.GetComponent<SkinnedMeshRenderer>().sharedMaterials.Length];
                for (int i = 0; i < outlineMats.Length; i++)
                {
                    outlineMats[i] = outlineMaterial;
                }
                mr.materials = outlineMats;
            }
            
            
        }

        

        
        

        // ��¦ Ȯ���ؼ� �ܰ����� �ѿ� ��������
        
    }

    public void Hideoutline()
    {
        if (!isShowing) return;
        isShowing = false;

        if (outlineObj != null)
            Destroy(outlineObj);
    }



}
