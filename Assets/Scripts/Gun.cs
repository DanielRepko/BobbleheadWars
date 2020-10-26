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

        currentTime += Time.deltaTime;
        if(currentTime > upgradeTime && isUpgraded == true)
        {
            isUpgraded = false;
        }
    }

    void FireBullet()
    {
        Rigidbody bullet = CreateBullet();
        bullet.velocity = transform.parent.forward * 100;

        // if the gun is upgraded, fire two additional bullets diagonally to the sides and play the upgraded shooting sound
        if (isUpgraded)
        {
            Rigidbody bullet2 = CreateBullet();
            bullet2.velocity = (transform.right + transform.forward / 0.5f) * 100;
            Rigidbody bullet3 = CreateBullet();
            bullet3.velocity = (-transform.right + transform.forward / 0.5f) * 100;
            audioSource.PlayOneShot(SoundManager.Instance.upgradedGunFire);
        }
        else // otherwise just play the regular shooting sound
        {
            audioSource.PlayOneShot(SoundManager.Instance.gunFire);
        }
    }

    //adding the code to create a bullet in its own function
    private Rigidbody CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab);

        bullet.transform.position = launchPosition.position;

        return bullet.GetComponent<Rigidbody>();
    }

    public void UpgradeGun()
    {
        isUpgraded = true;
        currentTime = 0;
    }


}
