using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeArea : MonoBehaviour
{

    private MeshRenderer _mesh;
    private Color _clearCol;

    private void Start()
        {
        _mesh = GetComponent<MeshRenderer>();
        //_clearCol = _mesh.material.GetColor("_BaseColor");
        _clearCol.a = 0.05f;
        }

    private void Update()
        {
        //_mesh.material.SetColor("_BaseColor", Color.Lerp(_mesh.material.GetColor("_BaseColor"), _clearCol, 3 * Time.deltaTime));

        //if (_mesh.material.GetColor("_BaseColor").a <= 0.3f)
        //    Destroy(gameObject);
        }
    }
