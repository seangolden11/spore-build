using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    public static CreateManager instance;
    public PartManager partManager;
    public BodyClick bodyClick;
    public AddMode addMode;
    public Camera boneCamera;
    private void Awake()
    {
        instance = this;
    }
}
