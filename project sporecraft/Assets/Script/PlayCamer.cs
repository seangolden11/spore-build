using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayCamer : MonoBehaviour
{
    public Transform playerHead;  // 플레이어 머리 Transform
    public float sensitivity = 100f;  // 마우스 감도
    public Transform eyePos;
    private float xRotation = 0f;

    void Start()
    {
        
        
        
        
        //eyePos = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // 수직 회전 처리 (카메라만 회전)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 회전 각도 제한



        transform.rotation = Quaternion.Euler(xRotation, playerHead.rotation.eulerAngles.y -180, 0);

        // 수평 회전 처리 (플레이어 몸체와 함께 회전)
        playerHead.Rotate(Vector3.forward * mouseX);



        // Esc를 누르면 마우스 커서가 해제됨
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }


    public float heightMultiplier = 1.5f;
    public float distanceMultiplier = 2.5f;
    public float smoothSpeed = 5f;
    Vector3 offset;

    public void SetCamera()
    {
        this.enabled = true;
        if(CreateManager.instance != null)
            playerHead = CreateManager.instance.mainBody.transform;
        else if (GameManager.instance != null)
            playerHead = GameManager.instance.mainBody.transform;

        Cursor.lockState = CursorLockMode.Locked; // 시작할 때 마우스 커서를 잠금 상태로 설정
        // 타겟 기준으로 오프셋 위치 계산
        Vector3 desiredPosition = playerHead.position + playerHead.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // 항상 타겟 바라보기
        transform.LookAt(playerHead);

        transform.parent = playerHead;
    }

    void CalculateOffset()
    {
        // 플레이어의 크기 측정
        Renderer renderer = playerHead.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float height = bounds.size.y;

            // 크기에 따라 offset 자동 조정
            float heightOffset = height * heightMultiplier;
            float backOffset = height * distanceMultiplier;

            offset = new Vector3(0, heightOffset, -backOffset);
        }
        else
        {
            // 기본 fallback
            offset = new Vector3(0, 2, -5);
        }
    }
}
