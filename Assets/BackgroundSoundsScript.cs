using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sc = UnityEngine.SceneManagement.SceneManager;

public class BackgroundSoundsScript : MonoBehaviour
{
    SoundManager _soundManager;

    bool _hasBegun = false;
    float _secondsToWait = 10;

    static BackgroundSoundsScript instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _soundManager = GetComponent<SoundManager>();

        _soundManager.Play("foley", true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Sc.GetActiveScene().buildIndex > 1)
            {
            if (!_soundManager.IsPlaying("tune"))
                _soundManager.Play("tune", true);
            }
        else
            _soundManager.Stop("tune");

        if (false) // people sounds, but i don't like how it sounds so no. maybe if we change them
        {
            if (!_soundManager.IsPlaying("people") && _secondsToWait < 0)
            {
                _soundManager.Play("people", false);
                _secondsToWait = 10;
            }

            _secondsToWait -= Time.deltaTime;
        }
    }

}
