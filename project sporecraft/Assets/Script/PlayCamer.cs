using UnityEngine;

public class PlayCamer : MonoBehaviour
{
    public Transform player;
    public float distance = 3.0f;             // 고정 거리 (반지름)
    public float heightOffset = 1.5f;         // 중심 높이 보정 (머리쪽 보기)

    public float mouseSensitivity = 2.0f;
    public float minPitch = -30f, maxPitch = 60f;

    private float yaw = 0f;    // 좌우 회전
    private float pitch = 15f; // 위아래 회전

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

        // 회전 계산
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // 오프셋 (뒤로 distance만큼)
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 카메라 위치 = 플레이어 위치 + 회전된 오프셋 + 높이 보정
        Vector3 targetPos = player.position + Vector3.up * heightOffset;
        transform.position = targetPos + offset;

        // 항상 플레이어 바라보기
        transform.LookAt(targetPos);
    }

    public void SetCamera()
    {
        if (CreateManager.instance != null)
            player = CreateManager.instance.mainBody.transform;
        else if (GameManager.instance != null)
            player = GameManager.instance.mainBody.transform;

        Cursor.lockState = CursorLockMode.Locked;

        // 초기 yaw 보정 (예: 플레이어가 x축 -90도 회전했다면)
        yaw = -90f;

        this.enabled = true;
    }
}
