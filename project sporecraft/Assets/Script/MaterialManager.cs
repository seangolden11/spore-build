using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager instance;
    public List<Material> materials;
    public List<GameObject> prefabs;

    private void Awake()
    {
        instance = this;
    }

}
