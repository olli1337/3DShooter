using System;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 20;
    [SerializeField] float magazineSize = 20;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] GameObject EmptyShell;
    [SerializeField] Vector3 EmptyShellLocation;
    [SerializeField] Vector3 EmptyShellRotation;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float shootCooldownTime;
    [SerializeField] AmmoType ammoType;
    [SerializeField] Enemies enemies;
    [SerializeField] GameObject bulletRayOrigin;
    [SerializeField] TextMeshProUGUI ammoText;
    private bool shootCooldown;

    void Update()
    {
        DisplayAmmo();
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if (ammoSlot.GetCurrentAmmo(ammoType) > 0 && !shootCooldown)
            {
                Invoke("ResetShotCooldown", shootCooldownTime);
                Shoot();
            }
        }
    }

    private void ResetShotCooldown()
    {
        shootCooldown = false;
    }

    private void Shoot()
    {
        PlayMuzzleFlash();
        ProcessRayCast();
        ammoSlot.ReduceCurrentAmmo(ammoType);
        CreateShell();
        PlayGunShotSound();
        shootCooldown = true;
        // Inform the enemies where the gunshot was made. This is in order to simulate the hearing of the gunshot noices.
        enemies.DamageNotification(gameObject.transform.position);
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(bulletRayOrigin.transform.position, FPCamera.transform.forward, out hit, range))
        {
            CreateHitImpact(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target == null) return;
            target.TakeDamage(damage);
        }
        else
        {
            return;
        }
    }

    private void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        // Inform the enemies where the gunshot hit. This is in order to simulate the hearing of the gunshot hit noices.
        enemies.DamageNotification(hit.point);
        Destroy(impact, .1f);
    }


    private void CreateShell()
    {
        GameObject shellPos = GameObject.Find("EmptyShellPosition");
        Instantiate(EmptyShell, shellPos.transform.position, Quaternion.identity);
    }

    private void PlayGunShotSound()
    {
        audioSource.Play();
    }

}
