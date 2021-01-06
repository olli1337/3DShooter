using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] float hitPoints = 100f;
    bool isDead = false;

    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(float damage)
    {
        BroadcastMessage("OnDamagetaken");
        hitPoints -= damage;
        GetComponent<Animator>().SetTrigger("TakeDamage");
        if (hitPoints <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GetComponent<Animator>().GetBool("Move"))
        {
            GetComponent<Animator>().SetInteger("DeathMode", 1);
            GetComponent<Animator>().SetInteger("ShootMode", 1);
        }
        if (isDead) return;
        isDead = true;
        GetComponent<Animator>().SetTrigger("die");
        GetComponent<Animator>().SetBool("IsAlive", false);
    }

}
