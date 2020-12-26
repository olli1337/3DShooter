using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float enemySpeed = 3.5f;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    bool isProvokedByDamage = false;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        setSpeed(enemySpeed);
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
        else if (distanceToTarget > chaseRange && !isProvokedByDamage)
        {
            isProvoked = false;
        } else if(isProvokedByDamage)
        {
            EngageTarget();
        }
        if (navMeshAgent.pathPending && (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) && !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            GetComponent<Animator>().SetTrigger("Idle");
        }
    }

    private void EngageTarget()
    {
        FaceTarget();
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
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        // Display radius when selected
        Gizmos.color = new Color(1, 1, 0, .5f);
        Gizmos.DrawSphere(transform.position, chaseRange);
    }

    public void OnDamagetaken()
    {
        isProvokedByDamage = true;
        setSpeed(5f);
    }

    private void setSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

}
