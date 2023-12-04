using System;
using System.Collections.Generic;
using UnityEngine;

public class DailySchedule : MonoBehaviour
{


    public event Action<Project> onNewProjectAdded;
    public event Action<Project, int> onProjectFinished;
    public event Action<Project, int> onProjectQuit;



    private List<Project> projectList;



    private int i;



    private void Awake()
    {
        if (!TryLoad())
        {
            projectList = new List<Project>();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            i = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            i = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            i = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            i = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            i = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            i = 6;
        }


        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GetProject(i);
        }
    }

    private void OnApplicationQuit()
    {
        Save();
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
        }
    }
    public void QuitProject(Project project)
    {
        if (projectList.Contains(project))
        {
            int index = projectList.IndexOf(project);
            projectList.Remove(project);
            onProjectQuit?.Invoke(project, index);
        }
    }


    public void Save()
    {

    }

    public bool TryLoad() 
    {
        return false;
    }


    [System.Serializable]
    public struct SavedData
    {
        private List<Project> projectList;
    }


}
