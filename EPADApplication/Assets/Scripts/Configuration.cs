using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Configuration : MonoBehaviour
{

    public static bool isSyncing = false;
    public static bool isLogging = false;
    public static string defaultLoggingPath;
    public static string subjectName = "test";

    public static bool shouldSearchAgain = false;

    //connection status
    public static bool ipadConnection = false;
    public static bool neuralDeviceConnection = false;
    // Start is called before the first frame update

    private void Awake()
    {
        defaultLoggingPath = Directory.GetCurrentDirectory();
        Debug.Log("current directory " + defaultLoggingPath);
        //defaultLoggingPath = Application.persistentDataPath;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.U))
        //{

        //    neuralDeviceConnection = true;
        //}
        
    }

    public void SearchAgain()
    {
        shouldSearchAgain = true;
    }
}
