using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class ThirdPersonMovement : MonoBehaviour

   
{

    public CharacterController controller;
    public Transform cam;

    public Vector3 move, moveDirfw;
    public float Speed;
    public float SpeedFlight;
    public float SpeedWalk;
    public float SpeedSprint;
    public float SpeedDiving;
    public float SpeedGliding;
    public float CurrentPitch, PitchRate;
    bool isSprinting = false;
    bool isSpeedingUp = false;
    bool isFlyingUp = false;
    public float SprintAcceleration;
    public float SprintDeceleration;
    public float AccelerationVelocity;
    public float JumpHeight;
    public float flytodivespeed;
    public float keepFlyingSpeed;
    public float currentVelocity_y;


    public float TurnRadius, TurnRadius2;
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
    public GameObject player;
    public Transform playerT;


    // Start is called before the first frame update
    void Start()
    {
    //Turn Radius Stats
        TurnRadius = 0.2f;
        TurnRadius2 = 150f;
        TurnRadiusWalk = 0.8f;
        TurnRadiusSprint = 1.2f;
        TurnRadiusGlide = 1f;
        TurnRadiusFlight = 1.3f;

        currentVelocity_y = 0f;
        Speed = 3f;
        SpeedWalk = 3f;
        SpeedSprint = 5f;
        SpeedFlight = 33f;
        SpeedDiving = 2f;
        SpeedGliding = 20f;
        CurrentPitch = 0f;
        PitchRate = 30f;
        flytodivespeed = 1f;
        SprintAcceleration = 1.5f;
        SprintDeceleration = 1.5f;
        keepFlyingSpeed = 50f;
        JumpHeight = 2f;

        //Grounded Stats
        GroundDistance = 0.2f;
        DistanceToGround = 1f;
        Gravity = -9.81f;
        animator = GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("BluigiCharacter");
        }
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
            if (vertical < 0)
            {
                animator.SetBool("isDiving", true);
            }
            else
            {
                animator.SetBool("isDiving", false);
            }
            if (vertical > 0)
            {

            }

            if (direction.z < 0f)
            {
                direction.z = 0f;
            }

            Velocity.y = currentVelocity_y;
            TurnRadius = TurnRadiusGlide;


            //if shift is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Flight", true);
                // accelerate to the dragon's max. speed.
                Speed = Mathf.Lerp(Speed, SpeedFlight, 1f * Time.deltaTime);
            }

            //if shift is not pressed anymore
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animator.SetBool("Flight", false);
                //reset the boolean and turn radius to gliding
                isSpeedingUp = false;
                { TurnRadius = TurnRadiusGlide; }
            }

            //If ctrl is pressed while flying
            if (Input.GetKey(KeyCode.LeftControl))
            {
                //smoothly transition from the dragon's current speed to 0.
                float velocity = 0f;
                Speed = Mathf.SmoothDamp(Speed, 0f, ref velocity, 3f * Time.deltaTime);
                Velocity.y = 0f;
            }


            if (Speed < 0.1f)
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
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isFlying)
            {
                //set the speed to the walking speed
                Speed = SpeedWalk;
                isSprinting = false;
            }
            



            if (direction.magnitude >= 0.1f || isFlying)

            {
                  //transform.Rotate(Vector3.up * horizontal * Time.deltaTime * TurnRadius2);


            //calculate the character's new angle, convert the result to degrees instead of radians, store it,
            float targetAngle =  Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerT.eulerAngles.y;

            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnRadius);

            //rotate the character said degrees around the y axis,
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (vertical != 0 && isFlying)
                {
                /* CurrentPitch = Mathf.Lerp(CurrentPitch, PitchRate, 3f * Time.deltaTime);
                Vector3 movDirUp = transform.up * Time.deltaTime * CurrentPitch * vertical;
                move = Vector3.Lerp(move, moveDirfw, 1f * Time.deltaTime);
                controller.Move(movDirUp);


                Speed = Mathf.Lerp(Speed, 0f, flytodivespeed * Time.deltaTime);
                 */

                

                CurrentPitch = Mathf.Lerp(CurrentPitch, PitchRate, 1f * Time.deltaTime);
                    Vector3 movDirUp = transform.up * Time.deltaTime * CurrentPitch * vertical * 1.5f;

                    move = Vector3.Lerp(move, movDirUp, 1f * Time.deltaTime);
                    /*if (vertical < 0 || Speed >= 0.1f)
                    {
                      Speed += 0.1f;
                      CurrentPitch += 0.1f;
                    }
                    else
                    {
                        Speed -= 0.1f;
                        CurrentPitch -= 0.1f;
                    }*/
                    
                }

                else
                {
                move = Vector3.Lerp(move, moveDirfw, 10f * Time.deltaTime);
                }

               

                //store new direction
                moveDirfw = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * Speed * Time.deltaTime;
                
                
                //and move the character in that direction multiplied by the speed and time (to make it frame-rate-independent).
                controller.Move(move);

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
