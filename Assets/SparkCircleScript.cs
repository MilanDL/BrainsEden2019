using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkCircleScript : MonoBehaviour
{
    CircleScript _circle;
    private float _startRad;
    private float _amplitude = 0;

    // Start is called before the first frame update
    void Start()
    {
        _circle = GetComponent<CircleScript>();
        _startRad = _circle.xradius;
    }

    // Update is called once per frame
    void Update()
    {
        float target = _startRad + _amplitude * Mathf.Sin(Time.time * 15);
        _circle.xradius = target;
        _circle.yradius = target;

        _amplitude = Mathf.Lerp(_amplitude, 0.03f, 5 * Time.deltaTime);
    }

    internal void Pickup()
        {
        _amplitude = 0.6f;
        }
    }
