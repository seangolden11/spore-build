using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamer : MonoBehaviour
{
    public Transform playerHead;  // 플레이어 머리 Transform
    public float sensitivity = 100f;  // 마우스 감도
    public Transform eyePos;
    private float xRotation = 0f;

    void Start()
    {
        
        // 시작할 때 마우스 커서를 잠금 상태로 설정
        Cursor.lockState = CursorLockMode.Locked;
        playerHead = CreateManager.instance.mainBody.transform;
        eyePos = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
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

        

        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.y, 0);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,0);
        //transform.localRotation = Quaternion.

        // 수평 회전 처리 (플레이어 몸체와 함께 회전)
        playerHead.Rotate(Vector3.forward * mouseX);



        // Esc를 누르면 마우스 커서가 해제됨
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
