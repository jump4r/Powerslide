using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener
{
    public void createListener<T>(ref T referenceObject, string referece)
    {
        
    }
}
public static class EventDelegator {
    public static List<EventListener> listeners = new List<EventListener>();

}
