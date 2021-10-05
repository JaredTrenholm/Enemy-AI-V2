using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    #region Fields
    public float sightRange;
    public float attackRange;

    public GameObject target;
    public Vector3 targetLastKnownPos;
    public EnemyState state = EnemyState.Patrol;
    public GameCharacter.CharacterType type;
    public RaycastHit hit = new RaycastHit();
    public NavMeshAgent agent;
    public Vector3 destination;
    public enum EnemyState { 
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
        state = EnemyState.Patrol;
        destination = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        AIState();
    }
    private void FixedUpdate()
    {
        ResetPath();
    }
    #endregion

    #region AI State Logics
    private void AIState()
    {
        LookForTarget();
        switch (state) {
            case EnemyState.Patrol:
                Patrol();
                CheckState();
                break;
            case EnemyState.Chasing:
                Chase();
                CheckState();
                break;
            case EnemyState.Searching:
                Search();
                CheckState();
                break;
            case EnemyState.Attacking:
                Attack();
                CheckState();
                break;
            case EnemyState.Retreating:
                Retreat();
                CheckState();
                break;
        }
    }
    private void ResetPath()
    {
        switch (state)
        {
            case EnemyState.Chasing:
                agent.SetDestination(target.transform.position);
                break;
            case EnemyState.Searching:
                agent.SetDestination(targetLastKnownPos);
                break;
            case EnemyState.Retreating:
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
            ChangeState(EnemyState.Searching);
            return;
        }
        targetLastKnownPos = target.transform.position;
        if (Vector3.Distance(target.transform.position, this.gameObject.transform.position) <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
            agent.SetDestination(this.transform.position);
        }
        else
        {
            agent.SetDestination(target.gameObject.transform.position);
        }
    }
    private void Search()
    {
        if (CanSeeTarget() == true)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }
        if (Vector3.Distance(targetLastKnownPos, this.gameObject.transform.position) <= attackRange)
        {
            ChangeState(EnemyState.Retreating);
        }
    }
    private void Attack()
    {
        target.GetComponent<GameCharacter>().TakeDamage(this.gameObject);
        if (!CanSeeTarget())
            ChangeState(EnemyState.Searching);
        if (target == null)
            return;
        if(target.activeInHierarchy == false)
            ChangeState(EnemyState.Patrol);
    }
    private void Retreat()
    {
        agent.SetDestination(destination);
        if(Vector3.Distance(this.transform.position, destination) <= 2)
        {
            ChangeState(EnemyState.Patrol);
        }
        if (IsAtDestination() || !agent.hasPath)
        {
            ChangeState(EnemyState.Patrol);
        }
    }
    #endregion

    #region Check and Changing States
    private void CheckState()
    {
        if(target != null && state != EnemyState.Attacking)
        {
            ChangeState(EnemyState.Chasing);
        }
        if (target != null)
            if (target.activeInHierarchy == false)
            {
                target = null;
                ChangeState(EnemyState.Patrol);
            }
    }

    private void ChangeState(EnemyState targetState)
    {
        state = targetState;
        if (state == EnemyState.Searching)
        {
            targetLastKnownPos = target.transform.position;
            agent.SetDestination(targetLastKnownPos);
            target = null;
        } else if(state == EnemyState.Patrol)
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
    }
    
    private bool CanSeeTarget()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + Vector3.left, out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + Vector3.right, out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left * 2), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right * 2), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left / 2), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right / 2), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left * 4), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right * 4), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.left / 4), out hit))
        {
            return hit.collider.gameObject == target;
        }
        if (Physics.Raycast(transform.position, Vector3.forward + (Vector3.right / 4), out hit))
        {
            return hit.collider.gameObject == target;
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
                }
            }
        }
        if (target != null)
            agent.SetDestination(target.transform.position);
    }
    #endregion
}
