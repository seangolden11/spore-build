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


                // 부딪힌 물체의 중심을 기준으로 대칭 위치 계산
                Vector3 mirroredPosition = MirrorPositionLocal(pointOnSurface, hit);
                
                    mirroredObject.transform.position = mirroredPosition;

                // 부딪힌 물체의 로컬 공간에서 대칭 회전 계산
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

            //설치할 물체가 2개 인지 하나인지 확인하는 부분
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
        mousePosition.z = 10; // 카메라로부터의 거리 설정

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 물체를 변환된 위치로 이동
        currentObject.transform.position = targetPosition;
        currentObject.transform.rotation = Quaternion.identity;
    }

    Vector3 MirrorPositionLocal(Vector3 position, RaycastHit hit)
    {
        // 월드 좌표를 로컬 좌표로 변환
        Vector3 localPosition = hit.transform.InverseTransformPoint(position);

        // 로컬 좌표에서 X 축을 기준으로 대칭

        localPosition.x = -localPosition.x;


        // 대칭된 로컬 좌표를 다시 월드 좌표로 변환
        Vector3 mirroredPosition = hit.transform.TransformPoint(localPosition);
        return mirroredPosition;
    }

    Quaternion MirrorRotationLocal(Quaternion rotation, RaycastHit hit)
    {
        // 월드 회전을 로컬 회전으로 변환
        Quaternion localRotation = Quaternion.Inverse(hit.transform.rotation) * rotation;

        // X 축을 기준으로 대칭 회전 생성
        Vector3 localEulerAngles = localRotation.eulerAngles;
        localEulerAngles.y = -localEulerAngles.y;
        localEulerAngles.z = -localEulerAngles.z;

        // 로컬 회전을 다시 월드 회전으로 변환
        Quaternion mirroredLocalRotation = Quaternion.Euler(localEulerAngles);
        Quaternion mirroredRotation = hit.transform.rotation * mirroredLocalRotation;

        return mirroredRotation;
    }

    void CheckRotationSimilarity(Quaternion rotationA, Quaternion rotationB)
    {
        // 두 회전 사이의 각도 차이를 계산
        float angleDifference = Quaternion.Angle(rotationA, rotationB);

        // 각도 차이가 임계값 이하라면 함수 호출
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
