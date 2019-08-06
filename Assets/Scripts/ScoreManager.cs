using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    static float score = 0;

    Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();

        if (!scoreText)
        {
            Debug.LogError("Error! Couldn't find score Text");
        }
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int scoreValue)
    {
        score += scoreValue;
        Debug.Log("Added " + scoreValue + " score");
        Debug.Log("Total " + score);
    }

}
