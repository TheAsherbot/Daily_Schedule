using System;
using System.Collections.Generic;
using System.Linq;

using TheAshBot;

using UnityEngine;
using UnityEngine.Rendering;

public class DailySchedule : MonoBehaviour
{

    private const string PROJECT_SAVE_DATA_NAME = "ProjectSaveData";
    private const string BREAK_SAVE_DATA_NAME = "BreakSaveData";
    private const string FINISHED_PROJECT_SAVE_DATA_NAME = "FinishedProjectSaveData";
    private const string QUIT_PROJECT_SAVE_DATA_NAME = "QuitProjectSaveData";
    private const string TODAYS_PROJECT_SAVE_DATA_NAME = "TodaysProjectSaveData";


    public event Action<Project> onNewProjectAdded;
    public event Action<Project, int> onProjectFinished;
    public event Action<Project, int> onProjectQuit;

    public event Action<Break> onNewBreakAdded;
    public event Action<Break, int> onBreakStopped;

    public event Action<Schedule.DaySchedule> onCurrentScheduleChanged;



    private List<Project> projectList;
    private List<Break> breakList;

    private Schedule schedules;
    private List<Schedule.DaySchedule> todaysSchedules;
    private Schedule.DaySchedule currentSchedule;


    private void Awake()
    {
        onCurrentScheduleChanged += PutProjectsIntoSchedule;
    }

