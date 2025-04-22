using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    MeshCollider mc;
    CapsuleCollider playColl;

    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public bool isGrounded = true;

    private Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // 플레이어의 로컬 좌표계 기준으로 이동 처리
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = (-transform.right * moveHorizontal) + transform.up * moveVertical;
        moveDirection.Normalize();  // 방향 벡터를 정규화

        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        // 점프 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        isGrounded = false;

        float targetHeight = transform.position.y + jumpHeight;

        // 상승
        while (transform.position.y < targetHeight)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // 착지
        while (!isGrounded)
        {
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("ground");
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void enterplaymode()
    {
        GetComponent<MeshCollider>().convex = true;
        //playColl = GetComponent<CapsuleCollider>();
        //playColl.enabled = true;

        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        Rigidbody[] childs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody t in childs)
        {
            t.isKinematic = true;
        }
    }


}
