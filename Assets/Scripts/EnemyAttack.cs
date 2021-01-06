using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    PlayerHealth target;
    [SerializeField] float damage = 40f;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] AudioSource meleeAudioSource;
    [SerializeField] AudioSource rangedAudioSource;
    private int shotsFired = 0;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
  
    }

    private void ResetAttackCooldown()
    {
        GetComponent<Animator>().SetBool("AttackCooldown", false);
    }

    public void RangedAttackHitEvent()
    {
        shotsFired++;
        rangedAudioSource.Play();
        if (enemyAI.ammo > 0 && !GetComponent<Animator>().GetBool("AttackCooldown") && GetComponent<Animator>().GetBool("Attack"))
        {
            Invoke("ResetAttackCooldown", enemyAI.shootCooldownTime);
        }
        GetComponent<Animator>().SetBool("AttackCooldown", true);
        float missChance = Random.Range(0f, 1f);
        // Miss
        if (missChance > enemyAI.ShotAccuracy())
        {
            if (target == null) return;
            target.TakeDamage(0);
            enemyAI.ReduceAmmo(1);
        }
        // Hit 
        else
        {
            if (target == null) return;
            target.TakeDamage(damage);
            enemyAI.ReduceAmmo(1);
        }
        if(shotsFired == 5 && enemyAI.ammo > 0)
        {
            ReloadWeapon();
        }
    }

    public void MeleeAttackHitEvent()
    {
        if (target == null) return;
        target.TakeDamage(damage);
        meleeAudioSource.Play();
    }

    public void ReloadWeapon()
    {
        GetComponent<Animator>().SetTrigger("Reload");
        shotsFired = 0;
    }

}
