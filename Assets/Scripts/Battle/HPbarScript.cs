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
}
