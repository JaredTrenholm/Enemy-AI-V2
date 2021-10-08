using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public GameObject patrol;
    public GameObject search;
    public GameObject chase;
    public GameObject attack;
    public GameObject retreat;
    public EnemyAI.AIState state;
    void Update()
    {
        state = gameObject.GetComponent<EnemyAI>().state;
        switch (state)
        {
            case EnemyAI.AIState.Searching:
                search.SetActive(true);
                patrol.SetActive(false);
                chase.SetActive(false);
                attack.SetActive(false);
                retreat.SetActive(false);
                break;
            case EnemyAI.AIState.Patrol:
                search.SetActive(false);
                patrol.SetActive(true);
                chase.SetActive(false);
                attack.SetActive(false);
                retreat.SetActive(false);
                break;
            case EnemyAI.AIState.Attacking:
                search.SetActive(false);
                patrol.SetActive(false);
                chase.SetActive(false);
                attack.SetActive(true);
                retreat.SetActive(false);
                break;
            case EnemyAI.AIState.Chasing:
                search.SetActive(false);
                patrol.SetActive(false);
                chase.SetActive(true);
                attack.SetActive(false);
                retreat.SetActive(false);
                break;
            case EnemyAI.AIState.Retreating:
                search.SetActive(false);
                patrol.SetActive(false);
                chase.SetActive(false);
                attack.SetActive(false);
                retreat.SetActive(true);
                break;
        }

    }
}
