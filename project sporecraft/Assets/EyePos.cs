using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePos : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Transform> Eyepos;
   public void AddEyePos(Transform pos)
    {
        Eyepos.Add(pos);
    }

    public void DeleteEyePos(Transform pos)
    {
        Eyepos.Remove(pos);
    }
}
