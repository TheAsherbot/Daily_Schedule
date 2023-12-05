using System;
using System.Collections.Generic;
using System.Linq;

using TheAshBot;

using Unity.VisualScripting;

using UnityEngine;

public class DailySchedule : MonoBehaviour
{

    private const string PROJECT_SAVE_DATA_NAME = "ProjectSaveData";
    private const string FINISHED_PROJECT_SAVE_DATA_NAME = "FinishedProjectSaveData";
    private const string QUIT_PROJECT_SAVE_DATA_NAME = "QuitProjectSaveData";


    public event Action<Project> onNewProjectAdded;
    public event Action<Project, int> onProjectFinished;
    public event Action<Project, int> onProjectQuit;



    private List<Project> projectList;



    private void Start()
    {
        projectList = new List<Project>();
        TryLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GetProject(1);
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }


    public void AddNewProject(Project project)
    {
        if (projectList.Contains(project)) return;

        projectList.Add(project);
        onNewProjectAdded?.Invoke(project);
    }
    public void FinishProject(Project project)
    {
        if (projectList.Contains(project))
        {
            int index = projectList.IndexOf(project);
            projectList.Remove(project);
            onProjectFinished?.Invoke(project, index);
            AddToFinishedProjects(project);
        }
    }
    public void QuitProject(Project project)
    {
        if (projectList.Contains(project))
        {
            int index = projectList.IndexOf(project);
            projectList.Remove(project);
            onProjectQuit?.Invoke(project, index);
            AddToQuitProjects(project);
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

        SaveProjects(fileName, saveData);;
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
        int month = DateTime.Today.Day;
        int dayOfTheWeek;
        switch (DateTime.Today.DayOfWeek.ToString())
        {
            default:
            case "Sunday":
                dayOfTheWeek = 1;
                    break;
            case "Monday":
                dayOfTheWeek = 2;
                break;
            case "Tuesday":
                dayOfTheWeek = 3;
                break;
            case "Wednesday":
                dayOfTheWeek = 4;
                break;
            case "Thursday":
                dayOfTheWeek = 5;
                break;
            case "Friday":
                dayOfTheWeek = 6;
                break;
            case "Saturday":
                dayOfTheWeek = 7;
                break;
        }
        int dayOfTheMonth = DateTime.Today.Month;
        int DayOfTheYear = DateTime.Today.DayOfYear;
        int Year = DateTime.Today.Year;

        int i;
        switch (projectNumber)
        {
            case 1:
                i = month + dayOfTheWeek + dayOfTheMonth + DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            case 2:
                i = month * dayOfTheWeek + dayOfTheMonth % DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            case 3:
                i = month - dayOfTheWeek * dayOfTheMonth + DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            case 4:
                i = month + dayOfTheWeek % dayOfTheMonth * DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            case 5:
                i = month + dayOfTheWeek - dayOfTheMonth + DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            case 6:
                i = month * dayOfTheWeek - dayOfTheMonth % DayOfTheYear + Year;
                return projectList[i % projectList.Count];
            default:
                i = month - dayOfTheWeek + dayOfTheMonth - DayOfTheYear + Year;
                return projectList[i % projectList.Count];
        }
    }

    private void GetSchedule()
    {

    }


    [Serializable]
    public struct SaveData
    {
        public Project.SaveData[] projectArray;
    }

    [Serializable]
    public struct Schedule
    {

        private DaySchedule[] SundaySchedule;
        private DaySchedule[] MondaySchedule;
        private DaySchedule[] TuesdaySchedule;
        private DaySchedule[] WednesdaySchedule;
        private DaySchedule[] ThursdaySchedule;
        private DaySchedule[] FridaySchedule;
        private DaySchedule[] SaturdaySchedule;

        public struct DaySchedule
        {
            public string name;

            public TimeFrame[] timeFrames;

            public struct TimeFrame
            {

            }
            
        }

    }

}
