using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    #region Fields
    public float sightRange;
    public float attackRange;
    public GameObject bullet;
    public float waitToFire;
    public bool readyToFire = true;
    public float timeBeforeFiring;

    public GameObject target;
    public Vector3 targetLastKnownPos;
    public AIState state = AIState.Patrol;
    public GameCharacter.CharacterType type;
    public RaycastHit hit = new RaycastHit();
    public NavMeshAgent agent;
    public Vector3 destination;
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
        if (IsAtDestination() || !agent.hasPath)
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
        targetLastKnownPos = target.transform.position;
        if (Vector3.Distance(target.transform.position, this.gameObject.transform.position) <= attackRange)
        {
            ChangeState(AIState.Attacking);
            agent.SetDestination(this.transform.position);
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
        if (Vector3.Distance(targetLastKnownPos, this.gameObject.transform.position) <= 1)
        {
            ChangeState(AIState.Retreating);
        }
    }
    private void Attack()
    {
        if (readyToFire)
        {
            this.transform.LookAt(target.transform);
            if (!CanSeeTarget())
            {
                targetLastKnownPos = target.transform.position;
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
        if(Vector3.Distance(this.transform.position, destination) <= 2)
        {
            ChangeState(AIState.Patrol);
        }
        if (IsAtDestination() || !agent.hasPath)
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

    #region Check and FInd Destinations
    private bool IsAtDestination()
    {
        if(Vector3.Distance(this.transform.position, destination) <= 2)
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
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + Vector3.left, out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + Vector3.right, out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left * 2), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right * 2), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left / 2), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right / 2), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left * 4), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right * 4), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left / 4), out hit))
        {
            CheckTarget();
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right / 4), out hit))
        {
            CheckTarget();
        }
        if (Physics.SphereCast(transform.position, sightRange, Vector3.forward, out hit))
        {
            CheckTarget();
        }

        RaycastHit[] hits =  Physics.SphereCastAll(transform.position, attackRange, Vector3.forward);
        for(int x = 0; x < hits.Length; x++)
        {
            if (hits[x].collider.tag == "GameCharacter")
            {
                Debug.Log(hits[x].collider.tag);
                hit = hits[x];
                CheckTarget();
            }
        }
    }
    
    private bool CanSeeTarget()
    {
        if (target != null)
        {
            if (Physics.Raycast(transform.position, target.transform.position, out hit))
            {
                return hit.collider.gameObject == target;
            }
        }
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRange, Vector3.forward);
        for (int x = 0; x < hits.Length; x++)
        {
            if (hits[x].collider.gameObject == target)
            {
                if (Physics.Raycast(transform.position, hits[x].collider.gameObject.transform.position, out hit))
                {
                    return hit.collider.gameObject == target;
                }
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
        if (target != null)
            agent.SetDestination(target.transform.position);
    }
    #endregion
}
