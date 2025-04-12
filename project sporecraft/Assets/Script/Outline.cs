using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outline : MonoBehaviour
{
    /*
    Renderer rd;
    Material outline;
    bool isShowing;
    List<Material> materialList = new List<Material>();

    
    // Start is called before the first frame update
    void Start()
    {
        //outline = new Material(Shader.Find("Custom/ScreenSpaceOutline"));
        
        outline = new Material(Shader.Find("Custom/OutlineShader"));
        isShowing = false;
    }

    // Update is called once per frame
   public void ShowOutline(GameObject target)
    {
        if (isShowing)
        {
            Hideoutline();
        }
        isShowing = true;
        
        rd = target.GetComponent<Renderer>();
        if (rd == null)
        {
            rd = target.GetComponentInChildren<Renderer>();
        }

        materialList.Clear();
        materialList.AddRange(rd.sharedMaterials);
        materialList.Add(outline);

        rd.materials = materialList.ToArray();
       
    }

    public void Hideoutline()
    {
        if (!isShowing)
            return;
        if (rd == null)
            return;
        isShowing = false;
        materialList.Clear();
        materialList.AddRange(rd.sharedMaterials);
        materialList.Remove(outline);

        rd.materials = materialList.ToArray();
    }
    */

    public Material outlineMaterial;
    private GameObject outlineObj;
    private bool isShowing = false;
    public string outlineLayerName = "Outline";

    public void Start()
    {
        outlineMaterial = new Material(Shader.Find("Custom/OutlineShader"));
    }

    public void ShowOutline(GameObject target)
    {
        if (isShowing) Hideoutline();
        isShowing = true;

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
        mf.sharedMesh = sourceMF.sharedMesh;

        MeshRenderer sourceMR = target.GetComponent<MeshRenderer>();
        Debug.Log(sourceMR);
        if (sourceMR == null)
        {
            sourceMR = target.GetComponentInChildren<MeshRenderer>();
            if (target != CreateManager.instance.mainBody)
            {
                outlineObj.transform.localPosition = sourceMR.transform.localPosition;
                outlineObj.transform.localRotation = sourceMR.transform.localRotation;
            }
            
        }

        

        MeshRenderer mr = outlineObj.AddComponent<MeshRenderer>();

        Material[] outlineMats = new Material[sourceMR.sharedMaterials.Length];
        for (int i = 0; i < outlineMats.Length; i++)
        {
            outlineMats[i] = outlineMaterial;
        }
        mr.materials = outlineMats;
        

        // ��¦ Ȯ���ؼ� �ܰ����� �ѿ� ��������
        outlineObj.transform.localScale = sourceMR.transform.localScale * 1.02f;
    }

    public void Hideoutline()
    {
        if (!isShowing) return;
        isShowing = false;

        if (outlineObj != null)
            Destroy(outlineObj);
    }



}
