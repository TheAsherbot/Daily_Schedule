using System;

using UnityEngine;

[Serializable]
public struct Break
{

    public Break(string name, DailySchedule dailySchedule)
    {
        this.name = name;
        this.dailySchedule = dailySchedule;
    }


    public string name;

    private DailySchedule dailySchedule;


    public void Stop()
    {
        dailySchedule.StopBreak(this);
    }


    public static Break Load(string json, DailySchedule dailySchedule)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        return new Break(saveData.name, dailySchedule);
    }
    public static Break Load(SaveData saveData, DailySchedule dailySchedule)
    {
        return new Break(saveData.name, dailySchedule);
    }

    public SaveData Save()
    {
        return new SaveData { name = name };
    }

    public override string ToString()
    {
        return
            "        {\n" +
            "            \"name\": \"" + name + "\"\n" +
            "        }";
    }


    [Serializable]
    public struct SaveData
    {
        public string name;
    }
}
