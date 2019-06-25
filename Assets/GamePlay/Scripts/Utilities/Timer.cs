using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CompleteEvent();
public delegate void UpdateEvent(float t);

public class Timer : MonoBehaviour
{
	UpdateEvent updateEvent;
	CompleteEvent onTimerCompleted;

	public bool isLog = true;
	public bool isTimerStart = false;
	public bool isTiming = false;
	public bool isTimerEnd = true;
	public bool isTimerDestoryAfterTiming = true;
	public bool isIgnoreTimeScale = true;
	public bool isRepeate = false;

	float timerTarget;
	float timerStartTime;
	float timerOffset;

	float nowTime;
	float now;
	float Time_ // total time from game start
	{
		get {return isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;}
	}

	public void startTiming(float timerTarget_, CompleteEvent onTimerCompleted_, UpdateEvent updateEvent_ = null,
		bool isIgnoreTimeScale_ = true, bool isRepeate_ = false, bool isDestory_ = true)
	{
		timerTarget = timerTarget_;
		if(onTimerCompleted_ != null)
			onTimerCompleted = onTimerCompleted_;
		if(updateEvent_ != null)
			updateEvent = updateEvent_;

		isTimerDestoryAfterTiming = isDestory_;
		isIgnoreTimeScale = isIgnoreTimeScale_;
		isRepeate = isRepeate_;

		timerStartTime = Time_;
		timerOffset = 0;
		isTimerStart = true;
		isTiming = true;
        this.gameObject.SetActive(true);
	}

	public static Timer createTimer(string timerName = "Timer")
	{
		GameObject g = new GameObject(timerName);
		Timer timer = g.AddComponent<Timer>();
		return timer;
	}

    // Update is called once per frame
    void Update()
    {
        if(isTiming)
        {
        	nowTime = Time_ - timerOffset;
        	now = nowTime - timerStartTime;

        	if(updateEvent != null)
        	{
        		updateEvent(Mathf.Clamp01(now/timerTarget));
        	}
        	if(now > timerTarget)
        	{
        		if(onTimerCompleted != null)
        			onTimerCompleted();
                if(isRepeate)
                    RestartTimer();
        		if(isTimerDestoryAfterTiming)
        			Destroy();
                else
                    EndTiming();
        	}
        }
    }

    public float GetLeftTime()
    {
    	return Mathf.Clamp(timerTarget - now, 0, timerTarget);
    }

    void OnApplicationPause(bool isPause_)
    {
    	if(isPause_)
    		PauseTimer();
    	else
    		ContinueTimer();
    }

    public void PauseTimer()
    {
        if (isTimerEnd)
        {
            if (isLog) Debug.LogWarning("pauseTimer() -> Timing End!");
        }
        else
        {
            isTiming = false;
            timerOffset = Time_;
        }
    }

    public void ContinueTimer()
    {
        if (isTimerEnd)
        {
            if (isLog) Debug.LogWarning("continueTimer() -> Timing End!");
        }
        else
        {
            if (!isTiming)
            {
                timerOffset = Time_ - timerOffset;
                isTiming = true;
            }
        }
    }

    public void RestartTimer()
    {
    	timerStartTime = Time_;
    	timerOffset = 0;
        isTimerStart = true;
        isTiming = true;
    }

    public void RestartTimerForCombo()
    {
        this.gameObject.SetActive(true);
        timerStartTime = Time_;
        timerOffset = 0;
        isTimerStart = true;
        isTiming = true;
    }

    public void increaseTimerTarget(float targetIncrement)
    {
    	timerTarget += targetIncrement;
    }

    public void EndTiming()
    {
        isTimerStart = false;
        isTiming = false;
        isTimerEnd = true;
    }

    public void Destroy()
    {
    	isTimerStart = false;
    	isTiming = false;
    	isTimerEnd = true;
    	if(isTimerDestoryAfterTiming)
    		Destroy(gameObject);
    }













}
