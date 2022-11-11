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
    public float SpeedWalk = 0.4f;
    public float SpeedSprint = 2f;
    bool isSprinting = false;
    public float SprintAcceleration = 1.5f;
    public float AccelerationVelocity;
    public float JumpHeight = 2f;

   
    public float TurnRadius;
    public float TurnRadiusWalk;
    public float TurnRadiusSprint;
    public float TurnRadiusGlide;
    public float TurnRadiusFlight;
    float turnSmoothVelocity;

    public float Gravity;
    Vector3 Velocity;
    
    public Transform GroundCheck;
    public float GroundDistance = 2f;
    public LayerMask GroundMask;
    bool isGrounded;

    public float DistanceToGround = 2f;
    bool isNearGround;
    bool isFlying = false;
    public float brake = 0.3f;
    public float lift = -20f;


    // Start is called before the first frame update
    void Start()
    {
    //Turn Radius Stats
        TurnRadius = 0.2f;
        TurnRadiusWalk = 1f;
        TurnRadiusSprint = 1.2f;
        TurnRadiusGlide = 1f;
        TurnRadiusFlight = 1.3f;
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

        //gravity
        Velocity.y += Gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);

        //Reset velocity after touching the ground
        if (isGrounded && Velocity.y < 0)
        { Velocity.y = -2f;}

        if (Input.GetKey(KeyCode.Space) && !isNearGround && !isGrounded)
        {
            isFlying = true;

        }

        //if the player is flying
        if (isFlying)
        {
            TurnRadius = TurnRadiusGlide;
            Velocity.y = 0f;

            if (Input.GetButtonDown("Vertical"))
            { }

            //and ctrl is pressed
            if (Input.GetKey(KeyCode.LeftControl) && Speed >= 0 + brake)
            {
                //and subtract the braking speed from the current speed
                Speed -= brake;
            }
        }

        //if shift is pressed
        if (Input.GetKey(KeyCode.LeftShift))

        {  //and if the player is not sprinting already
            if (!isSprinting)
            {
                //and add the sprinting speed to the current speed
                Speed += SpeedSprint;
                isSprinting = true;

                //if the player is touching the ground while pressing shift
                if (isGrounded)
                  //change the turn radius to sprinting
                    { TurnRadius = TurnRadiusSprint; }
                //if not
                else
                 //change the turn radius to flying
                    { TurnRadius = TurnRadiusFlight; }

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


        }
        //if a button for jumping is pressed and the player is on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)

        //calculate jumping using fucking physics. Did not understand. Am not planning to either.
        { Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity); }


        

        

        


    }
}
