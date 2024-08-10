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
    public bool nullreturn;
    

    void Start()
    {
        mainCamera = Camera.main;
        addmode = false;
        added = false;
        nullreturn = false;
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
                // No hit, move freely with mouse
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, offset.z);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            currentObject.transform.position = worldPosition + offset;
        }

        if (Input.GetMouseButtonDown(0))
        {
            nearbone = currentObject.GetComponentInChildren<BodyPart>().FindNearestBone();
            if(nullreturn)
            {
                addmode = false;
                CreateManager.instance.bodyClick.enabled = true;
                added = false;
                nullreturn = false;
                Destroy(currentObject);
                return;
            }
            currentObject.transform.parent = nearbone;
            added = true;
        }

        if (Input.GetMouseButtonUp(0) && added)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                currentObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
                offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
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
        offset = transform.position - mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
        
        addmode = true;
        CreateManager.instance.bodyClick.enabled = false;

    }
}
