using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public bool isGrounded;
    public bool isSprinting;

    Rigidbody rb;
    MeshCollider mc;
    CapsuleCollider playColl;
    Camera cam;

    World world;

    public float moveSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    
    public float gravity = -9.8f;

    public float playerWidth = 1f;
    public float playerHeight = 2f;


    private Vector3 moveDirection;

    float horizontal;
    float vertical;
    float mousehorizontal;
    private float mousevertical;
    Vector3 velocity;

    float verticalMomentum = 0;
    bool jumpRequest;

    

    void Start()
    {
        world =GameObject.Find("World").GetComponent<World>();
    }

    private void FixedUpdate()
    {
        
        CalculateVelocity();

        // 플레이어의 로컬 좌표계 기준으로 이동 처리


        if (jumpRequest)
            {
                Jump();
            }

            // 카메라 기준 이동 방향 계산
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 inputDir = camForward * -vertical + camRight * -horizontal;
            velocity = Vector3.zero;

            if (inputDir.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);
                float targetY = targetRot.eulerAngles.y;

                Vector3 currentEuler = transform.eulerAngles;
                float smoothedY = Mathf.LerpAngle(currentEuler.y, targetY, 10f * Time.deltaTime);

                transform.eulerAngles = new Vector3(currentEuler.x, smoothedY, currentEuler.z);

                float speed = isSprinting ? sprintSpeed : moveSpeed;
                // 이동은 항상 정면 기준으로만 (transform.up = 회전 후의 전방)
                Vector3 moveDir = transform.up;

                velocity = moveDir * speed * Time.deltaTime;
            }



            transform.Translate(velocity, Space.World);
    }


    private void Update()
    {
        GetPlayerInputs();
    }
    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        if(isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * moveSpeed;

        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }

   
    void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mousehorizontal = Input.GetAxis("Mouse X");
        mousevertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) 
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;


        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }

    private float checkDownSpeed(float downSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
        )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + upSpeed, transform.position.z + playerWidth))
        )
        {
            
            return 0;
        }
        else
        {
            
            return upSpeed;
        }
    }

    public bool front
    {
        get
        {
            if
                (
                 world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                 world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))

                )
                return true;
            else
                return false;
        }
    }

    public bool back
    {
        get
        {
            if
                (
                 world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                 world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))

                )
                return true;
            else
                return false;
        }
    }

    public bool left
    {
        get
        {
            if
                (
                 world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                 world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))

                )
                return true;
            else
                return false;
        }
    }

    public bool right
    {
        get
        {
            if
                (
                 world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                 world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))

                )
                return true;
            else
                return false;
        }
    }

    public void enterplaymode()
    {
        Debug.Log("playmode");
        this.enabled = true;
        GetComponent<MeshCollider>().convex = true;
        

        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        Rigidbody[] childs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody t in childs)
        {
            t.isKinematic = true;
        }
        cam = Camera.main;
    }


}
