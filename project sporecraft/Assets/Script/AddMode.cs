using UnityEngine;

public class AddMode : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    GameObject currentObject;
    GameObject mirroredObject;
    bool addmode;
    int partid;
    Transform nearbone;
    bool added;
    public float rotationSimilarityThreshold;
    bool isRotationSim;
    bool again;




    void Start()
    {
        mainCamera = Camera.main;
        
        added = false;
        again = false;
        isRotationSim = false;
        //this.enabled = false;
        
    }

    void Update()
    {
        
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) )
        {
            
                // Raycast hit something, attach object to surface
            Vector3 pointOnSurface = hit.point + hit.normal * 0.001f; // Small offset to prevent Z-fighting
            currentObject.transform.position = pointOnSurface;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            currentObject.transform.rotation = targetRotation;

            mirroredObject.SetActive(true);


            // �ε��� ��ü�� �߽��� �������� ��Ī ��ġ ���
            Vector3 mirroredPosition = MirrorPositionLocal(pointOnSurface, hit);
            mirroredObject.transform.position = mirroredPosition;

            // �ε��� ��ü�� ���� �������� ��Ī ȸ�� ���
            Quaternion mirroredRotation = MirrorRotationLocal(targetRotation, hit);
            mirroredObject.transform.rotation = mirroredRotation;

            CheckRotationSimilarity(currentObject.transform.rotation, mirroredObject.transform.rotation);
        

        }
        else
        {
            mirroredObject.SetActive(false);
            FollowMouse();
            
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(ray, out hit))
            {
                
                
                
                
                Destroy(currentObject);
                Destroy(mirroredObject);
                CreateManager.instance.bodyClick.enabled = true;
                this.enabled = false;
                return;
            }

            currentObject.GetComponent<BodyPart>().Fussion();
            

            if (!isRotationSim)
            {

                mirroredObject.GetComponent<BodyPart>().Fussion();
                mirroredObject.GetComponent<BodyPart>().mirroredObject = currentObject;
                currentObject.GetComponent<BodyPart>().mirroredObject = mirroredObject;

            }
            else
            {
                Destroy(mirroredObject);
                currentObject.GetComponent<BodyPart>().mirroredObject = null;
            }

            

            if (Input.GetKey(KeyCode.LeftControl))
            {
                
                    currentObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
                    mirroredObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);

            }
            else
            {

                
                CreateManager.instance.SwitchClickMode(1);
                this.enabled = false;
            }
        }
        
        
    }


  

    public void ActiveAddMode(BodyPartData partData)
    {
        partid = partData.partId;
        currentObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
        mirroredObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);
        mirroredObject.SetActive(false);
        CreateManager.instance.boneCamera.enabled = false;

        CreateManager.instance.bodyClick.enabled = false;
        
        this.enabled = true;
        

    }

    void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10; // ī�޶�κ����� �Ÿ� ����

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // ��ü�� ��ȯ�� ��ġ�� �̵�
        currentObject.transform.position = targetPosition;
        currentObject.transform.rotation = Quaternion.identity;
    }

    Vector3 MirrorPositionLocal(Vector3 position, RaycastHit hit)
    {
        // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 localPosition = hit.transform.InverseTransformPoint(position);

        // ���� ��ǥ���� X ���� �������� ��Ī
        
        localPosition.x = -localPosition.x;


        // ��Ī�� ���� ��ǥ�� �ٽ� ���� ��ǥ�� ��ȯ
        Vector3 mirroredPosition = hit.transform.TransformPoint(localPosition);
        return mirroredPosition;
    }

    Quaternion MirrorRotationLocal(Quaternion rotation, RaycastHit hit)
    {
        // ���� ȸ���� ���� ȸ������ ��ȯ
        Quaternion localRotation = Quaternion.Inverse(hit.transform.rotation) * rotation;

        // X ���� �������� ��Ī ȸ�� ����
        Vector3 localEulerAngles = localRotation.eulerAngles;
        localEulerAngles.y = -localEulerAngles.y;
        localEulerAngles.z = -localEulerAngles.z;

        // ���� ȸ���� �ٽ� ���� ȸ������ ��ȯ
        Quaternion mirroredLocalRotation = Quaternion.Euler(localEulerAngles);
        Quaternion mirroredRotation = hit.transform.rotation * mirroredLocalRotation;

        return mirroredRotation;
    }

    void CheckRotationSimilarity(Quaternion rotationA, Quaternion rotationB)
    {
        // �� ȸ�� ������ ���� ���̸� ���
        float angleDifference = Quaternion.Angle(rotationA, rotationB);

        // ���� ���̰� �Ӱ谪 ���϶�� �Լ� ȣ��
        if (angleDifference <= rotationSimilarityThreshold)
        {
            isRotationSim = true;
            mirroredObject.SetActive(false);
        }
        else
        {
            isRotationSim = false;
        }
    }

}
