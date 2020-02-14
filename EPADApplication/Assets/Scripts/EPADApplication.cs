using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EPADApplication : MonoBehaviour
{

    public Logger_Threading subjectLog;
    public ClockSynchronization clockSync;

    public Text debugText;

    private bool isConnected = false;


    //ui
    public RawImage ipadConnectionIndicator;
    public RawImage neuralConnectionIndicator;

    public SyncboxControl syncboxControl;

    public TimeSyncLog timeSyncLog;

    public string sessionDirectory = "";
    public int sessionID = 0;
    public string sessionStartedFileName = "";

    //A SINGLETON

    private static EPADApplication _instance;


    public static EPADApplication Instance
    {
        get
        {
            return _instance;
        }
    }



    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Instance already exists!");
            return;
        }
        _instance = this;

    }

        // Start is called before the first frame update
        void Start()
    {
        StartCoroutine(InitLogging());
        ipadConnectionIndicator.color = Color.red;
    }


    public void PrepareLogging()
    {
        Debug.Log("preparing logging");
        isConnected = true;
    }

    public void UpdateIPADConnectionStatus(bool isConnected)
    {
        Configuration.ipadConnection = isConnected;

        //debugText.text = "iPad: " + isConnected.ToString();
        if (isConnected)
            Green();
        else
            Red();
        //ipadConnectionIndicator.color = Color.green;
    }

    public void UpdateNeuralConnectionStatus(bool isConnected)
    {
        Configuration.neuralDeviceConnection = isConnected;
        //debugText.text = "Device: " + isConnected.ToString();
        neuralConnectionIndicator.color = (isConnected ? Color.green : Color.red);
    }

    public void BeginSyncboxSync()
    {
        StartCoroutine(syncboxControl.SyncImmediately());

    }

    void Red()
    {
        ipadConnectionIndicator.color = Color.red;
    }

    void Green()
    {
        ipadConnectionIndicator.color = Color.green;
    }
    IEnumerator InitLogging()
    {
        while(!isConnected)
        {
            yield return 0;
        }
        string subjectDirectory = Configuration.defaultLoggingPath + "/" + Configuration.subjectName + "/";
        UnityEngine.Debug.Log("subj directory is " + subjectDirectory);
        sessionDirectory = subjectDirectory + "session_0" + "/";

        //turn logging on
        Configuration.isLogging = true;

        UnityEngine.Debug.Log("session directory is " + sessionDirectory);
        sessionID = 0;
        string sessionIDString = "_0";

        if (!Directory.Exists(subjectDirectory))
        {
            Directory.CreateDirectory(subjectDirectory);
        }
        while (File.Exists(sessionDirectory + sessionStartedFileName))
        {//Directory.Exists(sessionDirectory)) {
            sessionID++;

            sessionIDString = "_" + sessionID.ToString();

            sessionDirectory = subjectDirectory + "session" + sessionIDString + "/";
        }

        //delete old files.
        if (Directory.Exists(sessionDirectory))
        {

            UnityEngine.Debug.Log("deleting old files");
            DirectoryInfo info = new DirectoryInfo(sessionDirectory);
            FileInfo[] fileInfo = info.GetFiles();
            for (int i = 0; i < fileInfo.Length; i++)
            {
                File.Delete(fileInfo[i].ToString());
            }
        }
        else
        { //if directory didn't exist, make it!
            UnityEngine.Debug.Log("creating directory");
            Directory.CreateDirectory(sessionDirectory);
        }

        subjectLog.fileName = sessionDirectory + Configuration.subjectName + "Log" + ".txt";
        subjectLog.InitiateLogging();
        Debug.Log("SUBJECT LOG: " + subjectLog.fileName);


        yield return null;
    }


    public void BeginClockSync(string[] ipadTime)
    {
        long[] epadTimes = new long[2];
        StartCoroutine(clockSync.RunSyncInterval());
        //timeSyncLog.LogEPADSyncTime(epadTimes);


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
