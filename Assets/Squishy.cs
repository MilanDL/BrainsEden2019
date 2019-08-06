using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squishy : MonoBehaviour
{
    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;
    [SerializeField] private float _dampen;
    private bool _custom;
    private float _time;
    private float _ampl;

    private float _defampl, _deffreq, _defdamp;

    private void Awake()
        {
        _defampl = _amplitude;
        _deffreq = _frequency;
        _defdamp = _dampen;
        }

    private void OnValidate()
        {
        _defampl = _amplitude;
        _deffreq = _frequency;
        _defdamp = _dampen;
        }

    // Update is called once per frame
    void Update()
    {
        float sin = _ampl * Mathf.Sin(_time * _frequency);
        transform.localScale = Vector3.one + new Vector3(sin, -sin, 0);
        _ampl = Mathf.Lerp(_ampl, 0, _dampen * Time.deltaTime);

        _time += Time.deltaTime;
    }

    public void Squish(float amplitude = -99, float frequency = -99, float dampen = -99)
        {
        if (amplitude != -99) _amplitude = amplitude;
        if (frequency != -99) _frequency = frequency;
        if (dampen != -99) _dampen = dampen;
        _custom = true;

        Squish();
        }

    public void Squish()
        {
        if (!_custom)
            {
            _amplitude = _defampl;
            _frequency = _deffreq;
            _dampen = _defdamp;
            }

        _time = 0;
        _ampl = _amplitude;


        _custom = false;
        }
}
