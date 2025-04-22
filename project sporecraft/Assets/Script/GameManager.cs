
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject mainBody;
    public Camera mainCamera;
    public Transform cameraHolder;
    public World world;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectDataManager.instance.LoadCapsule("ww");
        mainCamera = Camera.main;
        mainCamera.GetComponent<PlayCamer>().SetCamera();
        world.player = mainBody.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
