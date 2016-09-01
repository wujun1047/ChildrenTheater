public sealed class TimerHelper : DDOLSingleton<TimerHelper>
{
    void Update()
    {
        TimerHeap.Tick();
        FrameTimerHeap.Tick();
    }
}
