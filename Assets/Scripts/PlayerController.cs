using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50f;
    private CharacterController characterController;
    public Rigidbody head;
    private Vector3 wobbleDirection;

    // Start is called before the first frame update
    void Start()
    {
        //initializing the characterController property
        characterController = GetComponent<CharacterController>();
        wobbleDirection = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),
                                            0,
                                            Input.GetAxis("Vertical"));
        characterController.SimpleMove(moveDirection * moveSpeed);
    }

    private void FixedUpdate()
    {
        //make a vector3 to hold the direction the player is moving
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),
                                            0, Input.GetAxis("Vertical"));
        //if the player is not moving
        if(moveDirection == Vector3.zero)
        {
            //TODO
        }
        //if moveDirection isn't zero then player must be moving
        else
        {
            //added my own code to make the head actually wobble instead of just being pushed to the right
            Quaternion headRotation = head.GetComponent<Transform>().localRotation;
            Debug.Log(headRotation.z);
            //set wobble direction to left if the head is all the way to the right
            if (headRotation.z <= -.2)
            {
                wobbleDirection = -transform.right;
            }
            //set wobble direction to right if the head is all the way to the left
            else if (headRotation.z >= .2)
            {
                wobbleDirection = transform.right;
            }
            //add force to the head to make it wobble
            head.AddForce(wobbleDirection * 150, ForceMode.Acceleration);

            //head.AddForce(-transform.right * 150, ForceMode.Acceleration);
        }
    }
}
