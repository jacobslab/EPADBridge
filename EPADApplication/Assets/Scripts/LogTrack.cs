using UnityEngine;
using System.Collections;

//a parent class for all log track classes
public abstract class LogTrack : MonoBehaviour {
	public Logger_Threading subjectLog { get { return EPADApplication.Instance.subjectLog; } }
	public string separator { get { return Logger_Threading.LogTextSeparator; } }

	bool hasLoggedDestroy = false;

	string GetNameToLog(){
        string objName = gameObject.name;
        //SpawnableObject spawnObj = GetComponent<SpawnableObject>();
        //if (spawnObj != null)
        //{
        //    name = spawnObj.GetName();
        //}
        return objName;
    }

	public void LogDestroy(){
		subjectLog.Log (GameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), GetNameToLog() + separator + "DESTROYED");
	}

	void OnDestroy(){
		if (Configuration.isLogging) {
			if(!hasLoggedDestroy){
				LogTrack[] allLogTracks = GetMultipleLogTracks();
				if(allLogTracks.Length > 1){
					for(int i = 0; i < allLogTracks.Length; i++){
						allLogTracks[i].SetHasLoggedDestroy();
					}
				}
				else{
					SetHasLoggedDestroy();
				}
				LogDestroy();
			}
		}
	}

	public void SetHasLoggedDestroy(){
		hasLoggedDestroy = true;
	}
	
	LogTrack[] GetMultipleLogTracks(){
		LogTrack[] allLogTracks = GetComponents<LogTrack> ();
		return allLogTracks;
	}
}
