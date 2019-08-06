using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplyColorToTrailScript : MonoBehaviour
{
    [SerializeField] private Color _color;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TrailRenderer>().material.SetColor("_UnlitColor", _color);
    }
}
