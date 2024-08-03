using UnityEngine;

public class Dragable : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // 오브젝트를 클릭할 때의 Z좌표를 저장
        zCoord = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;

        // 클릭한 오브젝트와 카메라 간의 오프셋을 계산
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        Vector3 nextPos;
        nextPos = (GetMouseWorldPos() + offset);
        nextPos.x = transform.position.x;
        // 마우스의 위치를 이용해 오브젝트를 이동
        transform.position = nextPos;
    }

    // 마우스의 월드 좌표를 계산하는 함수
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
