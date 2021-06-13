using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;
    public Transform detatchSuit;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float speed = 0.1f;
    public float jumpSpeed = 1.0f;
    public float maxspeed = 1.5f;
    public float stoppingVel = 5;
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
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed * 150 * Time.deltaTime, rb.velocity.z);
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
        
        

        //Time.deltaTime is important for variable framerates (speeds may need reworking?)
        rb.AddForce(moveDirection * 150 * Time.deltaTime, ForceMode.Impulse);

        //This is to re-add fricition after removing with physics material (This is to stop cube bouncing across the ground and sticking to objects it shouldn't)
        if (moveDirection.magnitude == 0 || Input.GetButton("Fire3"))
            rb.AddForce(new Vector3(-rb.velocity.x, 0, -rb.velocity.z) * stoppingVel);


        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        transform.eulerAngles = new Vector2(0, rotation.y);


        //Fire Basic Attack
        if (Input.GetButtonDown("Fire1") && !detatchSuit.GetChild(0).GetComponent<Animator>().GetBool("BasicAttack"))
        {
            OxygenMeter.changeOxygen(-5);
            detatchSuit.position = transform.position;
            detatchSuit.rotation = playerCameraParent.rotation * Quaternion.Euler(-15, 0, 0);
            detatchSuit.rotation = Quaternion.Euler(Mathf.Max(0, detatchSuit.eulerAngles.x), Mathf.Max(0, detatchSuit.eulerAngles.y), detatchSuit.eulerAngles.z);
            detatchSuit.gameObject.SetActive(true);
            detatchSuit.GetChild(0).GetComponent<Animator>().SetBool("BasicAttack", true);
            StartCoroutine(resetAttack());
        }

    }

    IEnumerator resetAttack ()
    {
        yield return new WaitForSeconds(1);
        //GrappleHook
        while (Vector3.Distance(transform.position, detatchSuit.GetChild(0).position) > 3)
        {
            rb.AddForce((detatchSuit.GetChild(0).position - transform.position).normalized * 5, ForceMode.Impulse);
            yield return null;
        }
        detatchSuit.gameObject.SetActive(false);
        detatchSuit.GetChild(0).GetComponent<Animator>().SetBool("BasicAttack", false);
    }
}
