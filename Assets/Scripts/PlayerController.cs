using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50f;
    private CharacterController characterController;
    public Rigidbody head;
    private Vector3 wobbleDirection;
    public LayerMask layerMask;
    private Vector3 currentLookTarget = Vector3.zero;
    public Animator bodyAnimator;
    public float[] hitForce;
    public float timeBetweenHits = 2.5f;
    private bool isHit = false;
    private float timeSinceHit = 0;
    private int hitNumber = -1;
    public Rigidbody marineBody;
    private DeathParticles deathParticles;

    // Start is called before the first frame update
    void Start()
    {
        //initializing the characterController property
        characterController = GetComponent<CharacterController>();
        wobbleDirection = transform.right;
        deathParticles = gameObject.GetComponentInChildren<DeathParticles>();
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
        //making sure that the player stays at the same elevation (the aliens will sometimes send him upwards when colliding with him)
        if(transform.position.y != 12.54)
        {
            transform.position = new Vector3(transform.position.x, 12.54f, transform.position.z);
        }
        //make a vector3 to hold the direction the player is moving
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"),
                                            0, Input.GetAxis("Vertical"));
        //if the player is not moving
        if(moveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
        }
        //if moveDirection isn't zero then player must be moving
        else
        {
            //added my own code to make the head actually wobble instead of just being pushed to the right
            Quaternion headRotation = head.GetComponent<Transform>().localRotation;
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

            bodyAnimator.SetBool("IsMoving", true);
        }

        //creating a raycast
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);

        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            //making sure that currentLookTarget is filled with the very last thing that the raycast hit
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
            }
            //getting the position of where the mouse clicked, from the marines perspective (the y is from the marine, not the hit)
            Vector3 targetPositon = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            //getting the rotation to give the marine to make them turn towards the targetPosition
            Quaternion rotation = Quaternion.LookRotation(targetPositon - transform.position);
            //rotating the marine to look at the targetPosition
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
        }

        if (isHit)
        {
            timeSinceHit += Time.deltaTime;
            if(timeSinceHit > timeBetweenHits)
            {
                isHit = false;
                timeSinceHit = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Alien alien = other.gameObject.GetComponent<Alien>();
        if (alien != null)
        {
            if (!isHit)
            {
                hitNumber += 1;
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                if(hitNumber < hitForce.Length)
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                }
                else
                {
                    Die();
                }
                isHit = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
            }
            alien.Die();
        }
    }

    public void Die()
    {
        bodyAnimator.SetBool("IsMoving", false);
        marineBody.transform.parent = null;
        marineBody.isKinematic = false;
        marineBody.useGravity = true;
        marineBody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        marineBody.gameObject.GetComponent<Gun>().enabled = false;

        Destroy(head.gameObject.GetComponent<HingeJoint>());
        head.transform.parent = null;
        head.useGravity = true;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.marineDeath);

        deathParticles.Activate();

        Destroy(gameObject);
    }
}
