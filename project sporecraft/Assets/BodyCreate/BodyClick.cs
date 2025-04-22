using UnityEngine;
using UnityEngine.EventSystems;

public class BodyClick : MonoBehaviour
{
    public LayerMask BodyLayer;
    public LayerMask BoneLayer;
    public LayerMask ArrowLayer;
    public LayerMask PartLayer;
    
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;
    public GameObject targetObject;
    public GameObject lastObjectCilcked;
    public GameObject MainBody;
    public ProceduralCapsule PC;
    bool isBoneClicked;
    bool isbodyclicked;
    public bool isArrowClicked;
    public GameObject partPanel;
    bool isbuttondown;
    Outline outline;

    

    void Start()
    {
        mainCamera = Camera.main;
        PC = MainBody.GetComponent<ProceduralCapsule>();
        outline = CreateManager.instance.outline;
        isbuttondown = false;
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            ResetClick();
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (isbuttondown)
                return;
            if (PC == null)
            {
                MainBody = CreateManager.instance.mainBody;
                PC = MainBody.GetComponent<ProceduralCapsule>();

            }
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, BoneLayer | BodyLayer | PartLayer);

            isbuttondown = true;

            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & BoneLayer) != 0)
                {
                    
                    targetObject = hit.collider.gameObject;

                    zCoord = mainCamera.WorldToScreenPoint(targetObject.transform.position).z;
                    offset = targetObject.transform.position - GetMouseWorldPos();
                    
                    ClickOther();
                    PC.BoneCilceked();
                    isBoneClicked = true;
                    targetObject.GetComponent<Bone>().enabled = true;
                    partPanel.SetActive(true);
                    outline.ShowOutline(targetObject);
                    
                    return;
                }
            }

            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & PartLayer) != 0)
                {
                    targetObject = hit.collider.gameObject;


                    ClickOther();


                    partPanel.SetActive(true);
                    outline.ShowOutline(targetObject);
                    CreateManager.instance.editMode.currentObject = targetObject;
                    CreateManager.instance.SwitchClickMode(2);
                    targetObject = null;
                    isbuttondown = false;
                    this.enabled = false;
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
                    outline.ShowOutline(targetObject);
                    
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
            if(!isbuttondown)
                return;
            lastObjectCilcked = targetObject;
            
            targetObject = null;
            if (isBoneClicked)
            {
                PC.UpdateMeshCollider();
                
            }
            isbuttondown = false;
        }
    }

    public void ClickOther()
    {
        
        Debug.Log(targetObject);
        Debug.Log(lastObjectCilcked);
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
            outline.Hideoutline();
            return;
        }
        if (isBoneClicked && (targetObject.layer != BoneLayer))
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

    public void ResetClick()
    {
        isbodyclicked = false;

        if (isBoneClicked)
            lastObjectCilcked.GetComponent<Bone>().enabled = false;
        isBoneClicked = false;
        PC.otherCilceked();
        partPanel.SetActive(false);
        outline.Hideoutline();
        return;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 저장한 Z좌표를 사용
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
