using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    #region Fields
    public float sightRange;
    public float attackRange;
    public GameObject bullet;
    public GameObject player;
    public float waitToFire;
    public GameObject target;
    public GameCharacter.CharacterType type;

    private float timeBeforeFiring;
    private bool readyToFire = true;
    private Vector3 targetLastKnownPos;
    public AIState state = AIState.Patrol;
    private RaycastHit hit = new RaycastHit();
    private NavMeshAgent agent;
    private Vector3 destination;
    public enum AIState { 
        Patrol,
        Searching,
        Chasing,
        Attacking,
        Retreating
    }
    #endregion

    #region Unity Messages
    private void Start()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        type = this.GetComponent<GameCharacter>().type;
        state = AIState.Patrol;
        destination = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        DoState();
    }
    private void FixedUpdate()
    {
        ResetPath();
    }
    #endregion

    #region AI State Logics
    private void DoState()
    {
        LookForTarget();
        CheckState();
        switch (state) {
            case AIState.Patrol:
                Patrol();
                CheckState();
                break;
            case AIState.Chasing:
                Chase();
                CheckState();
                break;
            case AIState.Searching:
                Search();
                CheckState();
                break;
            case AIState.Attacking:
                Attack();
                CheckState();
                break;
            case AIState.Retreating:
                Retreat();
                CheckState();
                break;
        }
    }
    private void ResetPath()
    {
        switch (state)
        {
            case AIState.Attacking:
                agent.SetDestination(this.transform.position);
                break;
            case AIState.Chasing:
                agent.SetDestination(target.transform.position);
                break;
            case AIState.Searching:
                agent.SetDestination(targetLastKnownPos);
                break;
            case AIState.Retreating:
                agent.SetDestination(destination);
                break;
        }
    }
    #endregion

    #region States
    private void Patrol()
    {
        if (IsAtDestination(destination, 2f) || !agent.hasPath)
        {
            FindNewDestination();
        }
    }
    private void Chase()
    {
        if(CanSeeTarget() == false)
        {
            ChangeState(AIState.Searching);
            return;
        }
        if (IsAtDestination(target.transform.position, attackRange))
        {
            if (Physics.Raycast(transform.position, target.transform.position, out hit))
            {
                ChangeState(AIState.Attacking);
                agent.SetDestination(this.transform.position);
            }
            
        }
        else
        {
            agent.SetDestination(target.gameObject.transform.position);
        }
    }
    private void Search()
    {
        LookForTarget();
        if (CanSeeTarget() == true)
        {
            ChangeState(AIState.Chasing);
            return;
        }
        if (IsAtDestination(targetLastKnownPos, 2f))
        {
            ChangeState(AIState.Retreating);
        }
    }
    private void Attack()
    {
        if (readyToFire)
        {
            this.transform.LookAt(target.transform);
            agent.SetDestination(this.transform.position);
            if (!CanSeeTarget())
            {
                ChangeState(AIState.Searching);
                return;
            }

            agent.SetDestination(this.transform.position);
            GameObject attackingBullet = GameObject.Instantiate(bullet);
            attackingBullet.transform.position = this.transform.position;
            attackingBullet.GetComponent<Bullet>().SetAttacker(this.gameObject);
            readyToFire = false;
            timeBeforeFiring = 0;
            if (!CanSeeTarget())
                ChangeState(AIState.Searching);
            if (target == null)
                return;
            if (target.activeInHierarchy == false)
                ChangeState(AIState.Patrol);
        } else if(timeBeforeFiring >= waitToFire)
        {
            readyToFire = true;
        } else
        {
            timeBeforeFiring += Time.deltaTime;
        }
    }
    private void Retreat()
    {
        agent.SetDestination(destination);
        if (IsAtDestination(destination, 2f) || !agent.hasPath)
        {
            ChangeState(AIState.Patrol);
        }
    }
    #endregion

    #region Check and Changing States
    private void CheckState()
    {
        if(target != null && state != AIState.Attacking)
        {
            ChangeState(AIState.Chasing);
        }
        if (target != null)
            if (target.activeInHierarchy == false)
            {
                target = null;
                ChangeState(AIState.Patrol);
            } else if(Vector3.Distance(this.transform.position, target.transform.position) <= attackRange)
            {
                agent.SetDestination(this.transform.position);
                ChangeState(AIState.Attacking);
            }

    }

    private void ChangeState(AIState targetState)
    {
        state = targetState;
        if (state == AIState.Searching)
        {
            targetLastKnownPos = target.transform.position;
            agent.SetDestination(targetLastKnownPos);
            target = null;
        }
        else if (state == AIState.Patrol)
        {
            FindNewDestination();
        }
    }
    #endregion

    #region Check and Find Destinations
    private bool IsAtDestination(Vector3 destinationPos, float distanceRange)
    {
        if(Vector3.Distance(this.transform.position, destinationPos) <= distanceRange)
            return true;
        return false;
    }

    private void FindNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * sightRange;
        randomDirection += transform.position;
        agent.SetDestination(randomDirection);
    }
    #endregion

    #region Target Finding
    private void LookForTarget()
    {
        if(Vector3.Distance(this.transform.position, player.transform.position) <= attackRange * player.GetComponent<PlayerController>().speedSound)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(player.transform.position), out hit))
            {
                if (hit.collider.gameObject == player && state != AIState.Attacking && state != AIState.Chasing)
                {
                    target = player;
                    targetLastKnownPos = player.transform.position;
                    ChangeState(AIState.Chasing);
                }
                
            }
        }
        if(state == AIState.Searching)
        {
            transform.LookAt(targetLastKnownPos);
            if (Physics.Raycast(transform.position, transform.TransformDirection(player.transform.position), out hit))
            {
                if (hit.collider.gameObject == player && state != AIState.Attacking)
                {
                    target = player;
                    targetLastKnownPos = player.transform.position;
                    ChangeState(AIState.Chasing);
                }
            }
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left * 2) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left * 4) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left / 2) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left / 4) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right * 2) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right * 4) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right / 2) * 1000, Color.black);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right / 4) * 1000, Color.black);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 1000, out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left * 2)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right * 2)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left / 2)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right / 2)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left * 4)) * 1000, out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right * 4)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left / 4)) * 1000 , out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right / 4)) * 1000 , out hit))
        {
            CheckTarget();
        }
    }
    
    private bool CanSeeTarget()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) <= 10f * player.GetComponent<PlayerController>().speedSound)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(player.transform.position), out hit))
            {
                if(hit.collider.gameObject == player)
                    return true;
            }
        }
        if (state == AIState.Searching)
        {
            transform.LookAt(targetLastKnownPos);
            if (Physics.Raycast(transform.position, transform.TransformDirection(player.transform.position), out hit))
            {
                if (hit.collider.gameObject == player)
                    return true;
            }
        }
        if (target != null)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.left) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + Vector3.right) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left * 2)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right * 2)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left / 2)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right / 2)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left * 4)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right * 4)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.left / 4)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + (Vector3.right / 4)) * 1000, out hit))
            {
                return hit.collider.gameObject == target;
            }
        }
        return false;
    }

    private void CheckTarget()
    {
        if ((hit.collider.gameObject.tag == "GameCharacter") && (hit.collider.gameObject != this))
        {
            if (Vector3.Distance(this.transform.position, hit.collider.gameObject.transform.position) <= sightRange)
            {
                if (hit.collider.gameObject.GetComponent<GameCharacter>().type != type) {
                    target = hit.collider.gameObject;
                    targetLastKnownPos = target.gameObject.transform.position;
                } else if(target != null)
                {
                    if(hit.collider.gameObject == target)
                    {
                        targetLastKnownPos = target.gameObject.transform.position;
                    }
                }
            }
        }
    }
    #endregion
}
