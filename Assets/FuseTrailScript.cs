using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuseTrailScript : MonoBehaviour
{
    SoundManager _soundManager;

    public enum FuseState
    {
        Idle,
        Burning,
        Trail,
        End
    }
    [HideInInspector] public FuseState State = FuseState.Idle;

    [SerializeField] private Transform[] _ropePoints;
    [SerializeField, Range(0, 0.9f)] private float _currRopePos = 0.9f;
    [SerializeField] private Transform _fuseObject;
    [SerializeField] private float _burnSpeed;
    [SerializeField] private GameObject _collector;
    [SerializeField] private Light _sun;
    [SerializeField] private Transform _fuseParticles;

    //trail
    private int _currIndex;
    [SerializeField] private TrailRenderer _fuseTrail;
    [SerializeField] private TrailRenderer _reverseTrail;
    [SerializeField] private float _trailSpeed;
    private List<int> _loopIndexes = new List<int>();
    private float _startIntensitySun;
    [SerializeField] private Material _areaMat;
    [SerializeField] private GameObject _sparkles;
    [SerializeField] private Light _fuseLight;

    // Start is called before the first frame update
    void Start()
    {
        _fuseTrail.emitting = false;
        _reverseTrail.emitting = false;

        _startIntensitySun = _sun.intensity;

        _soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _sun.intensity = Mathf.Lerp(_sun.intensity, _startIntensitySun, 8 * Time.deltaTime);

        if (State == FuseState.Trail)
        {
            if (!_soundManager.IsPlaying("fuze"))
                _soundManager.Play("fuze");

            _fuseObject.position = Vector3.MoveTowards(_fuseObject.position, _fuseTrail.GetPosition(_currIndex), _trailSpeed * Time.deltaTime);

            if (Vector3.Distance(_fuseObject.position, _fuseTrail.GetPosition(_currIndex)) < 0.02f)
            {
                _currIndex--;

                for (int i = 0; i < _loopIndexes.Count; i++)
                {
                    if (_loopIndexes[i] == _currIndex && LoopFinder.GetLoop(i).Count > 15) //is at loop
                    {
                        _soundManager.Play("explosion");
                        MeshRenderer mesh = LoopFinder.SpawnMeshOnLoop(i);
                        mesh.material = _areaMat;
                        mesh.gameObject.AddComponent<ExplodeArea>();
                        FixedTime.FreezeTime(0.1f);
                        Camera.main.GetComponent<ShakeScript>().Shake(0.05f);

                        foreach (Vector2 point in LoopFinder.GetLoop(i))
                        {
                            Instantiate(_sparkles).transform.position = point;
                        }

                        LoopFinder.FindGameObjectsInLoop(i, "PickUp", out List<GameObject> coins);
                        foreach (GameObject coin in coins) //find coins
                        {
                            FixedTime.FreezeTime(0.02f);
                            CollectAnim();
                            Camera.main.GetComponent<ShakeScript>().Shake(0.2f);
                            coin.GetComponent<PickUps>()?.PickupCoin();
                            coin.GetComponent<Barrel>()?.Explode();
                        }
                    }
                }

            }

            if (_currIndex <= 0)
            {
                TrailEnd();
                State = FuseState.End;
            }

        }

        _collector.SetActive(State == FuseState.Trail);


        if (State == FuseState.Burning)
        {
            _currRopePos = Mathf.MoveTowards(_currRopePos, 0, _burnSpeed * Time.deltaTime);
            _fuseObject.position = SampleRopePoint(_currRopePos);

            if (_currRopePos <= 0)
            {
                State = FuseState.Trail;
                BurnEnd();
                _currIndex = _fuseTrail.positionCount - 1;
                Vector3[] pos = new Vector3[_fuseTrail.positionCount];
                _fuseTrail.GetPositions(pos);
                List<Vector2> listPos = new List<Vector2>();
                listPos.AddRange(pos.Select(x => (Vector2)x));
                _loopIndexes = LoopFinder.FindLoops(listPos);
            }
        }

    }

    private Vector3 SampleRopePoint(float time)
    {
        if (time >= 1)
        {
            return _ropePoints[_ropePoints.Length - 1].position;
        }
        else
        {
            int leftIndex = (int)(time * _ropePoints.Length);
            Vector3 left = _ropePoints[leftIndex].position;
            Vector3 right = _ropePoints[leftIndex + 1].position;
            return Vector3.Lerp(left, right, (time - ((float)leftIndex / _ropePoints.Length)) * _ropePoints.Length);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(SampleRopePoint(_currRopePos), 0.1f);
    }

    public int GetMaxDrawInt()
    {
        return (int)(_currRopePos * _ropePoints.Length);
    }

    public Vector3 GetFurthestPoint()
    {
        return SampleRopePoint(_currRopePos);
    }

    public void StartBurn()
    {
        if (State == FuseState.Idle)
        {
            if (!_soundManager.IsPlaying("fuze"))
                _soundManager.Play("fuze");
            State = FuseState.Burning;
            foreach (ParticleSystem part in _fuseParticles.GetComponentsInChildren<ParticleSystem>())
            {
                part.Play();
            }
            _fuseLight.enabled = true;
            _fuseTrail.Clear();
            _fuseTrail.emitting = true;
        }
    }

    private void BurnEnd()
    {
        _soundManager.Stop("fuze");
        _soundManager.Play("fuzeend");
        _fuseTrail.emitting = false;
        _reverseTrail.emitting = true;
        _fuseLight.enabled = false;
    }

    private void TrailEnd()
    {
        _soundManager.Play("fuzeend");
        _soundManager.Stop("fuze");
        foreach (ParticleSystem part in _fuseParticles.GetComponentsInChildren<ParticleSystem>())
        {
            part.Stop();
        }
        _fuseLight.enabled = false;
    }

    public void CollectAnim(bool bad = false)
    {
        Camera.main.GetComponent<ShakeScript>().Shake(0.1f);
        if (!bad)
            _sun.intensity = _startIntensitySun + 20;
    }
}
