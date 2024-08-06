using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject body;
    ProceduralCapsule pC;
    int topOrBottom = 0;
    public float requiredDragDistance = 100.0f; // �巡�� �ؾ��ϴ� �ּ� �Ÿ� (��ũ�� ��ǥ�� ����)
    private Camera mainCamera;
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
        isDragging = true; // �巡�� ����
        Debug.Log("Mouse down on " + gameObject.name);
    }

    private void OnMouseDrag()
    {
        if (mainCamera == null || !isDragging) return;

        Vector3 currentMousePosition = Input.mousePosition;
        float dragDistance = Vector3.Distance(dragStartPosition, currentMousePosition);

        Debug.Log("Current Drag Distance: " + dragDistance);

        // �巡�� �Ÿ��� ������ �Ѿ����� Ȯ��
        if (dragDistance >= requiredDragDistance)
        {
            
            
            pC.AppendCapsule(1);
            
            dragStartPosition = currentMousePosition;
            Debug.Log("Dragged enough on " + gameObject.name);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false; // ���콺 ��ư�� ������ �巡�� ����
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
        mousePoint.z = zCoord; // ������ Z��ǥ�� ���
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private void FixedUpdate()
    {

        if(topOrBottom == 1)
        {
            if(pC.topBone != null)
            {

                transform.position = pC.topBone.transform.position + (2 * pC.topBone.transform.TransformDirection(Vector3.up));
                transform.rotation = pC.topBone.transform.rotation;
            }
        }
        else if (topOrBottom == 2)
        {

        }
    }
}
