using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// Possible updates:
/// -Get the tick to start for each object when they subscribe, as currently if anything joins in the middle of
/// a tick the time isn't consistent. Possible solution is to reset the tick on a scene change to give a bigger window  
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

public class TickTimeTimer : MonoBehaviour
{
    //Creating the event args for the the event
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }

    //This allows for a check of the current number of event subscriptions
    public static int GetOnTickSubCount()
    {
        return OnTick.GetInvocationList().Length;
    }

    //Creating the Event
    public static event EventHandler<OnTickEventArgs> OnTick, OnTick_OneSec;

    const float TickTimerMax = .2f; // 200 milliseconds, 1/5 second

    int totalTick;
    float tickTimer;

    void Awake()
    {
        totalTick = 0;
    }

    void Update()
    {
        //This is how the constant tick handles so there is a constant time between each tick.
        tickTimer += Time.deltaTime;
        if (tickTimer >= TickTimerMax)
        {
            //Tick 5 times per second
            tickTimer -= TickTimerMax;
            totalTick++;
            if (OnTick != null) OnTick(this, new OnTickEventArgs {tick = totalTick});
            if (OnTick_OneSec != null && totalTick % 5 == 0) {OnTick_OneSec(this, new OnTickEventArgs {tick = totalTick});}
        }
    }
}
