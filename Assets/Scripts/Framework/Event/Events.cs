using UnityEngine;

public static class Events
{
    public static class NetworkEvent
    {
        public readonly static string S2C_SvrIP = ProtocolID.S2C_SvrIP.ToString();
        public readonly static string S2C_ChangeToAR = ProtocolID.S2C_ChangeToAR.ToString();
        public readonly static string S2C_ChangeToVR = ProtocolID.S2C_ChangeToVR.ToString();
        public readonly static string C2S_EnterAR = ProtocolID.C2S_EnterAR.ToString();
        public readonly static string C2S_EnterVR = ProtocolID.C2S_EnterVR.ToString();
    }

    public static class GameEvent
    {
        public readonly static string GameStart = "GameStart";
        public readonly static string GameOver = "GameOver";
        public readonly static string UpdateScore = "UpdateScore";
        public readonly static string SelectedFoodChanged = "SelectedFoodChanged";
        public readonly static string FoodItemCooldownComplete = "FoodItemCooldownComplete";
        public readonly static string ThrowFoodBegin = "ThrowFoodBegin";
        public readonly static string ThrowFoodFinish = "ThrowFoodFinish";
    }
}

public class EventArgs
{
    public string eventType;
    public EventArgs(string type)
    {
        eventType = type;
    }
    public EventArgs() { }
}

public class EventArgs_Int : EventArgs
{
    public int nValue;
}

public class EventArgs_FoodType : EventArgs
{
    public eFoodType eFoodType;
}

public class NetworkEventArgs : EventArgs
{
    public ProtocolID id;
}

public class NetworkEventArgs_S2C_SvrIP : NetworkEventArgs
{
    public string svrIP;
}


public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);
public enum EnumTouchEventType
{
    OnClick,
    OnDoubleClick,
    OnDown,
    OnUp,
    OnEnter,
    OnExit,
    OnSelect,
    OnUpdateSelect,
    OnDeSelect,
    OnDrag,
    OnDragEnd,
    OnDrop,
    OnScroll,
    OnMove,
}
