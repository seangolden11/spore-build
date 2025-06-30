using UnityEngine;

public class PlayCamer : MonoBehaviour
{
    public Transform player;
    public float distance = 3.0f;             // ���� �Ÿ� (������)
    public float heightOffset = 1.5f;         // �߽� ���� ���� (�Ӹ��� ����)

    public float mouseSensitivity = 2.0f;
    public float minPitch = -30f, maxPitch = 60f;

    private float yaw = 0f;    // �¿� ȸ��
    private float pitch = 15f; // ���Ʒ� ȸ��

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // ȸ�� ���
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // ������ (�ڷ� distance��ŭ)
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // ī�޶� ��ġ = �÷��̾� ��ġ + ȸ���� ������ + ���� ����
        Vector3 targetPos = player.position + Vector3.up * heightOffset;
        transform.position = targetPos + offset;

        // �׻� �÷��̾� �ٶ󺸱�
        transform.LookAt(targetPos);
    }

    public void SetCamera()
    {
        if (CreateManager.instance != null)
            player = CreateManager.instance.mainBody.transform;
        else if (GameManager.instance != null)
            player = GameManager.instance.mainBody.transform;

        Cursor.lockState = CursorLockMode.Locked;

        // �ʱ� yaw ���� (��: �÷��̾ x�� -90�� ȸ���ߴٸ�)
        yaw = -90f;

        this.enabled = true;
    }
}
