using UnityEngine;

public class TimerManager
{
    public static TimerBehaviour GetTimer(GameObject target)
    {
        return target.GetOrAddComponent<TimerBehaviour>();
    }
}