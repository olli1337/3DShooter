using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float enemySpeed = 5f;
    [SerializeField] public int ammo;
    [SerializeField] bool hasAmmo = true;
    [SerializeField] float stoppingDistanceAmmo = 15f;
    [SerializeField] float stoppingDistanceNoAmmo = 2f;
    [SerializeField] float shotAccuracy = .45f;
    [SerializeField] Enemies enemies;

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    bool isProvokedByDamage = false;
    EnemyHealth health;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] GameObject eyesStanding;
    [SerializeField] GameObject eyesCrouch;
    private GameObject activeEyes;

    private bool canSeePlayer = false;
    private bool alertedOthers = false;

    [SerializeField] CapsuleCollider standingCapsuleCollider;
    [SerializeField] CapsuleCollider crouchCapsuleCollider;
    [SerializeField] CapsuleCollider proneCapsuleCollider;

    public float shootCooldownTime = 5;
    private string enemyInitialForm;

    // 1 = standing, 2 = crouch, 3 = prone
    public void SetEnemyForm(string mode)
    {
        Debug.Log(mode);
        switch (mode)
        {
            case "Standing":
                activeEyes = eyesStanding;
                standingCapsuleCollider.enabled = true;
                crouchCapsuleCollider.enabled = false;
                proneCapsuleCollider.enabled = false;
                GetComponent<Animator>().SetInteger("DeathMode", 1);
                GetComponent<Animator>().SetInteger("ShootMode", 1);
                break;
            case "Crouch":
                activeEyes = eyesCrouch;
                standingCapsuleCollider.enabled = false;
                crouchCapsuleCollider.enabled = true;
                proneCapsuleCollider.enabled = false;
                if (UnityEngine.Random.Range(0, 1f) > .5f)
                {
                    GetComponent<Animator>().SetInteger("DeathMode", 2);
                }
                else
                {
                    GetComponent<Animator>().SetInteger("DeathMode", 3);
                }
                GetComponent<Animator>().SetInteger("ShootMode", 2);
                break;
            case "Prone":
                activeEyes = eyesCrouch;
                standingCapsuleCollider.enabled = false;
                crouchCapsuleCollider.enabled = false;
                proneCapsuleCollider.enabled = true;
                GetComponent<Animator>().SetInteger("DeathMode", 4);
                GetComponent<Animator>().SetInteger("ShootMode", 3);
                break;
            case "Dead":
                if (standingCapsuleCollider)
                {
                    standingCapsuleCollider.enabled = false;
                }
                if (crouchCapsuleCollider)
                {
                    crouchCapsuleCollider.enabled = false;
                }
                if (proneCapsuleCollider)
                {
                    proneCapsuleCollider.enabled = false;
                }
                break;
        }
    }

    public void ProcessRaycast()
    {

        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (angle < 100f && distanceToTarget < chaseRange)
        {
            RaycastHit hit;
            if (Physics.Linecast(activeEyes.transform.position, target.position, out hit))
            {
                if (hit.transform.name == "Player")
                {
                    Debug.DrawLine(activeEyes.transform.position, target.position, Color.red, .2f);
                    canSeePlayer = true;
                    GetComponent<Animator>().SetBool("CanSeePlayer", true);
                    isProvoked = true;
                }
                else
                {
                    canSeePlayer = false;
                    GetComponent<Animator>().SetBool("CanSeePlayer", false);
                }
            }
        }
        else
        {
            canSeePlayer = false;
            GetComponent<Animator>().SetBool("CanSeePlayer", false);
        }

    }

    public void ReduceAmmo(int amount)
    {
        ammo -= amount;
    }

    public float ShotAccuracy()
    {
        return shotAccuracy;
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistanceAmmo;
        health = GetComponent<EnemyHealth>();
        SetSpeed(enemySpeed);
        RandomizeAnimations();
    }

    void Update()
    {
        if (health.IsDead())
        {
            enabled = false;
            navMeshAgent.enabled = false;
            SetEnemyForm("Dead");
        }
        else
        {
            ProcessRaycast();
            if (isProvoked)
            {
                EngageTarget();
                GetComponent<Animator>().ResetTrigger("TakeDamage");

            }
            if (navMeshAgent.pathPending && (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance) && !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                GetComponent<Animator>().SetTrigger("Idle");
            }
        }
    }

    private void EngageTarget()
    {
        if (!alertedOthers) enemies.DamageNotification(gameObject.transform.position);
        alertedOthers = true;
        FaceTarget();
        if (ammo > 0)
        {
            if (canSeePlayer && distanceToTarget < chaseRange)
            {
                AttackTarget();
            }
            else
            {
                ChaseTarget();
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("HasAmmo", false);
            navMeshAgent.stoppingDistance = stoppingDistanceNoAmmo;
            if (distanceToTarget >= navMeshAgent.stoppingDistance)
            {
                ChaseTarget();
            }

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                AttackTarget();
            }
        }

    }

    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        GetComponent<Animator>().SetBool("Move", true);
        SetEnemyForm("Standing");
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        if (GetComponent<Animator>().GetBool("HasAmmo"))
        {
            SetEnemyForm(enemyInitialForm);
        }
        else
        {
            SetEnemyForm("Standing");
            GetComponent<Animator>().SetInteger("DeathMode", 1);
            GetComponent<Animator>().SetInteger("ShootMode", 1);
        }
        GetComponent<Animator>().SetBool("Attack", true);
        GetComponent<Animator>().SetBool("Move", false);
        navMeshAgent.isStopped = true;
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
        isProvoked = true;
    }

    private void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

    private void ReceiverFunction(Vector3 location)
    {
        if (!isProvoked && !isProvokedByDamage)
        {
            float distance = Mathf.Infinity;
            distance = Vector3.Distance(location, gameObject.transform.position);
            if (distance < chaseRange)
            {
                isProvoked = true;
            }
            //    Debug.Log("Received notification about damage this far away:" + distance);
        }
        else
        {
            //      Debug.Log("I am already angry..!");
        }
    }

    private void RandomizeAnimations()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        shootCooldownTime = UnityEngine.Random.Range(1f, 2f);

        if (rand > .7)
        {
            SetEnemyForm("Standing");
            enemyInitialForm = "Standing";
        }
        else if (rand < .7 && rand > .3)
        {
            SetEnemyForm("Crouch");
            enemyInitialForm = "Crouch";
        }
        else
        {
            SetEnemyForm("Prone");
            enemyInitialForm = "Prone";
        }

    }
}
