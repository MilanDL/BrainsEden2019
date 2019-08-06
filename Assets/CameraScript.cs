using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Sc = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    SoundManager _soundManager;

#pragma warning disable 649
    [SerializeField] private float _zoomOnLose;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _timeTillTransition;
    [SerializeField] private int _coinsNeededToWin = 3;
    [SerializeField] private FuseTrailScript _trail;

    [SerializeField] private Text _coinText;
    [SerializeField] private Text _levelText;

    private Camera _cam;
    private Vector3 _zoomTarget;
    private bool _zoom;
    private Vector3 _startPos;
    [SerializeField] private Volume _transition;


    [Header("Light Parameters")]
    [SerializeField] private float _directionalIntensitySTART;
    [SerializeField] private Color _directionalColorSTART;
    [SerializeField] private Vector3 _directionalRotationSTART;
    [SerializeField] private float _areaLightIntensitySTART;
    [SerializeField] private Color _areaLightColorSTART;

    [SerializeField] private float _directionalIntensityEND;
    [SerializeField] private Color _directionalColorEND;
    [SerializeField] private Vector3 _directionalRotationEND;
    [SerializeField] private float _areaLightIntensityEND;
    [SerializeField] private Color _areaLightColorEND;

    [SerializeField] private Color _directionalColorMIDDLE;
    [SerializeField] private Color _areaLightColorMIDDLE;

    [SerializeField] private float _secondsToLerpToMax = 30.0f;
    [SerializeField] private int _maxLevels = 10;
    private int _myId = 0;

    private Light _directionalLight;
    private Light _areaLight;

    private float _accuSceneSec = 0.0f;


    private float _transitionTimer;
    private bool _doTransition;
    private int _targetBuildIndex;

    private List<PickUps> _coins;
    private bool _win;
    private int _amtCoinsPickedUp = 0;
    private int _amtTotalGoodCoins = 0;
#pragma warning restore 649

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = GetComponent<SoundManager>();
        _soundManager.Play("up");

        _directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        _areaLight = GameObject.Find("Area Light").GetComponent<Light>();

        _myId = Sc.GetActiveScene().buildIndex;

        _coins = GameObject.FindGameObjectsWithTag("PickUp").Where(x => !x.GetComponent<Barrel>()).Select(x => x.GetComponent<PickUps>()).ToList();
        _cam = GetComponentInChildren<Camera>();
        _startPos = transform.position;
        FixedTime.Pause(false);
        _transition.weight = 1;

        _transitionTimer = _timeTillTransition;


        // HUD setup
        foreach (PickUps coin in _coins)
        {
            if (coin)
            {
                if (!coin.Bad)
                {
                    _amtTotalGoodCoins++;
                    coin.GotPickedUp.AddListener(CoinPickedUp);
                }
            }
        }

        // Light Setup
        float delta = (float)_myId / _maxLevels;

        _directionalLight.intensity = Mathf.Lerp(_directionalIntensitySTART, _directionalIntensityEND, delta);
        if (_myId < _maxLevels / 2)
            _directionalLight.color = Color.Lerp(_directionalColorSTART, _directionalColorMIDDLE, delta);
        else
            _directionalLight.color = Color.Lerp(_directionalColorMIDDLE, _directionalColorEND, delta);
        Quaternion newRot = new Quaternion();
        newRot.eulerAngles = (Vector3.Lerp(_directionalRotationSTART, _directionalRotationEND, delta));
        _directionalLight.transform.rotation = newRot;

        _areaLight.intensity = Mathf.Lerp(_areaLightIntensitySTART, _areaLightIntensityEND, delta);
        if (_myId < _maxLevels / 2)
            _areaLight.color = Color.Lerp(_areaLightColorSTART, _areaLightColorMIDDLE, delta);
        else
            _areaLight.color = Color.Lerp(_areaLightColorMIDDLE, _areaLightColorEND, delta);

        // More HUD
        _levelText.text = "Level " + _myId + " / " + _maxLevels;
    }




    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Sc.LoadScene(Sc.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.Escape))
            Sc.LoadScene(1);

        if (_zoom)
            _cam.transform.position = Vector3.Lerp(_cam.transform.position, _zoomTarget + Vector3.back * _zoomOnLose, _zoomSpeed * FixedTime.FreezelessDeltaTime);

        _transition.weight = Mathf.Lerp(_transition.weight, _transitionTimer <= 0 && _doTransition ? 1 : 0, 6 * FixedTime.FreezelessDeltaTime);

        if (_doTransition)
        {
            _transitionTimer -= FixedTime.FreezelessDeltaTime;
            if (_transitionTimer <= 0)
            {
                int currIndex = Sc.GetActiveScene().buildIndex;
                if (_transition.weight >= 0.99f)
                    {
                    if (_zoom)
                        Sc.LoadScene(Sc.GetActiveScene().buildIndex);
                    else
                        Sc.LoadScene(_win ? (currIndex >= 11 ? 1 : currIndex + 1) : Sc.GetActiveScene().buildIndex);
                    }
            }
        }

        if (_coins.Where(x => x == null).ToArray().Length == _coinsNeededToWin)
        {
            _win = true;
        }

        if (_trail.State == FuseTrailScript.FuseState.End)
            Transition();

        // HUD
        _coinText.text = _amtCoinsPickedUp.ToString() + " / " + _coinsNeededToWin.ToString();


        // Light
        _accuSceneSec += Time.deltaTime;
        float delta = 0.0f;

        if (_accuSceneSec >= _secondsToLerpToMax)
        {
            _accuSceneSec = _secondsToLerpToMax;

            delta = (float)(_myId + 1) / _maxLevels;
        }
        else
            delta = (_myId + _accuSceneSec / _secondsToLerpToMax) / _maxLevels;

        _directionalLight.intensity = Mathf.Lerp(_directionalIntensitySTART, _directionalIntensityEND, delta);
        if (_myId < _maxLevels / 2)
            _directionalLight.color = Color.Lerp(_directionalColorSTART, _directionalColorMIDDLE, delta);
        else
            _directionalLight.color = Color.Lerp(_directionalColorMIDDLE, _directionalColorEND, delta);
        Quaternion newRot = new Quaternion();
        newRot.eulerAngles = (Vector3.Lerp(_directionalRotationSTART, _directionalRotationEND, delta));
        _directionalLight.transform.rotation = newRot;

        _areaLight.intensity = Mathf.Lerp(_areaLightIntensitySTART, _areaLightIntensityEND, delta);
        if (_myId < _maxLevels / 2)
            _areaLight.color = Color.Lerp(_areaLightColorSTART, _areaLightColorMIDDLE, delta);
        else
            _areaLight.color = Color.Lerp(_areaLightColorMIDDLE, _areaLightColorEND, delta);
    }

    private void CoinPickedUp()
    {
        _amtCoinsPickedUp++;
    }


    internal void Zoom(Vector3 position)
    {
        _zoomTarget = position;
        _zoom = true;
        FixedTime.Pause(true);
        Transition();
        GetComponent<ShakeScript>().Shake();
        _targetBuildIndex = Sc.GetActiveScene().buildIndex;
    }

    public void Transition()
    {
        if (!_doTransition)
        {
            _soundManager.Play("down");
            _doTransition = true;
            _targetBuildIndex = _win ? Sc.GetActiveScene().buildIndex + 1 : Sc.GetActiveScene().buildIndex;
        }
    }

}

