using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbarScript : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void setHealth(float healthNormalized)
    {
        health.transform.localScale = new Vector3(healthNormalized, 1f);
    }
}
