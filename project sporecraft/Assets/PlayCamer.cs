using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamer : MonoBehaviour
{
    public Transform playerHead;  // �÷��̾� �Ӹ� Transform
    public float sensitivity = 100f;  // ���콺 ����
    public Transform eyePos;
    private float xRotation = 0f;

    void Start()
    {
        
        // ������ �� ���콺 Ŀ���� ��� ���·� ����
        Cursor.lockState = CursorLockMode.Locked;
        playerHead = CreateManager.instance.mainBody.transform;
        eyePos = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
    }

    // Update is called once per frame
    void Update()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // ���� ȸ�� ó�� (ī�޶� ȸ��)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // ȸ�� ���� ����

        

        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.y, 0);
        transform.localRotation = Quaternion.Euler(transform.eulerAngles);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,0);
        //transform.localRotation = Quaternion.

        // ���� ȸ�� ó�� (�÷��̾� ��ü�� �Բ� ȸ��)
        playerHead.Rotate(Vector3.forward * mouseX);



        // Esc�� ������ ���콺 Ŀ���� ������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
