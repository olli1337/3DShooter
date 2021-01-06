using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBoxScript : MonoBehaviour
{

    [SerializeField] Canvas victoryCanvas;

    private void Start()
    {
        victoryCanvas.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Debug.Log("Victory!!");
            victoryCanvas.enabled = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FindObjectOfType<WeaponSwitcher>().enabled = false;
        }
    }
}
