using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoggerScript : MonoBehaviour
{
    public LogData LogData = new LogData { estimatedDifficulty = new List<float>()};

    public void OnDestroy()
    {
        var dataPath = Path.Combine(Application.persistentDataPath, "LogData.json");
        string jsonString = JsonUtility.ToJson(LogData);

        using (StreamWriter streamWriter = File.CreateText(dataPath))
        {
            streamWriter.Write(jsonString);
        }
        Debug.Log(dataPath);
    }
}
