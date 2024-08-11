using UnityEngine;

public class AddMode : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    GameObject currentObject;
    bool addmode;
    int partid;
    Transform nearbone;
    bool added;
    
    

    void Start()
    {
        mainCamera = Camera.main;
        addmode = false;
        added = false;
        
        //this.enabled = false;
        
    }

    void Update()
    {
        if (!addmode)
            return;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
                // Raycast hit something, attach object to surface
            Vector3 pointOnSurface = hit.point + hit.normal * 0.001f; // Small offset to prevent Z-fighting
            currentObject.transform.position = pointOnSurface;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            currentObject.transform.rotation = targetRotation;
        }
        else
        {
            FollowMouse();
            
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(ray, out hit))
            {
                addmode = false;
                CreateManager.instance.bodyClick.enabled = true;
                added = false;
                
                Destroy(currentObject);
                return;
            }

            nearbone = currentObject.GetComponentInChildren<BodyPart>().FindNearestBone();
            
            currentObject.transform.parent = nearbone;
            added = true;
        }

        if (Input.GetMouseButtonUp(0) && added)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                currentObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
                
            }
            else
            {

                addmode = false;
                CreateManager.instance.bodyClick.enabled = true;
            }
            added = false;
        }
        
    }


  

    public void ActiveAddMode(BodyPartData partData)
    {
        partid = partData.partId;
        currentObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
        CreateManager.instance.boneCamera.enabled = false;
        

        addmode = true;
        CreateManager.instance.bodyClick.enabled = false;

    }

    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10; // 카메라로부터의 거리 설정

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 물체를 변환된 위치로 이동
        currentObject.transform.position = targetPosition;
        currentObject.transform.rotation = Quaternion.identity;
    }


}
