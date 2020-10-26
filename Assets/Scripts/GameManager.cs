using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] spawnPoints;
    public GameObject alien;
    public int maxAliensOnScreen;
    public int totalAliens;
    public float minSpawnTime;
    public float maxSpawnTime;
    public int aliensPerSpawn;

    // holding the prefab info for the pickup
    public GameObject upgradePrefab;
    // the Gun script
    public Gun gun;
    // the time before another upgrade can spawn
    public float upgradeMaxTimeSpawn = 7.5f;

    private int aliensOnScreen = 0;
    private float generatedSpawnTime = 0;
    private float currentSpawnTime = 0;

    private bool spawnedUpgrade = false;
    private float actualUpgradeTime = 0;
    private float currentUpgradeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        actualUpgradeTime = Random.Range(upgradeMaxTimeSpawn - 3.0f, upgradeMaxTimeSpawn);
        actualUpgradeTime = Mathf.Abs(actualUpgradeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }

        currentUpgradeTime += Time.deltaTime;

        if (currentUpgradeTime > actualUpgradeTime)
        {
            if (!spawnedUpgrade)
            {
                int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                GameObject spawnLocation = spawnPoints[randomNumber];

                GameObject upgrade = Instantiate(upgradePrefab);
                Upgrade upgradeScript = upgrade.GetComponent<Upgrade>();
                upgradeScript.gun = gun;
                upgrade.transform.position = spawnLocation.transform.position;
                spawnedUpgrade = true;
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.powerUpAppear);
            }
        }

        //code to determine if and when to spawn more aliens
        currentSpawnTime += Time.deltaTime;
        if(currentSpawnTime > generatedSpawnTime)
        {
            currentSpawnTime = 0;
            generatedSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

            //if aliens can be spawned and there are not too many aliens on screen already
            if (aliensPerSpawn > 0 && aliensOnScreen < totalAliens)
            {
                List<int> previousSpawnLocations = new List<int>();
                if (aliensPerSpawn > spawnPoints.Length)
                {
                    aliensPerSpawn = spawnPoints.Length - 1;                    
                }
                aliensPerSpawn = (aliensPerSpawn > totalAliens) ? aliensPerSpawn - totalAliens : aliensPerSpawn;

                for(int i = 0; i < aliensPerSpawn; i++)
                {
                    if(aliensOnScreen < maxAliensOnScreen)
                    {
                        aliensOnScreen += 1;

                        int spawnPoint = -1;
                        while(spawnPoint == -1)
                        {
                            int randomNumber = Random.Range(0, spawnPoints.Length - 1);
                            if (!previousSpawnLocations.Contains(randomNumber))
                            {
                                previousSpawnLocations.Add(randomNumber);
                                spawnPoint = randomNumber;
                            }
                        }
                        GameObject spawnLocation = spawnPoints[spawnPoint];

                        //instantiating the Alien prefab
                        GameObject newAlien = Instantiate(alien) as GameObject;

                        newAlien.transform.position = spawnLocation.transform.position;

                        //setting the player as the alien's target
                        Alien alienScript = newAlien.GetComponent<Alien>();
                        alienScript.target = player.transform;

                        //rotating the alien to face the player
                        Vector3 targetRotation = new Vector3(player.transform.position.x,
                                                             newAlien.transform.position.y,
                                                             player.transform.position.z);
                        newAlien.transform.LookAt(targetRotation);

                        alienScript.OnDestroy.AddListener(AlienDestroyed);
                    }
                }
            }
        }        
    }

    public void AlienDestroyed()
    {
        aliensOnScreen -= 1;
        totalAliens -= 1;
    }
}
