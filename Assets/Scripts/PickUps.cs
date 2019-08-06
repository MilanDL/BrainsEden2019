using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUps : MonoBehaviour
{
    SoundManager _soundManager;

    public bool Bad;

    public int score = 10;
    ScoreManager scoreManager;
    PickUpSpawner pickUpSpawner;
    private bool _pickup;
    public UnityEvent GotPickedUp = new UnityEvent();

    [SerializeField] private Vector3 _targetPos;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private Transform _model;
    [SerializeField] private bool _circledOnly = false;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        _soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickup)
        {
            if (Bad)
            {
                _spinSpeed = 0;
                _model.localEulerAngles = Vector3.zero;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _targetPos, 8 * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, 4 * Time.deltaTime);

                if (Vector3.Distance(transform.position, _targetPos) <= 0.2f)
                {
                    GotPickedUp.Invoke();
                    Destroy(gameObject);
                }
            }

        }

        _model.localEulerAngles += Vector3.up * _spinSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trail"))
        {
            if (!_circledOnly)
            {
                collision.transform.GetComponent<SparkCircleScript>().Pickup();
                collision.transform.root.GetComponentInChildren<FuseTrailScript>().CollectAnim(Bad);
                PickupCoin();
            }
        }

        if (collision.gameObject.GetComponent<Barrel>())
        {
            PickupCoin();
        }
    }

    public void PickupCoin()
    {
        _soundManager?.Play("coin", false);
        Destroy(GetComponent<Collider2D>());
        _pickup = true;
        transform.localScale += Vector3.one * 0.4f;

        if (Bad)
        {
            Camera.main.transform.root.GetComponentInChildren<CameraScript>().Zoom(transform.position);
        }
    }
}
