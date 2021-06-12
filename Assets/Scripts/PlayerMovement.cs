using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float speed = 0.1f;
    public float jumpSpeed = 1.0f;
    public float maxspeed = 1.5f;
    Vector3 moveDirection = Vector3.zero;
    private Rigidbody rb;
    public Vector3 Ground_check_position;
    public bool Jetpack = false;


    [HideInInspector]
    public bool canMove = true;
    public bool grounded = true;


    // Start is called before the first frame update
    void Start()
    {
          rb = GetComponent<Rigidbody>();
          rotation.y = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = false;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical"):0;
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal"):0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        
        moveDirection = Vector3.ClampMagnitude(moveDirection, maxspeed);


        Collider[] colliders = Physics.OverlapBox(transform.position+Ground_check_position, Vector3.one / 2);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {

                grounded = true;

                if (!grounded)
                    //first landing
                    i = colliders.Length;
            }
        }

        if (grounded)
        {
            if (Input.GetButton("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x,jumpSpeed, rb.velocity.z);
            }
        }
        else if (Jetpack)
        {
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = speed;
            }

            if (Input.GetButton("Fire3"))
            {
                moveDirection.y = -speed;
            }
        }
        
        


        rb.AddForce(moveDirection, ForceMode.Impulse);




        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        transform.eulerAngles = new Vector2(0, rotation.y);
    }
}
