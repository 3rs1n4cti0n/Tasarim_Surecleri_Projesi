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

    public IEnumerator SetHPslowly(float newHealth)
    {
        float currentHealth = health.transform.localScale.x;
        float changeAmt = currentHealth - newHealth;

        while(currentHealth - newHealth > Mathf.Epsilon)
        {
            currentHealth -= changeAmt * Time.deltaTime; //* 2;
            setHealth(currentHealth);
            yield return null;
        }
        setHealth(newHealth);
    }
}
