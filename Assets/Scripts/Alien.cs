using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Alien : MonoBehaviour
{
    //the target for the alien to follow
    public Transform target;
    private NavMeshAgent agent;

    //the amount of time when the alien should update its path
    public float navigationUpdate;
    private float navigationTime = 0;

    public UnityEvent OnDestroy;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navigationTime += Time.deltaTime;
        if(navigationTime > navigationUpdate)
        {
            agent.destination = target.position;
            navigationTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Die();
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.alienDeath);
    }

    public void Die()
    {
        // destroying the gameobject
        Destroy(gameObject);
        // invoking OnDestroy
        OnDestroy.Invoke();
        // removing listeners 
        OnDestroy.RemoveAllListeners();
    }
}
