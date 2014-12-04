using UnityEngine;
using System.Collections;

public class Timer
{
    private float currentTime;
    private float endTime;

    public Timer(float timerEndTime)
    {
        endTime = timerEndTime;
        Reset();
    }

	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	public void Update () 
    {
        currentTime += Time.deltaTime;
	}

    public bool HasElapsed()
    {
        return currentTime >= endTime;
    }

    public void Reset()
    {
        currentTime = 0.0f;
    }
}
