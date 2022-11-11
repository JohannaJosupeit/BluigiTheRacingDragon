using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody BluigiRB;
    [SerializeField] float VerticalWalkSpeed = 5f;
    [SerializeField] float HorizontalWalkSpeed = 5f;
    [SerializeField] float TurnSpeed = 10f;
    [SerializeField] float JumpHeight = 5f;

    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask Ground;

    // Start is called before the first frame update
    void Start()
    {
        BluigiRB = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       
        //multíply the walking speed by the input for each axis. This allows Inputs other than 1 and 0 and makes everything smoother.
        BluigiRB.velocity = new Vector3( horizontalInput * HorizontalWalkSpeed , BluigiRB.velocity.y, verticalInput * VerticalWalkSpeed);
  
        //if a button for vertical movement is pressed
        if (Input.GetButtonDown("Vertical"))
        //change the player's velocity on the x-axis to their vertical walking speed
        { BluigiRB.velocity = new Vector3(VerticalWalkSpeed, BluigiRB.velocity.y, BluigiRB.velocity.z); }

    
        //if a button for horizantal movement is pressed
        if (Input.GetButtonDown("Horizontal"))
        //change the player's velocity on the z-axis to their horizontal walking speed
        { BluigiRB.velocity = new Vector3(BluigiRB.velocity.x, BluigiRB.velocity.y, HorizontalWalkSpeed); }
      

        //if a button for jumping is pressed and the player is touching the ground
        if (Input.GetButtonDown("Jump") && IsGrounded())
        //change the player's velocity on the y-axis to their jump height
        { BluigiRB.velocity = new Vector3(BluigiRB.velocity.x, JumpHeight, BluigiRB.velocity.z); }


        // checks, if a sphere at the bottom of the player with a radius of 0.1 is intersecting with the "Ground" layer
        bool IsGrounded()
        {
           return Physics.CheckSphere(GroundCheck.position, .1f, Ground);

        }
    }
}
