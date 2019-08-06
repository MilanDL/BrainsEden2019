using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnCursor : MonoBehaviour
{

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 direction = new Vector2(0, 1);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {


        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 16));
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("PickUp"))
    //    {
    //        Debug.Log("YES");
    //    }
    //    else
    //    {
    //        Debug.Log("NO");

    //    }
    //}

    private void OnGUI()
    {
        // GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 32, 32), cursorTexture);
    }

    void OnMouseEnter()
    {
        // Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        //Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

}