using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private GameEventListener[] eventListeners = new GameEventListener[0];

    public void Raise()
    {
        for (int i = eventListeners.Length - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (Array.IndexOf(eventListeners, listener) == -1)
        {
            // If the listener is not in the array, resize the array and add the listener
            Array.Resize(ref eventListeners, eventListeners.Length + 1);
            eventListeners[eventListeners.Length - 1] = listener;
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        int index = Array.IndexOf(eventListeners, listener);
        if (index != -1)
        {
            // If the listener is in the array, remove it by creating a new array
            GameEventListener[] newArray = new GameEventListener[eventListeners.Length - 1];
            Array.Copy(eventListeners, 0, newArray, 0, index);
            Array.Copy(eventListeners, index + 1, newArray, index, newArray.Length - index);
            eventListeners = newArray;
        }
    }
}
