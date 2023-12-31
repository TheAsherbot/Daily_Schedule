using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;


public class DailyScheduleUI : MonoBehaviour
{


    #region Veriables

    #region UI Toolkit

    private VisualElement root;


    [SerializeField] private VisualTreeAsset projectTree;
    private ScrollView projectsScrollView;
    private TextField projectNameTextField;
    private Button addNewProjectButton;
    private bool isProjectNameTextFieldFocused;


    [SerializeField] private VisualTreeAsset scheduleTimeFrameTree;
    private ScrollView scheduleScrollView;
    private Label dayOfTheWeekLabel;
    private DropdownField scheduleDropDownField;

    #endregion





    [Space(5)]
    [SerializeField] private DailySchedule dailySchedule;

    #endregion



    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;


        projectsScrollView = root.Q<ScrollView>("projects-scroll-view");
        projectNameTextField = root.Q<TextField>("project-name-text-field");
        addNewProjectButton = root.Q<Button>("add-new-project-button");

        projectNameTextField.RegisterCallback<FocusInEvent>(projectNameTextField_OnFocus);
        projectNameTextField.RegisterCallback<FocusOutEvent>(projectNameTextField_OnUnfocus);
        addNewProjectButton.clicked += AddNewProjectButton_clicked;


        scheduleScrollView = root.Q<ScrollView>("schedule-scroll-view");
        dayOfTheWeekLabel = root.Q<Label>("day-of-the-week");
        scheduleDropDownField = root.Q<DropdownField>("schedule-drop-down-field");

        dayOfTheWeekLabel.text = DateTime.Today.DayOfWeek.ToString();
        List<DailySchedule.Schedule.DaySchedule> todaysSchedules = dailySchedule.GetTodaysSchedules();

        scheduleDropDownField.choices = new List<string>();

        scheduleDropDownField.RegisterValueChangedCallback(ScheduleDropDownField_OnValueChanged);


        dailySchedule.onNewProjectAdded += DailySchedule_onNewProjectAdded;
        dailySchedule.onProjectFinished += DailySchedule_onProjectFinished;
        dailySchedule.onProjectQuit += DailySchedule_onProjectQuit;

        dailySchedule.onCurrentScheduleChanged += DailySchedule_onCurrentScheduleChanged;
    }

    private void Start()
    {
        TabbedMenuController tabbedMenuController = new TabbedMenuController(root);

        tabbedMenuController.RegisterTabCallback();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isProjectNameTextFieldFocused)
            {
                AddProject();
                projectNameTextField.Focus();
            }
        }
    }


    private void projectNameTextField_OnFocus(FocusInEvent evt)
    {
        isProjectNameTextFieldFocused = true;
    }
    private void projectNameTextField_OnUnfocus(FocusOutEvent evt)
    {
        isProjectNameTextFieldFocused = false;
    }



    private void ScheduleDropDownField_OnValueChanged(ChangeEvent<string> eventCallback)
    {
        dailySchedule.SetCurrentSchedule(dailySchedule.GetTodaysSchedules()[scheduleDropDownField.choices.ToList().FindIndex((string value) => { return value == eventCallback.newValue; })]);
    }

    private void AddNewProjectButton_clicked()
    {
        AddProject();
    }



    private void DailySchedule_onCurrentScheduleChanged(DailySchedule.Schedule.DaySchedule currentSchedule)
    {
        UpdateSchedule();
        RefreshTodaysSchedules();
    }

    private void DailySchedule_onNewProjectAdded(Project project)
    {
        VisualElement projectRootElement = projectTree.CloneTree();
        projectRootElement.Q<Label>("project-name-label").text = project.name;
        projectRootElement.Q<VisualElement>("project").style.backgroundColor = projectsScrollView.childCount % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
        projectsScrollView.Add(projectRootElement);

        projectRootElement.Q<Button>("finish-button").clicked += project.Finish;
        projectRootElement.Q<Button>("quit-button").clicked += project.Quit;
    }
    private void DailySchedule_onProjectFinished(Project project, int index)
    {
        RemoveProject(index);
        UpdateSchedule();
        RefreshProjects();
    }
    private void DailySchedule_onProjectQuit(Project project, int index)
    {
        RemoveProject(index);
        UpdateSchedule();
        RefreshProjects();
    }


    private void RefreshTodaysSchedules()
    {
        List<DailySchedule.Schedule.DaySchedule> todaysSchedules = dailySchedule.GetTodaysSchedules();
        
        for (int i = 0; i < todaysSchedules.Count; i++)
        {
            scheduleDropDownField.choices.Add(todaysSchedules[i].name);
        }
        scheduleDropDownField.SetValueWithoutNotify(todaysSchedules[0].name);
    }

    private void RefreshProjects()
    {
        for (int i = 0; i < projectsScrollView.childCount; i++)
        {
            projectsScrollView.ElementAt(i).Q<VisualElement>("project").style.backgroundColor = i % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
        }
    }

    private void UpdateSchedule()
    {
        scheduleScrollView.Clear();
        DailySchedule.Schedule.DaySchedule todaysSchedule = dailySchedule.GetCurrentSchedule();

        for (int i = 0; i < todaysSchedule.TimeFrames.Length; i++)
        {
            string startTime = todaysSchedule.TimeFrames[i].StartTime.ToString();
            startTime = (startTime.Length == 3 ? ("" + startTime[0] + ':' + startTime[1] + startTime[2]) : ("" + startTime[0] + startTime[1] + ':' + startTime[2] + startTime[3]));
            
            string endTime = todaysSchedule.TimeFrames[i].EndTime.ToString();
            endTime = (endTime.Length == 3 ? ("" + endTime[0] + ':' + endTime[1] + endTime[2]) : ("" + endTime[0] + endTime[1] + ':' + endTime[2] + endTime[3]));


            VisualElement timeFrame = scheduleTimeFrameTree.CloneTree();
            timeFrame.Q<VisualElement>("schedule-time-frame").style.backgroundColor = i % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
            timeFrame.Q<Label>("time-frame-label").text = startTime + " - " + endTime;
            timeFrame.Q<Label>("time-frame-name-label").text = todaysSchedule.TimeFrames[i].TimeFrameName;
            if (todaysSchedule.TimeFrames[i].ProjectName != null && todaysSchedule.TimeFrames[i].ProjectName != " ")
            {
                timeFrame.Q<Label>("project-name-label").text = todaysSchedule.TimeFrames[i].ProjectName;
            }
            else
            {
                timeFrame.Q<VisualElement>("bottom").style.display = DisplayStyle.None;
            }


            scheduleScrollView.Add(timeFrame);
        }
    }

    private void AddProject()
    {
        if (projectNameTextField.value == "" || projectNameTextField.value == "null" || projectNameTextField.value == null)
        {
            return;
        }

        Project project = new Project(projectNameTextField.value, dailySchedule);
        dailySchedule.AddNewProject(project);
        projectNameTextField.value = "";
    }

    private void RemoveProject(int index)
    {
        projectsScrollView.RemoveAt(index);
    }



}
