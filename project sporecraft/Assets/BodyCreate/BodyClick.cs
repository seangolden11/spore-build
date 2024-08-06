using UnityEngine;

public class BodyClick : MonoBehaviour
{
    public LayerMask BodyLayer;
    public LayerMask BoneLayer;
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;
    private GameObject targetObject;
    public GameObject MainBody;
    ProceduralCapsule PC;
    bool isBoneClicked;

    void Start()
    {
        mainCamera = Camera.main;
        PC = MainBody.GetComponent<ProceduralCapsule>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, BoneLayer | BodyLayer);

            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & BoneLayer) != 0)
                {
                    isBoneClicked = true;
                    targetObject = hit.collider.gameObject;

                    zCoord = mainCamera.WorldToScreenPoint(targetObject.transform.position).z;
                    offset = targetObject.transform.position - GetMouseWorldPos();
                    return;
                }
            }

            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & BodyLayer) != 0)
                {
                    targetObject = hit.collider.gameObject;
                    zCoord = mainCamera.WorldToScreenPoint(targetObject.transform.position).z;
                    offset = targetObject.transform.position - GetMouseWorldPos();
                    
                    return;
                }
            }
        }

        if (Input.GetMouseButton(0) && targetObject != null)
        {
            Vector3 nextPos = GetMouseWorldPos() + offset;
            nextPos.x = targetObject.transform.position.x;
            targetObject.transform.position = nextPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            targetObject = null;
            if (isBoneClicked)
            {
                PC.UpdateMeshCollider();
                isBoneClicked = false;
            }

        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
