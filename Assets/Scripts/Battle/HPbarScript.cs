using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbarScript : MonoBehaviour
{
    // object that holds health bar
    [SerializeField] GameObject health;

    // function to set the health bar by % of health
    public void setHealth(float healthNormalized)
    {
        health.transform.localScale = new Vector3(healthNormalized, 1f);
    }

    // sets health bar slowly depending on time
    public IEnumerator SetHPslowly(float newHealth)
    {
        // current health
        float currentHealth = health.transform.localScale.x;
        // change amount from previous health
        float changeAmt = currentHealth - newHealth;

        // while loop to set it slowly with deltaTime
        while(currentHealth - newHealth > Mathf.Epsilon)
        {
            currentHealth -= changeAmt * Time.deltaTime;
            setHealth(currentHealth);
            yield return null;
        }
        // set it to new health
        setHealth(newHealth);
    }
}
