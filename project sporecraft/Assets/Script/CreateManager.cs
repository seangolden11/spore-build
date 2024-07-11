using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    bool playMode;

    public FreeCameraLook cameraRig;

    List<Transform> PlayerParts = new List<Transform>();

    [SerializeField]
    Transform startingCube;

    [SerializeField]
    GameObject PartToPlacePrefab;
    GameObject partToPlace;

    Transform socketToPlace;

    Vector3 placePos;

    private void Start()
    {
        PlayerParts.Add(startingCube);
    }

    private void Update()
    {
        if (!playMode)
        {
            InstantiatePart();
        }
        else
        {
            if (partToPlace)
            {
                Destroy(partToPlace);
                partToPlace = null;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            cameraRig.enabled = false;
        }
        if (Input.GetMouseButtonUp(2))
        {
            cameraRig.enabled = false;
        }
    }

    void InstantiatePart()
    {
        if (!partToPlace)
        {
            if(PartToPlacePrefab)
                partToPlace = Instantiate(PartToPlacePrefab, -Vector3.up* 2000, Quaternion.identity) as GameObject;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit, Mathf.Infinity))
            {
                CheckHit(hit);
            }

            partToPlace.transform.position = placePos;

            if (Input.GetMouseButtonUp(0))
            {
                SiegePart_base placeBase = partToPlace.GetComponent<SiegePart_base>();

                PlayerParts.Add(partToPlace.transform);
                partToPlace.GetComponentInChildren<Collider>().enabled = true; //콜라이더 활성화

                placeBase.DisableSocket(socketToPlace); //소켓 비활성화

                placeBase.AssignTargetToJoint(socketToPlace.parent);//타겟을 조인트에 추가
                partToPlace.transform.position = placePos; //포지션 정함

                partToPlace = null;
            }
            
        }

        
    }
    void CheckHit(RaycastHit hit)
    {
        if (hit.transform.GetComponent<SiegePart_base>())
        {
            SiegePart_base partbase = hit.transform.GetComponent<SiegePart_base>();

            socketToPlace = partbase.RetrunClosetDirection(hit.point);

            if (socketToPlace)
                placePos = socketToPlace.position;

            partToPlace.transform.LookAt(partbase.rendererToFindEdges.bounds.center);

        }
        else
        {
            placePos = new Vector3(0, -2000, 0);
        }
    }

    public void PassNewPrefabToInstantiate(GameObject prefab)
    {
        if (partToPlace)
        {
            if (PlayerParts.Contains(partToPlace.transform))
                PlayerParts.Remove(partToPlace.transform);

            Destroy(partToPlace);
        }

        PartToPlacePrefab = prefab;
    }

    public void EnablePlayMode()
    {
        for(int i = 0; i < PlayerParts.Count; i++)
        {
            if (PlayerParts[i] != null)
                PlayerParts[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        playMode = true; //플레이모드 전환
    }
}
