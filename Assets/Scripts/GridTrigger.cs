using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridTrigger : MonoBehaviour
{
    [SerializeField] private GameObject healer;
    [SerializeField] private int healAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("code gets here!!!");
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.Heal(healAmount);
            }
            Destroy(healer);
        }
    }
}