    private void Start()
    {
        projectList = new List<Project>();
        breakList = new List<Break>();
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

    public void AddNewBreak(Break _break)
    {
        if (breakList.Contains(_break)) return;

        breakList.Add(_break);
        onNewBreakAdded?.Invoke(_break);
        Save();
    }
    public void StopBreak(Break _break)
    {
        if (breakList.Contains(_break))
        {
            int index = breakList.IndexOf(_break);
            string oldProjectName = breakList[index].name;
            breakList.Remove(_break);
            ShiftTodaysProjects(index, oldProjectName);
            onBreakStopped?.Invoke(_break, index);
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

        if (currentSchedule.TimeFrames[timeFrame].TimeFrameName.Contains("Project #"))
        {
            int projectNumber = 0;
            for (int i = 0; i < currentSchedule.TimeFrames.Length; i++)
            {
                if (currentSchedule.TimeFrames[i].TimeFrameName.Contains("Project #"))
                {
                    if (i == timeFrame)
                    {
                        switch (projectNumber)
                        {
                            case 0:
                                todaysProjects.projectIndex0 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                            case 1:
                                todaysProjects.projectIndex1 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                            case 2:
                                todaysProjects.projectIndex2 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                            case 3:
                                todaysProjects.projectIndex3 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                            case 4:
                                todaysProjects.projectIndex4 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                            case 5:
                                todaysProjects.projectIndex5 = UnityEngine.Random.Range(0, projectList.Count);
                                break;
                        }
                        break;
                    }
                    projectNumber++;
                }
            }
        }

        SaveSystem.SaveJson(todaysProjects, SaveSystem.RootPath.Resources, "Saves", TODAYS_PROJECT_SAVE_DATA_NAME, true);

        onCurrentScheduleChanged?.Invoke(currentSchedule);
    }

    private void Save()
    {
        ProjectSaveData projectSaveData = new ProjectSaveData();

        projectSaveData.projectArray = new Project.SaveData[projectList.Count];
        for (int i = 0; i < projectSaveData.projectArray.Length; i++)
        {
            projectSaveData.projectArray[i] = projectList[i].Save();
        }

        SaveProjects(PROJECT_SAVE_DATA_NAME, projectSaveData);


        BreakSaveData breakSaveData = new BreakSaveData();

        breakSaveData.breakArray = new Break.SaveData[breakList.Count];
        for (int i = 0; i < breakSaveData.breakArray.Length; i++)
        {
            breakSaveData.breakArray[i] = breakList[i].Save();
        }

        SaveBreaks(breakSaveData);
    }
    private bool TryLoad()
    {
        bool isFileLoaded = TryLoadProjects(PROJECT_SAVE_DATA_NAME, out ProjectSaveData projectSaveData);

        if (!isFileLoaded)
        {
            return false;
        }

        for (int i = 0; i < projectSaveData.projectArray.Length; i++)
        {
            projectList.Add(Project.Load(projectSaveData.projectArray[i], this));
            onNewProjectAdded(projectList[i]);
        }

        isFileLoaded = TryLoadBreaks(out BreakSaveData breakSaveData);

        if (!isFileLoaded)
        {
            return false;
        }

        for (int i = 0; i < breakSaveData.breakArray.Length; i++)
        {
            breakList.Add(Break.Load(breakSaveData.breakArray[i], this));
            onNewBreakAdded(breakList[i]);
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
        bool isFileLoaded = TryLoadProjects(fileName, out ProjectSaveData saveData);

        if (!isFileLoaded)
        {
            return;
        }

        List<Project.SaveData> finishedProjectList = saveData.projectArray.ToList();
        finishedProjectList.Add(project);
        saveData.projectArray = finishedProjectList.ToArray();

        SaveProjects(fileName, saveData);
    }


    private bool TryLoadProjects(string fileName, out ProjectSaveData saveData)
    {
        saveData = SaveSystem.LoadJson<ProjectSaveData>(SaveSystem.RootPath.Resources, "Saves", fileName);

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
    private void SaveProjects(string fileName, ProjectSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);

        SaveSystem.SaveString(json, SaveSystem.RootPath.Resources, "Saves", fileName, SaveSystem.FileType.Json, true);
    }

    private bool TryLoadBreaks(out BreakSaveData saveData)
    {
        saveData = SaveSystem.LoadJson<BreakSaveData>(SaveSystem.RootPath.Resources, "Saves", BREAK_SAVE_DATA_NAME);

        if (saveData.breakArray == null)
        {
            return false;
        }
        else if (saveData.breakArray.Length == 0)
        {
            return false;
        }

        return true;
    }
    private void SaveBreaks(BreakSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);

        SaveSystem.SaveString(json, SaveSystem.RootPath.Resources, "Saves", BREAK_SAVE_DATA_NAME, SaveSystem.FileType.Json, true);
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

    private Project GetBreak(int breakNumber)
    {
        // TODO: Make this do breaks
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

    private void PutProjectsIntoSchedule(Schedule.DaySchedule currentSchedule)
    {
        int j = 0;
        for (int i = 0; i < currentSchedule.TimeFrames.Length; i++)
        {
            if (currentSchedule.TimeFrames[i].TimeFrameName.Contains("Project #"))
            {
                currentSchedule.TimeFrames[i].ProjectName = GetProject(j).name;
                j++;
            }
        }
    }

    private void PutBreaksIntoSchedule(Schedule.DaySchedule currentSchedule)
    {
        int j = 0;
        for (int i = 0; i < currentSchedule.TimeFrames.Length; i++)
        {
            if (currentSchedule.TimeFrames[i].TimeFrameName.Contains("Break #"))
            {
                currentSchedule.TimeFrames[i].BreakName = GetProject(j).name;
                j++;
            }
        }
    }



    [Serializable]
    public struct ProjectSaveData
    {
        public Project.SaveData[] projectArray;
    }
    
    [Serializable]
    public struct BreakSaveData
    {
        public Break.SaveData[] breakArray;
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
                public string BreakName;



                public static bool operator ==(TimeFrame timeFrame1, TimeFrame timeFrame2)
                {
                    if (timeFrame1.StartTime == timeFrame2.StartTime && timeFrame1.EndTime == timeFrame2.EndTime && timeFrame1.TimeFrameName == timeFrame2.TimeFrameName && timeFrame1.ProjectName == timeFrame2.ProjectName)
                    {
                        return true;
                    }
                    return false;
                }
                public static bool operator !=(TimeFrame timeFrame1, TimeFrame timeFrame2)
                {
                    if (timeFrame1.StartTime == timeFrame2.StartTime && timeFrame1.EndTime == timeFrame2.EndTime && timeFrame1.TimeFrameName == timeFrame2.TimeFrameName && timeFrame1.ProjectName == timeFrame2.ProjectName)
                    {
                        return false;
                    }
                    return true;
                }

                public override bool Equals(object obj)
                {
                    if (!(obj is TimeFrame timeFrame))
                    {
                        return false;
                    }

                    if (timeFrame.StartTime == StartTime && timeFrame.EndTime == EndTime && timeFrame.TimeFrameName == TimeFrameName && timeFrame.ProjectName == ProjectName)
                    {
                        return true;
                    }

                    return false;
                }
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
