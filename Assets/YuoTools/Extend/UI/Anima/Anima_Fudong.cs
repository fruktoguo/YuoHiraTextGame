using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Anima_Fudong : MonoBehaviour
{
    public float Life = 4;
    public float offset = 0;
    
    public Vector3 TargetOffset;
    public Vector3 TargetScale;
    private Vector3 DefPos;
    private Vector3 DefScale;
    

    void Start()
    {
        DefPos = transform.position;
        DefScale = transform.localScale;
        timer = offset;
    }

    private float timer;
    private int speed = 1;
    void Update()
    {
        timer += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(DefPos, DefPos + TargetOffset, timer /Life);
        transform.localScale = Vector3.Lerp(DefScale, DefScale + TargetScale, timer / Life);
        if (speed == 1 && timer > Life)
        {
            timer = Life;
            speed = -1;
        }
        else if (speed == -1 && timer < 0)
        {
            timer = 0;
            speed = 1;
        }
    }
}