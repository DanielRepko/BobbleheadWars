using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    //holds the prefab info for the bullet
    public GameObject bulletPrefab;
    //the position for where to fire the bullets from on the player model
    public Transform launchPosition;

    private AudioSource audioSource;

    public bool isUpgraded;
    public float upgradeTime = 10.0f;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
       
    }

    // Update is called once per frame
    void Update()
    {
        //checking if the the left mouse button is being held
        if (Input.GetMouseButtonDown(0))
        {
            //checking if FireBullet() is being invoked
            if (!IsInvoking("FireBullet"))
            {
                //repeatedly call FireBullet()
                InvokeRepeating("FireBullet", 0f, 0.1f);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            CancelInvoke("FireBullet");
        }
    }

    void FireBullet()
    {
        //creating an instance of the bullet prefab
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        //moving the bullet to the launchPosition
        bullet.transform.position = launchPosition.position;
        //adding velocity to the bullet (shooting it)
        bullet.GetComponent<Rigidbody>().velocity = transform.parent.forward * 100;
        // PlayOneShot plays the sound once but allows it to be played again and to overlap the last play
        audioSource.PlayOneShot(SoundManager.Instance.gunFire);
    }
}
