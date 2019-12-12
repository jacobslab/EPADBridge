using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSyncLog : LogTrack 
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LogIPADSyncTime(string[] ipadtime)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, "IPAD_TIME" + separator +  ipadtime[0] + separator + ipadtime[1]);
    }
    public void LogEPADSyncTime(long[] epadtime)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, "EPAD_TIME" + separator + epadtime[0].ToString() + separator + epadtime[1].ToString());
    }

    public void LogNTPSyncEvent(bool hasStarted)
    {
        subjectLog.Log(GameClock.SystemTime_Milliseconds, "NTP_SYNC_EVENT" + separator + (hasStarted ? "STARTED" : "ENDED"));
    }
}

