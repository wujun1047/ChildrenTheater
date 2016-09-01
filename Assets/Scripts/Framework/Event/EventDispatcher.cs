using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventDispatcher : Singleton<EventDispatcher>
{
    // 消息路由表
    Dictionary<string, Action<EventArgs>> _router = new Dictionary<string, Action<EventArgs>>();

    public Dictionary<string, Action<EventArgs>> Router
    {
        get { return _router; }
    }

    public void Clear()
    {
        _router.Clear();
    }

    void _CheckAddingListenerParam(string eventType, Delegate delegateToAdd)
    {
        if (!_router.ContainsKey(eventType))
        {
            _router.Add(eventType, null);
        }

        Delegate d = _router[eventType];
        if (d != null && d.GetType() != delegateToAdd.GetType())
        {
            throw new Exception(string.Format(
                    "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                    eventType, d.GetType().Name, delegateToAdd.GetType().Name));
        }
    }

    bool _CheckRemoveListenerParam(string eventType, Delegate delegateToRemove)
    {
        if (!_router.ContainsKey(eventType))
            return false;

        Delegate d = _router[eventType];
        if ((d != null) && (d.GetType() != delegateToRemove.GetType()))
        {
            throw new Exception(string.Format(
                "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                eventType, d.GetType(), delegateToRemove.GetType()));
        }
        else
            return true;
    }

    public void AddListener(string eventType, Action<EventArgs> handler)
    {
        _CheckAddingListenerParam(eventType, handler);
        _router[eventType] += handler;
    }

    public void RemoveListener(string eventType, Action<EventArgs> handler)
    {
        if (_CheckRemoveListenerParam(eventType, handler))
        {
            _router[eventType] -= handler;
            if (_router[eventType] == null)
            {
                _router.Remove(eventType);
            }
        }
    }

    public void TriggerEvent(EventArgs args)
    {
        Action<EventArgs> handler;
        if (!_router.TryGetValue(args.eventType, out handler))
        {
            return;
        }

        var callbacks = handler.GetInvocationList();
        for (int i = 0; i < callbacks.Length; ++i)
        {
            Action<EventArgs> callback = callbacks[i] as Action<EventArgs>;
            
            if (callback == null)
                throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", args.eventType));

            try
            {
                callback(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("TriggerEvent {0} error: ", args.eventType) + e.Message);
                throw e;
            }
        }
    }
}
