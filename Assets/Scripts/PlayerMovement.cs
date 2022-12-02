using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed, SpeedWalk, WalkAcceleration, SpeedSprint, SpeedGlide, GlideAcceleration, SpeedFlight, SpeedDiving;
    public float SprintAcceleration, SprintDeceleration, AccelerationVelocity;
    public float TurnRadius, turnSmoothVelocity, TurnSpeed, TurnWalk, TurnWalkAcceleration, TurnGlide, TurnGlideAcceleration;
    public float Pitch, PitchSpeed, PitchAcceleration;
    bool isWalking, isSprinting, isSpeedingUp = false;
    public float JumpHeight, JumpAcceleration;
    public float Gravity;
    Vector3 Velocity;

    public Transform GroundCheck;
    public float GroundDistance;
    public LayerMask GroundMask;
    public bool isGrounded;

    public float DistanceToGround;
    public bool isNearGround;
    public bool isFlying = false;
    public float brake;

    public Transform cam;
    public Rigidbody rb;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //Turning Stats
        TurnRadius = 2f;
        TurnWalk = 3f;
        TurnWalkAcceleration = 10f;
        TurnGlide = 2f;
        TurnGlideAcceleration = 2f;

        //Speed Stats
        SpeedWalk = 4f;
        WalkAcceleration = 14f;
        SpeedGlide = 20f;
        GlideAcceleration = 2.5f;
        SpeedSprint = 5f;
        SpeedFlight = 4f;
        SpeedDiving = 1.01f;
        
        SprintAcceleration = 1.5f;
        SprintDeceleration = 1.5f;
        JumpAcceleration = 10f;
        JumpHeight = 5f;

        //Pitch Stats
        Pitch = 1f;

        //Grounded Stats
        GroundDistance = 0.2f;
        DistanceToGround = 1f;
        brake = 0.3f;
        

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //GroundCheck
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        isNearGround = Physics.CheckSphere(GroundCheck.position, DistanceToGround, GroundMask);


        Speed = Mathf.Lerp(Speed, Input.GetAxisRaw("Axis1") * SpeedWalk, WalkAcceleration * Time.deltaTime);
        TurnSpeed = Mathf.Lerp(TurnSpeed, Input.GetAxisRaw("Axis2") * TurnWalk, TurnWalkAcceleration * Time.deltaTime);
        transform.position += (transform.forward * Speed * Time.deltaTime);

        //calculate the character's new angle, convert the result to degrees instead of radians, store it,
        float targetAngle = Mathf.Atan2(TurnSpeed, Speed) * Mathf.Rad2Deg;

        

        //rotate the character said degrees around the y axis,
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        if (Input.GetKeyDown("space") && isGrounded)
        {
            rb.transform.position += (transform.up * JumpHeight * Time.deltaTime);
        }
        if (Speed > 0.1f || TurnSpeed > 0.1f || Speed < -0.1f || TurnSpeed < 0.1f)
        {
            animator.SetBool("isMoving", true);
            isWalking = true;
        }

        else
        {
            animator.SetBool("isMoving", false);
            isWalking = false;
        }

        //if the player is flying
        if (isFlying)
        {
            Speed = SpeedGlide;
            TurnSpeed = Mathf.Lerp(TurnSpeed, Input.GetAxisRaw("Axis2") * TurnGlide, TurnGlideAcceleration * Time.deltaTime);
            PitchSpeed = Mathf.Lerp(PitchSpeed, Input.GetAxisRaw("Axis1") * Pitch, PitchAcceleration * Time.deltaTime);
        }

    }
}
