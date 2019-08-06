using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour, IButton
{
    [SerializeField] private UnityEvent _onClick;

    public void OnClick()
        {
        _onClick.Invoke();
        }
    }

internal interface IButton
    {
    void OnClick();
    }