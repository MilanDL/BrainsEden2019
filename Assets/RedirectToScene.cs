using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sc =
        UnityEngine.SceneManagement.SceneManager;

public class RedirectToScene : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Sc.LoadScene(Sc.GetActiveScene().buildIndex + 1);
    }
}
