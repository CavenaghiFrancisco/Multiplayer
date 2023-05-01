using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerOut
{
    private float timer = 0;
    private float timeOutMax = 4;
    
    public float Timer
    {
        get { return timer; }
    }

    public void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    public void ResetTimer()
    {
        Debug.Log("Reset");
        timer = 0;
    }

    public bool IsTimeOut()
    {
        return timer > timeOutMax;
    }


}
