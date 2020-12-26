using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float chaseRange = 5f;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (isProvoked && distanceToTarget <= chaseRange)
        {
            EngageTarget();
        }
        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
        }
        else if (distanceToTarget > chaseRange)
        {
            isProvoked = false;
        }
        if (navMeshAgent.pathPending && (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) && !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            GetComponent<Animator>().SetTrigger("Idle");
        }
    }

    private void EngageTarget()
    {
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }
    //  GetComponent<Animator>().SetTrigger("Idle");
    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        GetComponent<Animator>().SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("Attack", true);
        Debug.Log("Attacking target: " + target.name);
    }

    private void OnDrawGizmosSelected()
    {
        // Display radius when selected
        Gizmos.color = new Color(1, 1, 0, .5f);
        Gizmos.DrawSphere(transform.position, chaseRange);
    }

}
