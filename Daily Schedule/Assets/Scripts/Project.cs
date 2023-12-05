using System;

using UnityEngine;

[System.Serializable]
public struct Project
{

    public enum ProjectState
    {
        InProgress,
        Finish,
        Quit,
    }


    public Project(string name, DailySchedule dailySchedule)
    {
        this.name = name;
        state = ProjectState.InProgress;
        this.dailySchedule = dailySchedule;
    }
    public Project(string name, ProjectState state, DailySchedule dailySchedule)
    {
        this.name = name;
        this.state = state;
        this.dailySchedule = dailySchedule;
    }


    public string name;
    public ProjectState state;

    private DailySchedule dailySchedule;


    public void Quit()
    {
        dailySchedule.QuitProject(this);
    }
    public void Finish()
    {
        dailySchedule.FinishProject(this);
    }


    public static Project Load(string json, DailySchedule dailySchedule)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        return new Project(saveData.name, saveData.state, dailySchedule);
    }
    public static Project Load(SaveData saveData, DailySchedule dailySchedule)
    {
        return new Project(saveData.name, saveData.state, dailySchedule);
    }

    public SaveData Save()
    {
        return new SaveData { name = name, state = state };
    }

    public override string ToString()
    {
        return
            "        {\n" +
            "            \"name\": \"" + name + "\",\n" +
            "            \"state\": " + ((int)state) + "\n" +
            "        }";
    }


    [Serializable]
    public struct SaveData
    {
        public string name;
        public ProjectState state;
    }
}
