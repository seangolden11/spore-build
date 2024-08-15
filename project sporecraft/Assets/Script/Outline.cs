using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    Renderer rd;
    Material outline;
    bool isShowing;
    List<Material> materialList = new List<Material>();

    
    // Start is called before the first frame update
    void Start()
    {
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
        isShowing = false;
        materialList.Clear();
        materialList.AddRange(rd.sharedMaterials);
        materialList.Remove(outline);

        rd.materials = materialList.ToArray();
    }

   
}
