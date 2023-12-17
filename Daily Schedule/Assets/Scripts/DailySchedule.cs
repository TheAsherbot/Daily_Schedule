using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TheAshBot;

using Unity.VisualScripting;

using UnityEngine;

public class DailySchedule : MonoBehaviour
{

    private const string PROJECT_SAVE_DATA_NAME = "ProjectSaveData";
    private const string FINISHED_PROJECT_SAVE_DATA_NAME = "FinishedProjectSaveData";
    private const string QUIT_PROJECT_SAVE_DATA_NAME = "QuitProjectSaveData";
    private const string TODAYS_PROJECT_SAVE_DATA_NAME = "TodaysProjectSaveData";


    public event Action<Project> onNewProjectAdded;
    public event Action<Project, int> onProjectFinished;
    public event Action<Project, int> onProjectQuit;

    public event Action<Schedule.DaySchedule> onCurrentScheduleChanged;



    private List<Project> projectList;

    private Schedule schedules;
    private List<Schedule.DaySchedule> todaysSchedules;
    private Schedule.DaySchedule currentSchedule;


    private void Start()
    {
        projectList = new List<Project>();
        TryLoad();

        schedules = LoadSchedules();

        switch (DateTime.Today.DayOfWeek.ToString())
        {
            case "Sunday":
                todaysSchedules = schedules.SundaySchedule.ToList();
                break;
            case "Monday":
                todaysSchedules = schedules.MondaySchedule.ToList();
                break;
            case "Tuesday":
                todaysSchedules = schedules.TuesdaySchedule.ToList();
                break;
            case "Wednesday":
                todaysSchedules = schedules.WednesdaySchedule.ToList();
                break;
            case "Thursday":
                todaysSchedules = schedules.ThursdaySchedule.ToList();
                break;
            case "Friday":
                todaysSchedules = schedules.FridaySchedule.ToList();
                break;
            case "Saturday":
                todaysSchedules = schedules.SaturdaySchedule.ToList();
                break;
        }

        currentSchedule = todaysSchedules[0];
        PutProjectsIntoSchedule();
        onCurrentScheduleChanged?.Invoke(currentSchedule);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GetProject(1);
            Save();
        }
    }



    public void AddNewProject(Project project)
    {
        if (projectList.Contains(project)) return;

        projectList.Add(project);
        onNewProjectAdded?.Invoke(project);
        Save();
    }
    public void FinishProject(Project project)
    {
        if (projectList.Contains(project))
        {
            int index = projectList.IndexOf(project);
            string oldProjectName = projectList[index].name;
            projectList.Remove(project);
            ShiftTodaysProjects(index, oldProjectName);
            onProjectFinished?.Invoke(project, index);
            AddToFinishedProjects(project);
            Save();
        }
    }
    public void QuitProject(Project project)
    {
        if (projectList.Contains(project))
        {
            int index = projectList.IndexOf(project);
            string oldProjectName = projectList[index].name;
            projectList.Remove(project);
            ShiftTodaysProjects(index, oldProjectName);
            onProjectQuit?.Invoke(project, index);
            AddToQuitProjects(project);
            Save();
        }
    }

    public void ChangeSchedule(int scheduleIndex)
    {
        if (scheduleIndex < todaysSchedules.Count)
        {
            currentSchedule = todaysSchedules[scheduleIndex];
            onCurrentScheduleChanged?.Invoke(currentSchedule);
        }
    }

    public Schedule GetSchedules()
    {
        return schedules;
    }
    public List<Schedule.DaySchedule> GetTodaysSchedules()
    {
        return todaysSchedules;
    }
    public Schedule.DaySchedule GetCurrentSchedule()
    {
        return currentSchedule;
    }
    public void SetCurrentSchedule(Schedule.DaySchedule daySchedule)
    {
        currentSchedule = daySchedule;
        onCurrentScheduleChanged?.Invoke(currentSchedule);
    }

    public void RerollTimeFrame(int timeFrame)
    {
        TodaysProjects todaysProjects = SaveSystem.LoadJson<TodaysProjects>(SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME);


        if (currentSchedule.TimeFrames[timeFrame].ProjectName == projectList[todaysProjects.projectIndex0].name)
        {

        }
        else if (currentSchedule.TimeFrames[timeFrame].ProjectName == projectList[todaysProjects.projectIndex0].name)
        {

        }
            
    }

    private void Save()
    {
        SaveData saveData = new SaveData();

        saveData.projectArray = new Project.SaveData[projectList.Count];
        for (int i = 0; i < saveData.projectArray.Length; i++)
        {
            saveData.projectArray[i] = projectList[i].Save();
        }

        SaveProjects(PROJECT_SAVE_DATA_NAME, saveData);
    }
    private bool TryLoad()
    {
        bool isFileLoaded = TryLoadProjects(PROJECT_SAVE_DATA_NAME, out SaveData saveData);

        if (!isFileLoaded)
        {
            return false;
        }

        for (int i = 0; i < saveData.projectArray.Length; i++)
        {
            projectList.Add(Project.Load(saveData.projectArray[i], this));
            onNewProjectAdded(projectList[i]);
        }

        return true;
    }

    private void AddToFinishedProjects(Project project)
    {
        AddProjectToFile(FINISHED_PROJECT_SAVE_DATA_NAME, project.Save());
    }
    private void AddToQuitProjects(Project project)
    {
        AddProjectToFile(QUIT_PROJECT_SAVE_DATA_NAME, project.Save());
    }

    private void AddProjectToFile(string fileName, Project.SaveData project)
    {
        bool isFileLoaded = TryLoadProjects(fileName, out SaveData saveData);

        if (!isFileLoaded)
        {
            return;
        }

        List<Project.SaveData> finishedProjectList = saveData.projectArray.ToList();
        finishedProjectList.Add(project);
        saveData.projectArray = finishedProjectList.ToArray();

        SaveProjects(fileName, saveData); ;
    }


    private bool TryLoadProjects(string fileName, out SaveData saveData)
    {
        saveData = SaveSystem.LoadJson<SaveData>(SaveSystem.RootPath.Resources, "Saves", fileName);

        if (saveData.projectArray == null)
        {
            return false;
        }
        else if (saveData.projectArray.Length == 0)
        {
            return false;
        }

        return true;
    }
    private void SaveProjects(string fileName, SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);

        SaveSystem.SaveString(json, SaveSystem.RootPath.Resources, "Saves", fileName, SaveSystem.FileType.Json, true);
    }



    private Project GetProject(int projectNumber)
    {
        TodaysProjects todaysProjects = SaveSystem.LoadJson<TodaysProjects>(SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME);

        if (todaysProjects.date != DateTime.Today.Date.ToString())
        {
            // Change values

            todaysProjects.date = DateTime.Today.Date.ToString();
            todaysProjects.projectIndex0 = UnityEngine.Random.Range(0, projectList.Count);
            todaysProjects.projectIndex1 = UnityEngine.Random.Range(0, projectList.Count);
            todaysProjects.projectIndex2 = UnityEngine.Random.Range(0, projectList.Count);
            todaysProjects.projectIndex3 = UnityEngine.Random.Range(0, projectList.Count);
            todaysProjects.projectIndex4 = UnityEngine.Random.Range(0, projectList.Count);
            todaysProjects.projectIndex5 = UnityEngine.Random.Range(0, projectList.Count);

            SaveSystem.SaveJson(todaysProjects, SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME, true);

        }

        switch (projectNumber)
        {
            default:
            case 0:
                return projectList[todaysProjects.projectIndex0];
            case 1:
                return projectList[todaysProjects.projectIndex1];
            case 2:
                return projectList[todaysProjects.projectIndex2];
            case 3:
                return projectList[todaysProjects.projectIndex3];
            case 4:
                return projectList[todaysProjects.projectIndex4];
            case 5:
                return projectList[todaysProjects.projectIndex5];
        }
    }
    private void ShiftTodaysProjects(int removedProjectIndex, string oldProjectName)
    {
        TodaysProjects todaysProjects = SaveSystem.LoadJson<TodaysProjects>(SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME);

        todaysProjects.date = DateTime.Today.Date.ToString();

        ShiftIfNecessaire(ref todaysProjects.projectIndex0);
        ShiftIfNecessaire(ref todaysProjects.projectIndex1);
        ShiftIfNecessaire(ref todaysProjects.projectIndex2);
        ShiftIfNecessaire(ref todaysProjects.projectIndex3);
        ShiftIfNecessaire(ref todaysProjects.projectIndex4);
        ShiftIfNecessaire(ref todaysProjects.projectIndex5);

        SaveSystem.SaveJson(todaysProjects, SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME, true);

        void ShiftIfNecessaire(ref int projectIndex)
        {
            if (projectIndex == removedProjectIndex)
            {
                projectIndex = UnityEngine.Random.Range(0, projectList.Count);
                for (int i = 0; i < currentSchedule.TimeFrames.Length; i++)
                {
                    if (currentSchedule.TimeFrames[i].ProjectName == oldProjectName)
                    {
                        currentSchedule.TimeFrames[i].ProjectName = projectList[projectIndex].name;
                    }
                }
            }
            else if (projectIndex > removedProjectIndex)
            {
                projectIndex--;
            }
        }
    }

    private Schedule LoadSchedules()
    {
        Schedule schedule = SaveSystem.LoadJson<Schedule>(SaveSystem.RootPath.Resources, "Saves", "ScheduleSaveData");

        return schedule;
    }

    private void PutProjectsIntoSchedule()
    {
        int j = 0;
        for (int i = 0; i < currentSchedule.TimeFrames.Length; i++)
        {
            if (currentSchedule.TimeFrames[i].TimeFrameName.Contains("Project #"))
            {
                this.Log("GetProject(j).name: " + GetProject(j).name);
                currentSchedule.TimeFrames[i].ProjectName = GetProject(j).name;
                j++;
            }
        }
    }



    [Serializable]
    public struct SaveData
    {
        public Project.SaveData[] projectArray;
    }

    [Serializable]
    public struct Schedule
    {

        public DaySchedule[] SundaySchedule;
        public DaySchedule[] MondaySchedule;
        public DaySchedule[] TuesdaySchedule;
        public DaySchedule[] WednesdaySchedule;
        public DaySchedule[] ThursdaySchedule;
        public DaySchedule[] FridaySchedule;
        public DaySchedule[] SaturdaySchedule;

        [Serializable]
        public struct DaySchedule
        {
            public string name;

            public TimeFrame[] TimeFrames;

            [Serializable]
            public struct TimeFrame
            {
                public int StartTime;
                public int EndTime;
                public string TimeFrameName;
                public string ProjectName;
            }

        }


        public override string ToString()
        {
            string json = JsonUtility.ToJson(this);
            json = json.Replace("{", "{\n").
                Replace("[", "[\n").
                Replace(",", ",\n");
            return json;
        }
    }

    [Serializable]
    public struct TodaysProjects
    {
        public string date;

        public int projectIndex0;
        public int projectIndex1;
        public int projectIndex2;
        public int projectIndex3;
        public int projectIndex4;
        public int projectIndex5;
    }

}
