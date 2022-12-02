using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour

   
{

    public CharacterController controller;
    public Transform cam;

    public float Speed;
    public float SpeedFlight;
    public float SpeedWalk;
    public float SpeedSprint;
    public float SpeedDiving;
    public float SpeedGliding;
    bool isSprinting = false;
    bool isSpeedingUp = false;
    bool isFlyingUp = false;
    public float SprintAcceleration;
    public float SprintDeceleration;
    public float AccelerationVelocity;
    public float JumpHeight;
    public float keepFlyingSpeed;
    public float currentVelocity;


    public float TurnRadius;
    public float TurnRadiusWalk;
    public float TurnRadiusSprint;
    public float TurnRadiusGlide;
    public float TurnRadiusFlight;
    float turnSmoothVelocity;

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
    public float lift = -20f;

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
    //Turn Radius Stats
        TurnRadius = 0.2f;
        TurnRadiusWalk = 0.8f;
        TurnRadiusSprint = 1.2f;
        TurnRadiusGlide = 1f;
        TurnRadiusFlight = 1.3f;

        Speed = 3f;
        SpeedWalk = 3f;
        SpeedSprint = 5f;
        SpeedFlight = 4f;
        SpeedDiving = 1.01f;
        SpeedGliding = 4f;
        SprintAcceleration = 1.5f;
        SprintDeceleration = 1.5f;
        keepFlyingSpeed = 50f;
        JumpHeight = 2f;

        //Grounded Stats
        GroundDistance = 0.2f;
        DistanceToGround = 1f;
        brake = 0.3f;
        Gravity = -9.81f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // store input as variable; for example a value of -1 horizontally if "A" is pressed
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // store direction
         Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //GroundCheck
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        isNearGround = Physics.CheckSphere(GroundCheck.position, DistanceToGround, GroundMask);

        

        //Reset velocity after touching the ground
        if (isGrounded && Velocity.y < 0)
        { Velocity.y = -2f; }

        if (Input.GetKey(KeyCode.Space) & !isNearGround & !isGrounded)
        {
            isFlying = true;
            animator.SetBool("isFlying", true);

        }


        //if the player is flying
        if (isFlying)
        {

            //calculate the character's new angle, convert the result to degrees instead of radians, store it,
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnRadius);

            //rotate the character said degrees around the y axis,
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //store new direction
            Vector3 moveDir2 = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 direction2 = new Vector3(0f, horizontal, vertical).normalized;

            //and move the character in that direction multiplied by the speed and time (to make it frame-rate-independent).
            controller.Move(direction2.normalized * Speed * Time.deltaTime);



            Velocity.y = 0f;
            TurnRadius = TurnRadiusGlide;



            /*if (Input.GetKey(KeyCode.W))
            {
                if(!isFlyingUp)
                {
                    Velocity.y = 2f;
                    isFlyingUp = true;
                }
                
                
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                Debug.Log("Bluigi is flying up");
                Velocity.y = 0f;
                Speed = SpeedGliding;
            }
            */

            if (Input.GetKey(KeyCode.S))
            {
                Debug.Log("Diving");
                if (Speed <= 20)
                {
                    Speed *= SpeedDiving;
                }
                Velocity.y = -Speed * 0.5f + 0.01f;

            }
            
            //if shift is pressed
            if (Input.GetKey(KeyCode.LeftShift))

            {  //and if the player is not speeding up already
                if (!isSpeedingUp)
                {
                    // add speed to the current speed
                    Speed += SpeedFlight;
                    isSpeedingUp = true;

                    { TurnRadius = TurnRadiusSprint; }

                }
            }

            else

            {
                Speed = SpeedGliding;
                isSpeedingUp = false;
                { TurnRadius = TurnRadiusGlide; }

                
            }

            //and ctrl is pressed
            if (Input.GetKey(KeyCode.LeftControl) && Speed >= 0 + brake)
            {
                //and subtract the braking speed from the current speed
                Speed -= brake;
                Velocity.y = 0f;
            }

            if (Speed < 0.3f)
            {
                animator.SetBool("Speedis0", true);
            }
            else
            {
                animator.SetBool("Speedis0", false);
            }

        }

        else
        {
            TurnRadius = TurnRadiusWalk;

            //if shift is pressed
            if (Input.GetKey(KeyCode.LeftShift))

            {  //and if the player is not sprinting already
                if (!isSprinting)
                {
                    //and add the sprinting speed to the current speed
                    Speed += SpeedSprint;
                    isSprinting = true;

                    { TurnRadius = TurnRadiusSprint; }


                }

            }
        }





            // if shift is not pressed (or not pressed anymore)
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                //set the speed to the walking speed
                Speed = SpeedWalk;
                isSprinting = false;
            }
            Mathf.SmoothDamp(SpeedWalk, SpeedSprint, ref AccelerationVelocity, SprintAcceleration);



            if (direction.magnitude >= 0.1f || isFlying)

            {   //calculate the character's new angle, convert the result to degrees instead of radians, store it,
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

                //smooth turning
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnRadius);

                //rotate the character said degrees around the y axis,
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //store new direction
                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;


                //and move the character in that direction multiplied by the speed and time (to make it frame-rate-independent).
                controller.Move(moveDir.normalized * Speed * Time.deltaTime);

                animator.SetBool("isMoving", true);
            }

            else
            {
                animator.SetBool("isMoving", false);
            }

            //if a button for jumping is pressed and the player is on the ground
            if (Input.GetButtonDown("Jump") && isGrounded)

            //calculate jumping using fucking physics. Did not understand. Am not planning to either.
            { Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity); }


            if (isGrounded & Input.GetKey(KeyCode.LeftAlt))
            {
                isFlying = false;
                animator.SetBool("isFlying", false);
                Speed = SpeedWalk;
            }

            if (isGrounded && isFlying && Input.GetKey(KeyCode.S))
            {
                isFlying = false;
                animator.SetBool("isFlying", false);
                Speed = SpeedWalk;
            }
        //gravity
        Velocity.y += Gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);
    }
    }
