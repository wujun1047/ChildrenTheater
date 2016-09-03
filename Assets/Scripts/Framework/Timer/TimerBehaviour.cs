using UnityEngine;
using System;


/// <summary>
/// 每秒定时器
/// 功能：每秒执行onTimer，seconds秒后停止执行，并执行callback回调。
/// </summary>
public class TimerBehaviour : MonoBehaviour
{
    private uint m_seconds = 0;
    private Action<uint> m_onTimer = null;
    private Action m_callback = null;
    public void StartTimer(uint seconds, Action<uint> onTimer, Action callback)
    {
        m_seconds = seconds;
        m_onTimer = onTimer;
        m_callback = callback;


        if (m_seconds > 0)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
            if (m_callback != null)
            {
                m_callback();
            }
        }
    }

    public uint GetSeconds()
    {
        return m_seconds;
    }

    private bool timerIsActive = false;
    private float timeActive = 0f;

    public void StartTimer()
    {
        timerIsActive = true;
        timeActive = 0f;
    }
    public void StopTimer()
    {
        timerIsActive = false;
        timeActive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsActive)
        {
            timeActive += Time.deltaTime * 1000;
            if (timeActive > 1000)
            {
                timeActive = 0f;
                m_seconds--;
                if (m_seconds > 0)
                {
                    if (m_onTimer != null)
                    {
                        m_onTimer(m_seconds);
                    }
                }
                else
                {
                    StopTimer();
                    if (m_callback != null)
                    {
                        m_callback();
                    }
                }
            }

        }
    }

    public float GetCurrentTime()
    {
        return timeActive;
    }

    public string GetCurrentTimeString()
    {
        return timeActive.ToString("00.00");
    }
    public bool IsTimerRunning()
    {
        return timerIsActive;
    }

    public void OnDisable()
    {
        if (IsTimerRunning())
        {
            StopTimer();
        }
    }
}