using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    SoundManager _soundManager;

    CircleCollider2D coll;
    [SerializeField] private GameObject _explosionParticle;
    public ParticleSystem particleLanding;
    [SerializeField] private Transform _model;
    private CircleScript _circle;
    private float _startRad;
    bool exploded;
    [SerializeField] bool arrived;

    enum State
    {
        falling,
        set
    }

    State currentState;

    Vector3 initialPosition;
    Vector3 endPosition;
    float startTime;
    float distanceToTravel;
    public float speed = 25;

    Vector3 shakeRot;
    public Vector3 originalRot = new Vector3(90, 0, 0);
    public float shakeTime = 1;
    public float amount = 1;
    public float shakeSpeed = 25;
    private bool _once;

    private void Awake()
    {
        _circle = GetComponentInChildren<CircleScript>();
        _startRad = _circle.xradius;
        _circle.xradius = 0;
        _circle.yradius = 0;
        coll = GetComponent<CircleCollider2D>();

        if (!coll)
        {
            Debug.LogError("ERROR! Did not find circleCollider collider");
        }

        //_explosionParticle = GetComponent<ParticleSystem>();

        currentState = State.set;

        transform.Rotate(90, 0, 0);

    }

    private void Start()
    {
        exploded = false;
        arrived = false;

        initialPosition = transform.position;
        initialPosition.z -= 20;
        endPosition.Set(transform.position.x, transform.position.y, 0);

        startTime = Time.time;

        distanceToTravel = Vector3.Distance(initialPosition, endPosition);

        _soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

        float distCovered = (Time.time - startTime) * speed;
        float fraction = distCovered / distanceToTravel;
        transform.position = Vector3.Lerp(initialPosition, endPosition, fraction);


        if (Vector3.Distance(transform.position, endPosition) <= 0.001f && !arrived)
        {
            arrived = true;
            Debug.Log("ARRIVED");
            //Instantiate(particleLanding, endPosition, Quaternion.identity);
            particleLanding.Play();
        }

        if (arrived && shakeTime > 0)
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(
                originalRot.x + Mathf.Cos(Time.time * shakeSpeed) * amount,
                originalRot.y + Mathf.Cos(Time.time * shakeSpeed) * amount,
                originalRot.z + Mathf.Cos(Time.time * shakeSpeed) * amount));
            shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(originalRot));
        }

        if (arrived)
        {
            float newRad = Mathf.Lerp(_circle.xradius, exploded ? 0 : _startRad, 5 * Time.deltaTime);
            _circle.xradius = newRad;
            _circle.yradius = newRad;
        }


        if (exploded)
        {
            if (_model.transform.localScale.magnitude <= 0.02f && !_once)
            {
                _once = true;
                Invoke("Terminate", 3);
            }
            _model.transform.localScale = Vector3.MoveTowards(_model.transform.localScale, Vector3.zero, 4 * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trail") && !exploded && currentState != State.falling)
        {
            Explode();
            Debug.Log("Explode barrel");
        }

        if (collision.gameObject.CompareTag("Background"))
        {
            currentState = State.set;
        }

        if (collision.gameObject.GetComponent<Barrel>() && collision.gameObject != gameObject)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (!_soundManager.IsPlaying("explodedebris"))
            _soundManager.Play("explodedebris", false);
        coll.radius = 3;
        exploded = true;
        _explosionParticle.SetActive(true);
        //Invoke("Terminate", 1);
        FixedTime.FreezeTime(0.2f);
        transform.GetComponentInChildren<Squishy>().Squish();
        _circle.GetComponent<LineRenderer>().SetWidth(0.3f, 0.3f);
        GetComponentInChildren<Light>().enabled = false;
    }

    void Terminate()
    {
        Destroy(gameObject);
    }


}
