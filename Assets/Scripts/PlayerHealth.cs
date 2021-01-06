using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] AudioSource damageAudio;
    [SerializeField] AudioSource missAudio;
    [SerializeField] TextMeshProUGUI healthText;

    public void Start()
    {
        DisplayHealth();
    }

    public void TakeDamage(float damage)
    {
        DisplayHealth();
        if (damage > 0)
        {
            hitPoints -= damage;
            damageAudio.Play();
            if (hitPoints <= 0f)
            {
                GetComponent<PDeathHandler>().HandleDeath();
            }
        }
        else
        {
            missAudio.Play();
        }
    }

    private void DisplayHealth()
    {
        int currentHealth = (int)hitPoints;
        healthText.text = hitPoints.ToString() + " HP";
    }
}
