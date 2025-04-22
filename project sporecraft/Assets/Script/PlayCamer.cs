using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayCamer : MonoBehaviour
{
    public Transform playerHead;  // �÷��̾� �Ӹ� Transform
    public float sensitivity = 100f;  // ���콺 ����
    public Transform eyePos;
    private float xRotation = 0f;

    void Start()
    {
        
        
        
        
        //eyePos = CreateManager.instance.mainBody.GetComponent<EyePos>().Eyepos[0];
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



        transform.rotation = Quaternion.Euler(xRotation, playerHead.rotation.eulerAngles.y -180, 0);

        // ���� ȸ�� ó�� (�÷��̾� ��ü�� �Բ� ȸ��)
        playerHead.Rotate(Vector3.forward * mouseX);



        // Esc�� ������ ���콺 Ŀ���� ������
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

        Cursor.lockState = CursorLockMode.Locked; // ������ �� ���콺 Ŀ���� ��� ���·� ����
        // Ÿ�� �������� ������ ��ġ ���
        Vector3 desiredPosition = playerHead.position + playerHead.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // �׻� Ÿ�� �ٶ󺸱�
        transform.LookAt(playerHead);

        transform.parent = playerHead;
    }

    void CalculateOffset()
    {
        // �÷��̾��� ũ�� ����
        Renderer renderer = playerHead.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float height = bounds.size.y;

            // ũ�⿡ ���� offset �ڵ� ����
            float heightOffset = height * heightMultiplier;
            float backOffset = height * distanceMultiplier;

            offset = new Vector3(0, heightOffset, -backOffset);
        }
        else
        {
            // �⺻ fallback
            offset = new Vector3(0, 2, -5);
        }
    }
}
