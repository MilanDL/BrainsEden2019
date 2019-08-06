using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShakeMode {
    CanvasSpace,
    WorldSpace,
    LocalSpace
    }

public class ShakeScript : MonoBehaviour
{
    [Header("Shake Properties")]
    [SerializeField] private float _shakePower;
    [SerializeField] private float _shakeDampen;
    private float _savedDampen;

    [Header("Settings")]
    [SerializeField] private ShakeMode _mode = ShakeMode.LocalSpace;
    [SerializeField] private bool _increasesOnShake = true;
    [SerializeField] private bool _shakeOnAwake;
    [SerializeField] private bool _freezeLess;

    private RectTransform _rect;
    private Vector3 _startPos;
    private float _shake;

    private void Awake()
        {
        _savedDampen = _shakeDampen;
        }

    // Start is called before the first frame update
    void Start()
        {
        InitializeShakeMode();
        if (_shakeOnAwake)
            Shake();
        }
    
    private void InitializeShakeMode()
        {
        switch (_mode)
            {
            case ShakeMode.CanvasSpace:
                _rect = GetComponent<RectTransform>();
                _startPos = _rect.anchoredPosition;
                break;
            case ShakeMode.WorldSpace:
                _startPos = transform.position;
                break;
            case ShakeMode.LocalSpace:
                _startPos = transform.localPosition;
                break;
            default:
                break;
            }
        }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0 || _freezeLess)
            {
            ApplyShake();
            ReduceShake();
            }
    }

    private void ApplyShake()
        {
        Vector3 shake = new Vector3(UnityEngine.Random.Range(-_shake, _shake), UnityEngine.Random.Range(-_shake, _shake), UnityEngine.Random.Range(-_shake, _shake));

        switch (_mode)
            {
            case ShakeMode.CanvasSpace:
                shake.z = 0;
                _rect.anchoredPosition = _startPos + shake;
                break;
            case ShakeMode.WorldSpace:
                transform.position = _startPos + shake;
                break;
            case ShakeMode.LocalSpace:
                transform.localPosition = _startPos + shake;
                break;
            default:
                break;
            }
        }

    private void ReduceShake()
        {
        _shake = Mathf.Lerp(_shake, 0, _shakeDampen * (_freezeLess ? FixedTime.FreezelessDeltaTime : Time.deltaTime));
        }

    /// <summary>
    /// Shakes the object
    /// </summary>
    /// <param name="power">Define a custom power</param>
    public void Shake(float power = -1, float dampen = -1)
        {
        //set dampen
        if (dampen < 0)
            _shakeDampen = _savedDampen;
        else
            _shakeDampen = dampen;

        //set power
        if (power < 0)
            power = _shakePower;
        _shake = power + (_increasesOnShake ? _shake : 0);
        }

    /// <summary>
    /// Changes the pivot the object shakes around.
    /// </summary>
    /// <param name="pivot">The new pivot to change to.</param>
    /// <param mode="pivot">In what space the new pivot is.</param>
    public void SetShakePivot(Vector3 pivot, ShakeMode mode)
        {
        _mode = mode;
        InitializeShakeMode();
        _startPos = pivot;
        }
    
    }
