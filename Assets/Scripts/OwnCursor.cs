using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnCursor : MonoBehaviour
{
    public Vector2 speed;
    public Joystick joystick;

    void Start()
    {
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        float moveHorizontal = joystick.Horizontal;
        Debug.Log(moveHorizontal);
        float moveVertical = joystick.Vertical;
        Debug.Log(moveVertical);

        Vector3 movement = new Vector3(speed.x * moveHorizontal, speed.y * moveVertical, 0);
        movement *= Time.deltaTime;
        transform.Translate(movement);
    }
}