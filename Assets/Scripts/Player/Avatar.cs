using UnityEngine;

public class Avatar : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public float gravityForce;

    CharacterController cc;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        Application.targetFrameRate = 60;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
        Gravity();
    }

    void Movement()
    {
        //walk direction
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        //walking
        if(direction.magnitude > 0)
        {
            velocity.x = direction.normalized.x * speed;
            velocity.z = direction.normalized.z * speed;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        //sprinting
        if(Input.GetKey(KeyCode.LeftShift))
        {
            velocity.x = direction.normalized.x * speed * 2;
            velocity.z = direction.normalized.z * speed * 2;
        }
        else
        {

        }

        //jumping
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            velocity.y = jumpHeight;
        }

        cc.Move(velocity * Time.deltaTime);
    }
    void Gravity()
    {
        if(cc.isGrounded && velocity.y < -2f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravityForce * Time.deltaTime;
    }
}
