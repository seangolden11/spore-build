using UnityEngine;

public class EditMode : MonoBehaviour
{
    
    private Camera mainCamera;
   
    private Vector3 offset;
    public GameObject currentObject;
    GameObject mirroredObject;
    bool addmode;
    int partid;
    Transform nearbone;
    bool added;
    public float rotationSimilarityThreshold;
    bool isRotationSim;
    bool isEditing;
    public LayerMask bodyPartLayer;
    Outline outline;




    void Start()
    {
        mainCamera = Camera.main;
        isEditing = false;
        outline = CreateManager.instance.outline;
    }

    void Update()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyUp(KeyCode.R))
        {
            isEditing = true;
            if (currentObject.GetComponent<BodyPart>().mirroredObject != null)
                mirroredObject = currentObject.GetComponent<BodyPart>().mirroredObject;
            else
            {
                partid = currentObject.GetComponent<BodyPart>().partData.partId;
                mirroredObject = Instantiate(CreateManager.instance.partManager.Parts[partid]);

            }
            currentObject.GetComponent<Collider>().enabled = false;
            mirroredObject.GetComponent<Collider>().enabled = false;
            mirroredObject.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CreateManager.instance.SwitchClickMode(1);
            this.enabled = false;
        }
        

        if(!isEditing)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(ray, out hit, bodyPartLayer))
                {          
                    currentObject = hit.collider.gameObject;
                    outline.ShowOutline(currentObject);
                    

                }
            }


            return;
        }

        if (Physics.Raycast(ray, out hit))
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

            isEditing = false;
            if (!Physics.Raycast(ray, out hit))
            {




                Destroy(currentObject);
                Destroy(mirroredObject);
                CreateManager.instance.SwitchClickMode(1);
                this.enabled = false;
                return;
            }

            currentObject.GetComponent<BodyPart>().Fussion();

            //��ġ�� ��ü�� 2�� ���� �ϳ����� Ȯ���ϴ� �κ�
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
            
            


             CreateManager.instance.SwitchClickMode(1);
             this.enabled = false;
            
        }


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
