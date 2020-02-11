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

    public IEnumerator RunSyncInterval()
    {
             long[] resultArr = new long[2]; // it will contain NTP time and local time

        EPADApplication.Instance.timeSyncLog.LogNTPSyncEvent(true);
        gameClock._ntpSync.QueryNTPTime();
        yield return new WaitForSeconds(0.1f);
        while(!gameClock._ntpSync.didUpdateNTP)
        {
            yield return 0;
        }

        DateTime currentNTPTime = gameClock._ntpSync.lastSyncedNTPTime;
        EPADApplication.Instance.timeSyncLog.LogNTPSyncEvent(false);
        long ntpTime = GameClock.GetClockMilliseconds(currentNTPTime);
            long localTime = GameClock.GetClockMilliseconds(DateTime.Now);
            long difference = ntpTime - localTime;
        EPADApplication.Instance.debugText.text = "Last timestamp: " + ntpTime.ToString();
        Debug.Log("difference is " + difference.ToString());
        resultArr[0] = ntpTime;
        resultArr[1] = localTime;
        EPADApplication.Instance.timeSyncLog.LogEPADSyncTime(resultArr);
        yield return null;
        //return resultArr;
            //NetworkManager.Instance.SendMessageToEPAD(message);
            //TreasureHuntController_ARKit.Instance.trialLog.LogTimeSyncEvent(ntpTime, localTime, difference);

    }
}
