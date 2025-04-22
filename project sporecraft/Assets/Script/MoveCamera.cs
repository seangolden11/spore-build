using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float movementSpeed = 10.0f; // 카메라 이동 속도
    public float lookSpeed = 2.0f; // 마우스 회전 속도
    public float sprintMultiplier = 2.0f; // Shift를 누를 때 이동 속도 배수
    public bool isclicked =false;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    List<Vector3> ElementPos;

    void Start()
    {
        ElementPos = new List<Vector3>();
        this.GetComponent<PlayCamer>().enabled = false;
        // 시작할 때 마우스 커서를 잠금 상태로 설정
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (!isclicked) {
            isclicked = true;
            }
            else
                isclicked = false;
            
        }

        if (!isclicked)
        {
            // 마우스 이동에 따른 카메라 회전
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f); // pitch 회전을 -90도에서 90도 사이로 제한

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        // 카메라 이동
        float currentSpeed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        Vector3 forwardMovement = transform.forward * Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        Vector3 strafeMovement = transform.right * Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;

        transform.position += forwardMovement + strafeMovement;

        // 상승/하강 (Q와 E 키로)
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        }

        // Esc를 누르면 마우스 커서가 해제됨
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ChangeToPlay()
    {
        /*
        Transform[] childTrans = GetComponentsInChildren<Transform>(); 
        foreach (Transform t in childTrans) {
            ElementPos.Add(t.localPosition);
            t.localPosition = Vector3.zero;
        }
        this.transform.rotation = Quaternion.identity;
        this.transform.Rotate(0, 90, 0);
        */
        this.GetComponent<PlayCamer>().SetCamera();

        this.enabled = false;

    }
}
