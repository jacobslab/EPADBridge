using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class ClockSynchronization : MonoBehaviour
{
    public GameClock gameClock;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameClock.GetClockMilliseconds(GetNetworkTime()) - GameClock.GetClockMilliseconds(DateTime.Now));
    }

    public long[] RunSyncInterval()
    {
             long[] resultArr = new long[2]; // it will contain NTP time and local time

        EPADApplication.Instance.timeSyncLog.LogNTPSyncEvent(true);
        gameClock._ntpSync.QueryNTPTime();
        DateTime currentNTPTime = gameClock._ntpSync.lastSyncedNTPTime;
        EPADApplication.Instance.timeSyncLog.LogNTPSyncEvent(false);
        long ntpTime = GameClock.GetClockMilliseconds(currentNTPTime);
            long localTime = GameClock.GetClockMilliseconds(DateTime.Now);
            long difference = ntpTime - localTime;

        Debug.Log("difference is " + difference.ToString());
        resultArr[0] = ntpTime;
        resultArr[1] = localTime;

        return resultArr;
            //NetworkManager.Instance.SendMessageToEPAD(message);
            //TreasureHuntController_ARKit.Instance.trialLog.LogTimeSyncEvent(ntpTime, localTime, difference);

    }
}
