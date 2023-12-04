using System;
using System.Collections.Generic;
using UnityEngine;

public class DailySchedule : MonoBehaviour
{


    public event Action<Project> onNewProjectAdded;
    public event Action<Project, int> onProjectFinished;
    public event Action<Project, int> onProjectQuit;



    private List<Project> projectList;





    private void Awake()
    {
        Debug.Log($"DateTime.Today.Day: {DateTime.Today.Day}");
        Debug.Log($"DateTime.Today.DayOfWeek: {DateTime.Today.DayOfWeek}");
        Debug.Log($"DateTime.Today.DayOfYear: {DateTime.Today.DayOfYear}");
        Debug.Log($"DateTime.Today.Month: {DateTime.Today.Month}");
        Debug.Log($"DateTime.Today.Year: {DateTime.Today.Year}");

        if (!TryLoad())
        {
            projectList = new List<Project>();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }


    private Project GetProject(int projectNumber)
    {
        int month = DateTime.Today.Day;
        int dayOfTheWeek = DateTime.Today.DayOfWeek;
        int dayOfTheMonth = DateTime.Today.Month;
        int DayOfTheYear = DateTime.Today.DayOfYear;
        int Year = DateTime.Today.Year;
        switch (projectNumber)
        {
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            default: 
                break;
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
