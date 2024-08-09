using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject body;
    ProceduralCapsule pC;
    int topOrBottom = 0;
    public float requiredDragDistance = 100.0f; // 드래그 해야하는 최소 거리 (스크린 좌표계 기준)
    private Camera mainCamera;
    public Transform cameraTrans;
    private float zCoord;

    private Vector3 dragStartPosition;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure your main camera is tagged as MainCamera.");
        }
        else
        {
            Debug.Log("Main camera initialized successfully.");
        }

        pC = body.GetComponent<ProceduralCapsule>();

        if (transform.name == "TopArrow")
        {
            topOrBottom = 1;
        }
        else if (transform.name == "BottomArrow")
        {
            topOrBottom = 2;
        }
    }

    private void OnMouseDown()
    {
        if (mainCamera == null) return;

        zCoord = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
        dragStartPosition = Input.mousePosition;
        isDragging = true; // 드래그 시작
        Debug.Log("Mouse down on " + gameObject.name);
    }

    private void OnMouseDrag()
    {
        if (mainCamera == null || !isDragging) return;

        Vector3 currentMousePosition = Input.mousePosition;
        float dragDistance = Vector3.Distance(dragStartPosition, currentMousePosition);

        //Debug.Log("Current Drag Distance: " + dragDistance);

        // 드래그 거리가 기준을 넘었는지 확인
        if (dragDistance >= requiredDragDistance)
        {
            Vector3 dragDir = dragStartPosition - currentMousePosition;
            
            if(Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
            {
                if (cameraTrans.rotation.eulerAngles.y > 180)
                {
                    if (topOrBottom == 1)
                    {
                        if (dragDir.x > 0)
                        {
                            pC.AppendCapsule(1);
                            
                        }
                        else
                        {
                            pC.AppendCapsule(2);

                        }
                    }
                    else if (topOrBottom == 2)
                    {
                        if (dragDir.x < 0)
                        {
                            pC.AppendCapsule(3);
                        }
                        else
                        {
                            pC.AppendCapsule(4);
                        }
                    }
                }
                else
                {
                    if (topOrBottom == 1)
                    {
                        if (dragDir.x > 0)
                        {
                            pC.AppendCapsule(2);
                        }
                        else
                        {
                            pC.AppendCapsule(1);
                            

                        }
                    }
                    else if (topOrBottom == 2)
                    {
                        if (dragDir.x < 0)
                        {
                            pC.AppendCapsule(4);
                        }
                        else
                        {
                            pC.AppendCapsule(3);
                        }
                    }
                }
            }
            
            
            
            dragStartPosition = currentMousePosition;
            Debug.Log("Dragged enough on " + gameObject.name);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false; // 마우스 버튼을 놓으면 드래그 종료
        Debug.Log("Mouse up on " + gameObject.name);
    }

    public void SetActiveTrue()
    {
        gameObject.SetActive(true);
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private Vector3 GetMouseWorldPos()
    {
        if (mainCamera == null) return Vector3.zero;

        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void FixedUpdate()
    {

        if(topOrBottom == 1)
        {
            if(pC.topBone != null)
            {

                transform.position = pC.topBone.transform.position + (3 * pC.topBone.transform.TransformDirection(Vector3.up));
                transform.rotation = pC.topBone.transform.rotation;
            }
        }
        else if (topOrBottom == 2)
        {
            if (pC.bottomBone != null)
            {

                transform.position = pC.bottomBone.transform.position + (3 * pC.bottomBone.transform.TransformDirection(Vector3.down));
                transform.rotation = pC.bottomBone.transform.rotation * Quaternion.Euler(0,0,180);
            }
        }
    }
}
