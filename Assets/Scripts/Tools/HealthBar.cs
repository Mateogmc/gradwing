using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Material healthBarMaterial;
    [SerializeField] Material healthBarProgressMaterial;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarProgress;
    [SerializeField] float currentHealth;
    float oldHealth = 1;
    float t;

    private void Start()
    {
        healthBar.material = new Material(healthBarMaterial);
        healthBarProgress.material = new Material(healthBarProgressMaterial);
    }

    private void Update()
    {
        if (t > Time.time)
        {
            if (currentHealth < oldHealth)
            {
                healthBarProgress.material.SetFloat("_Health", Mathf.Lerp(currentHealth, oldHealth, Mathf.Pow(t - Time.time, 3)));
            }
            else
            {
                healthBar.material.SetFloat("_Health", Mathf.Lerp(currentHealth, oldHealth, Mathf.Pow(t - Time.time, 3)));
            }
            //Debug.Log(oldHealth + "   " + currentHealth);
        }
        else
        {
            healthBarProgress.material.SetFloat("_Health", currentHealth);
            healthBar.material.SetFloat("_Health", currentHealth);
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float health)
    {
        if (health < 0) { health = 0; }
        if (health > 1) { health = 1; }
        health = 1 - health;
        if ((health == 0 && currentHealth == 0) || (health == 1 && currentHealth == 1)) { return; }
        if (health < currentHealth)
        {
            currentHealth = health;
            oldHealth = healthBar.material.GetFloat("_Health"); 
            healthBar.material.SetFloat("_Health", currentHealth);
            healthBarProgress.material.SetInt("_IsDecreasing", 1);
            //StopAllCoroutines();
            //StartCoroutine(HealthRoutine());
        }
        else if (health > currentHealth)
        {
            currentHealth = health;
            oldHealth = healthBar.material.GetFloat("_Health");
            healthBarProgress.material.SetFloat("_Health", currentHealth);
            healthBarProgress.material.SetInt("_IsDecreasing", 0);
            // healthBar.material.SetFloat("_Health", currentHealth);
        }
        t = Time.time + 1;
    }

    private IEnumerator HealthRoutine()
    {
        float t = Time.time + 1;
        float oldHealth = healthBar.material.GetFloat("_Health");
        while (t > Time.time)
        {
            healthBar.material.SetFloat("_Health", 1 - Mathf.Lerp(oldHealth, currentHealth, Mathf.Sqrt(Time.time + 1 - t)));
            yield return new WaitForEndOfFrame();
        }
    }
}
