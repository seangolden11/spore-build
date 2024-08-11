using UnityEngine;
using UnityEngine.EventSystems;

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
    public GameObject partPanel;

    

    void Start()
    {
        mainCamera = Camera.main;
        PC = MainBody.GetComponent<ProceduralCapsule>();
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
                    PC.Cilcked();
                    isBoneClicked = true;
                    targetObject.GetComponent<Bone>().enabled = true;
                    partPanel.SetActive(true);
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
                    partPanel.SetActive(true);
                    PC.Cilcked();
                    
                    
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
        Debug.Log(targetObject);
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if(targetObject == null)
        {
            
            isbodyclicked = false;
            
            if(isBoneClicked)
                lastObjectCilcked.GetComponent<Bone>().enabled = false;
            isBoneClicked = false;
            PC.otherCilceked();
            partPanel.SetActive(false);
            return;
        }
        if (isBoneClicked && targetObject.layer != BoneLayer)
        {
            lastObjectCilcked.GetComponent<Bone>().enabled = false;
            isBoneClicked = false;
        }
        if (isbodyclicked && targetObject.layer != BodyLayer && targetObject.layer != BoneLayer)
        {
            
                PC.otherCilceked();
                
                isbodyclicked = false;
            
        }
        
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
