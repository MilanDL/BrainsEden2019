using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileScript : MonoBehaviour {

    [SerializeField] BoxCollider2D _coll;
    [SerializeField] SpriteRenderer _rend;

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(Mathf.Round(transform.position.x * 2)/2, Mathf.Round(transform.position.y * 2)/2, Mathf.Round(transform.position.z * 2)/2);
        if (_rend != null && _coll != null)
            {
            _rend.size = new Vector2(Mathf.Round(_rend.size.x), Mathf.Round(_rend.size.y));
            _coll.size = _rend.size;
            }
        }
}
