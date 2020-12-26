using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] float hitPoints = 100f;

    public void TakeDamage(float damage)
    {
        BroadcastMessage("OnDamagetaken");
        hitPoints -= damage;
        if (hitPoints <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
