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

    [SerializeField] private VisualTreeAsset breakTree;
    private ScrollView breaksScrollView;
    private TextField breakNameTextField;
    private Button addNewBreakButton;
    private bool isBreakNameTextFieldFocused;


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

        projectNameTextField.RegisterCallback<FocusInEvent>(ProjectNameTextField_OnFocus);
        projectNameTextField.RegisterCallback<FocusOutEvent>(ProjectNameTextField_OnUnfocus);
        addNewProjectButton.clicked += AddNewProjectButton_clicked;


        breaksScrollView = root.Q<ScrollView>("breaks-scroll-view");
        breakNameTextField = root.Q<TextField>("break-name-text-field");
        addNewBreakButton = root.Q<Button>("add-new-break-button");

        breakNameTextField.RegisterCallback<FocusInEvent>(BreakNameTextField_OnFocus);
        breakNameTextField.RegisterCallback<FocusOutEvent>(BreakNameTextField_OnUnfocus);
        addNewBreakButton.clicked += AddNewBreakButton_clicked;


        scheduleScrollView = root.Q<ScrollView>("schedule-scroll-view");
        dayOfTheWeekLabel = root.Q<Label>("day-of-the-week");
        scheduleDropDownField = root.Q<DropdownField>("schedule-drop-down-field");

        dayOfTheWeekLabel.text = DateTime.Today.DayOfWeek.ToString();
        List<Schedule.DaySchedule> todaysSchedules = dailySchedule.GetTodaysSchedules();

        scheduleDropDownField.choices = new List<string>();

        scheduleDropDownField.RegisterValueChangedCallback(ScheduleDropDownField_OnValueChanged);


        dailySchedule.OnNewProjectAdded += DailySchedule_onNewProjectAdded;
        dailySchedule.OnProjectFinished += DailySchedule_onProjectFinished;
        dailySchedule.OnProjectQuit += DailySchedule_onProjectQuit;

        dailySchedule.OnNewBreakAdded += DailySchedule_onNewBreakAdded;
        dailySchedule.OnBreakStopped += DailySchedule_onBreakStopped;
    }

    private void OnEnable()
    {
        dailySchedule.OnCurrentScheduleChanged += DailySchedule_onCurrentScheduleChanged;
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
            if (isBreakNameTextFieldFocused)
            {
                AddBreak();
                breakNameTextField.Focus();
            }
        }
    }


    private void ProjectNameTextField_OnFocus(FocusInEvent evt)
    {
        isProjectNameTextFieldFocused = true;
    }
    private void ProjectNameTextField_OnUnfocus(FocusOutEvent evt)
    {
        isProjectNameTextFieldFocused = false;
    }

    private void BreakNameTextField_OnFocus(FocusInEvent evt)
    {
        isProjectNameTextFieldFocused = true;
    }
    private void BreakNameTextField_OnUnfocus(FocusOutEvent evt)
    {
        isProjectNameTextFieldFocused = false;
    }



    private void ScheduleDropDownField_OnValueChanged(ChangeEvent<string> eventCallback)
    {
        dailySchedule.SetCurrentSchedule(dailySchedule.GetTodaysSchedules()[scheduleDropDownField.choices.ToList().FindIndex((string value) => { return value == eventCallback.newValue; })]);
        List<Schedule.DaySchedule> todaysSchedules = dailySchedule.GetTodaysSchedules();
        scheduleDropDownField.SetValueWithoutNotify(todaysSchedules[scheduleDropDownField.choices.ToList().FindIndex((string value) => { return value == eventCallback.newValue; })].name);
    }

    private void AddNewProjectButton_clicked()
    {
        AddProject();
    }
    
    private void AddNewBreakButton_clicked()
    {
        AddBreak();
    }



    private void DailySchedule_onCurrentScheduleChanged(Schedule.DaySchedule currentSchedule)
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

    private void DailySchedule_onBreakStopped(Break _break, int index)
    {
        RemoveBreak(index);
        UpdateSchedule();
        RefreshBreaks();
    }
    private void DailySchedule_onNewBreakAdded(Break _break)
    {
        VisualElement breakRootElement = breakTree.CloneTree();
        breakRootElement.Q<Label>("break-name-label").text = _break.name;
        breakRootElement.Q<VisualElement>("break").style.backgroundColor = breaksScrollView.childCount % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
        breaksScrollView.Add(breakRootElement);

        breakRootElement.Q<Button>("stop-button").clicked += _break.Stop;
    }


    private void RefreshTodaysSchedules()
    {
        List<Schedule.DaySchedule> todaysSchedules = dailySchedule.GetTodaysSchedules();
        
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

    private void RefreshBreaks()
    {
        for (int i = 0; i < breaksScrollView.childCount; i++)
        {
            breaksScrollView.ElementAt(i).Q<VisualElement>("break").style.backgroundColor = i % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
        }
    }

    private void UpdateSchedule()
    {
        scheduleScrollView.Clear();
        Schedule.DaySchedule todaysSchedule = dailySchedule.GetCurrentSchedule();

        for (int i = 0; i < todaysSchedule.TimeFrames.Length; i++)
        {
            string startTime = todaysSchedule.TimeFrames[i].StartTime.ToString();
            startTime = (startTime.Length == 3 ? ("" + startTime[0] + ':' + startTime[1] + startTime[2]) : ("" + startTime[0] + startTime[1] + ':' + startTime[2] + startTime[3]));
            
            string endTime = todaysSchedule.TimeFrames[i].EndTime.ToString();
            endTime = (endTime.Length == 3 ? ("" + endTime[0] + ':' + endTime[1] + endTime[2]) : ("" + endTime[0] + endTime[1] + ':' + endTime[2] + endTime[3]));

            VisualElement timeFrameVisualElement = scheduleTimeFrameTree.CloneTree();
            timeFrameVisualElement.Q<VisualElement>("schedule-time-frame").style.backgroundColor = i % 2 == 0 ? Color.HSVToRGB(0, 0, 0.65f) : Color.HSVToRGB(0, 0, 0.55f);
            timeFrameVisualElement.Q<Label>("time-frame-label").text = startTime + " - " + endTime;
            timeFrameVisualElement.Q<Label>("time-frame-name-label").text = todaysSchedule.TimeFrames[i].TimeFrameName;
            if (todaysSchedule.TimeFrames[i].ProjectName != null && todaysSchedule.TimeFrames[i].ProjectName != " ")
            {
                timeFrameVisualElement.Q<Label>("time-frame-task-name-label").text = todaysSchedule.TimeFrames[i].ProjectName;
            }
            else if (todaysSchedule.TimeFrames[i].BreakName != null && todaysSchedule.TimeFrames[i].BreakName != " ")
            {
                timeFrameVisualElement.Q<Label>("time-frame-task-name-label").text = todaysSchedule.TimeFrames[i].BreakName;
            }
            else
            {
                timeFrameVisualElement.Q<VisualElement>("bottom").style.display = DisplayStyle.None;
            }
            Schedule.DaySchedule.TimeFrame timeFrame = todaysSchedule.TimeFrames[i];
            timeFrameVisualElement.Q<Button>("reroll-button").clicked += () =>
            {
                int timeFrameNumber = 0;
                for (int i = 0; i < todaysSchedule.TimeFrames.Length; i++)
                {
                    if (timeFrame == todaysSchedule.TimeFrames[i])
                    {
                        timeFrameNumber = i; 
                        break;
                    }
                }
                dailySchedule.RerollTimeFrame(timeFrameNumber);
            };


            scheduleScrollView.Add(timeFrameVisualElement);
        }
    }

    private void AddProject()
    {
        if (projectNameTextField.value == "" || projectNameTextField.value == "null" || projectNameTextField.value == null || projectNameTextField.value == "Project Name")
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
    

    private void AddBreak()
    {
        if (breakNameTextField.value == "" || breakNameTextField.value == "null" || breakNameTextField.value == null)
        {
            return;
        }

        Break _break = new Break(breakNameTextField.value, dailySchedule);
        dailySchedule.AddNewBreak(_break);
        breakNameTextField.value = "";
    }

    private void RemoveBreak(int index)
    {
        breaksScrollView.RemoveAt(index);
    }



}
