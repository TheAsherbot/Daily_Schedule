using UnityEngine;
using UnityEngine.UIElements;

public class DailyScheduleUI : MonoBehaviour
{

    private VisualElement root;
    private ScrollView projectsScrollView;
    private ScrollView scheduleScrollView;
    private TextField projectNameTextField;
    private Button addNewProjectButton;

    [SerializeField] private bool isProjectNameTextFieldFocused;


    [SerializeField] private VisualTreeAsset projectTree;
    [SerializeField] private VisualTreeAsset scheduleTree;


    [Space(5)]
    [SerializeField] private DailySchedule dailySchedule;



    

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        projectsScrollView = root.Q<ScrollView>("projects-scroll-view");
        scheduleScrollView = root.Q<ScrollView>("schedule-scroll-view");
        projectNameTextField = root.Q<TextField>("project-name-text-field");
        addNewProjectButton = root.Q<Button>("add-new-project-button");

        projectNameTextField.RegisterCallback<FocusInEvent>(projectNameTextField_OnFocus);
        projectNameTextField.RegisterCallback<FocusOutEvent>(projectNameTextField_OnUnfocus);


        addNewProjectButton.clicked += AddNewProjectButton_clicked;

        dailySchedule.onNewProjectAdded += DailySchedule_onNewProjectAdded;
        dailySchedule.onProjectFinished += DailySchedule_onProjectFinished;
        dailySchedule.onProjectQuit += DailySchedule_onProjectQuit;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isProjectNameTextFieldFocused)
            {
                AddProject();
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



    private void AddNewProjectButton_clicked()
    {
        if (projectNameTextField.value == "" || projectNameTextField.value == "null" || projectNameTextField.value == null)
        {
            return;
        }

        Project project = new Project(projectNameTextField.value, dailySchedule);
        dailySchedule.AddNewProject(project);
        projectNameTextField.value = "";
    }

    private void DailySchedule_onNewProjectAdded(Project project)
    {
        VisualElement projectRootElement = projectTree.CloneTree();
        projectRootElement.Q<Label>("project-name-label").text = project.name;
        projectsScrollView.Add(projectRootElement);

        projectRootElement.Q<Button>("finish-button").clicked += project.Finish;
        projectRootElement.Q<Button>("quit-button").clicked += project.Quit;
    }

    private void DailySchedule_onProjectFinished(Project project, int index)
    {
        RemoveProject(index);
    }
    
    private void DailySchedule_onProjectQuit(Project project, int index)
    {
        RemoveProject(index);
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
