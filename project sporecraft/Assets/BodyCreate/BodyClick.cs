using UnityEngine;

public class BodyClick : MonoBehaviour
{
    public LayerMask BodyLayer;
    public LayerMask BoneLayer;
    public LayerMask ArrowLayer;
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;
    private GameObject targetObject;
    public GameObject lastObjectCilcked;
    public GameObject MainBody;
    ProceduralCapsule PC;
    bool isBoneClicked;
    bool isbodyclicked;
    public bool isArrowClicked;

    void Start()
    {
        mainCamera = Camera.main;
        PC = MainBody.GetComponent<ProceduralCapsule>();
        targetObject = MainBody;
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
                    
                    targetObject = hit.collider.gameObject;

                    zCoord = mainCamera.WorldToScreenPoint(targetObject.transform.position).z;
                    offset = targetObject.transform.position - GetMouseWorldPos();
                    
                    ClickOther();
                    isBoneClicked = true;
                    targetObject.GetComponent<Bone>().enabled = true;
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
                    
                    ClickOther();
                    isbodyclicked = true;
                    PC.Cilcked();
                    
                    
                    return;
                }
            }
            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & ArrowLayer) != 0)
                {
                   
                    isArrowClicked = true;
                    

                    return;
                }
            }
            ClickOther();
            

        }

        if (Input.GetMouseButton(0) && targetObject != null)
        {
            Vector3 nextPos = GetMouseWorldPos() + offset;
            nextPos.x = targetObject.transform.position.x;
            targetObject.transform.position = nextPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            
            lastObjectCilcked = targetObject;
            
            targetObject = null;
            if (isBoneClicked)
            {
                PC.UpdateMeshCollider();
                
            }

        }
    }

    void ClickOther()
    {
        if (isBoneClicked && targetObject.layer != BoneLayer)
        {
            lastObjectCilcked.GetComponent<Bone>().enabled = false;
            isBoneClicked = false;
        }
        if (isbodyclicked && targetObject.layer != BodyLayer)
        {
            if (!isArrowClicked)
            {
                PC.otherCilceked();
                isbodyclicked = false;
            }
        }
        if (isArrowClicked && targetObject.layer != ArrowLayer)
        {
            isArrowClicked = false;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
