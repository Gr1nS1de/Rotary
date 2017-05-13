using System;
using UnityEngine;
using System.Collections;

public static class CustomEventDelegate
{
    public static Action<CustomEvent> ActionCustomEvent = delegate { };
    public static void OnEvent(CustomEvent cEvent)
    {
        ActionCustomEvent(cEvent);
    }

}
