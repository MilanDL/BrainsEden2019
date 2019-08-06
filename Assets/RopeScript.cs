using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    private LineRenderer _line;
    [SerializeField] private Transform[] _points;
    [SerializeField] private Transform _mouse;
    [SerializeField] private FuseTrailScript _trail;
    [SerializeField] private Transform _handle;
    [SerializeField] private MeshRenderer _matToApplyTo;
    [SerializeField] private Material _mat;
    private bool _grabbed;
    private Transform _ribbon;
    
    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = _points.Length;
    }

    private void LateUpdate()
        {

        if (!_ribbon)
            _ribbon = GameObject.Find("RIBBONROOT")?.transform;
        else
            {
            if (_grabbed)
                {
                _ribbon.position = _mouse.position;
                }

            _handle.position = _ribbon.position;
            }

        //draw line
        _line.positionCount = _trail.GetMaxDrawInt() + 2;
        _line.SetPositions(_points.Select(x => x.position).ToArray());
        _line.SetPosition(_trail.GetMaxDrawInt() + 1, _trail.GetFurthestPoint());

        }

    public void Hold()
        {
        _grabbed = true;
         _matToApplyTo.material = _mat;
        }
    }