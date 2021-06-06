using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    // Layers
    [SerializeField] LayerMask solid;
    [SerializeField] LayerMask grass;
    [SerializeField] LayerMask interactables;

    public static GameLayers inst { get; set; }

    private void Awake()
    {
        inst = this;
    }

    public LayerMask SolidLayer
    {
        get => solid;
    }
    public LayerMask GrassLayer
    {
        get => grass;
    }
    public LayerMask InteractablesLayer
    {
        get => interactables;
    }
}
