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


}
