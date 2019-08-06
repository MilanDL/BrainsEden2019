using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnEmpty : MonoBehaviour
{
    private ParticleSystem _part;
    
    // Start is called before the first frame update
    void Start()
    {
        _part = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_part.particleCount <= 0)
            Destroy(gameObject);
    }
}
