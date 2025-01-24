using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public event Action OnRegenerateMesh;

    public void InvokeStructureUpdate()
    {
        if (OnRegenerateMesh==null) Debug.LogWarning("no subscribers to onregeneratemesh event");
        else
        {
            OnRegenerateMesh.Invoke();
        }
    }
}
