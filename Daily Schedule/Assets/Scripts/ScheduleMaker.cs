using System;

using TheAshBot;

using UnityEngine;

public class ScheduleMaker : MonoBehaviour
{


    public event Action<Schedule.DaySchedule> OnScheduleChanged;
    public event Action<Schedule.DaySchedule> OnScheduleAdded;
    public event Action<Schedule.DaySchedule> OnScheduleRemoved;




    private Schedule schedule;
    private const string SCHEDULE_SAVE_DATA_NAME = "ScheduleSaveData";




    private void Start()
    {
        LoadSchedule();
    }




    public void ChangeSchedule(Schedule.DayOfTheWeek dayOfTheWeek, int scheduleIndex, Schedule.DaySchedule changedSchedule)
    {
        switch (dayOfTheWeek)
        {
            case Schedule.DayOfTheWeek.Sunday:
                schedule.SundaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Monday:
                schedule.MondaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Tuesday:
                schedule.TuesdaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Wednesday:
                schedule.WednesdaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Thursday:
                schedule.ThursdaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Friday:
                schedule.FridaySchedule[scheduleIndex] = changedSchedule;
                break;
            case Schedule.DayOfTheWeek.Saturday:
                schedule.SaturdaySchedule[scheduleIndex] = changedSchedule;
                break;
        }

        SaveSchedule();

        OnScheduleChanged?.Invoke(changedSchedule);
    }

    public void AddNewSchedule(Schedule.DayOfTheWeek dayOfTheWeek, Schedule.DaySchedule newDaySchedule)
    {
        switch (dayOfTheWeek)
        {
            case Schedule.DayOfTheWeek.Sunday:
                schedule.SundaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Monday:
                schedule.MondaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Tuesday:
                schedule.TuesdaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Wednesday:
                schedule.WednesdaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Thursday:
                schedule.ThursdaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Friday:
                schedule.FridaySchedule.Add(newDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Saturday:
                schedule.SaturdaySchedule.Add(newDaySchedule);
                break;
        }

        SaveSchedule();

        OnScheduleAdded?.Invoke(newDaySchedule);
    }

    public void RemoveSchedule(Schedule.DayOfTheWeek dayOfTheWeek, Schedule.DaySchedule removedDaySchedule)
    {
        switch (dayOfTheWeek)
        {
            case Schedule.DayOfTheWeek.Sunday:
                schedule.SundaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Monday:
                schedule.MondaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Tuesday:
                schedule.TuesdaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Wednesday:
                schedule.WednesdaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Thursday:
                schedule.ThursdaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Friday:
                schedule.FridaySchedule.Remove(removedDaySchedule);
                break;
            case Schedule.DayOfTheWeek.Saturday:
                schedule.SaturdaySchedule.Remove(removedDaySchedule);
                break;
        }

        SaveSchedule();

        OnScheduleRemoved?.Invoke(removedDaySchedule);
    }


    private void LoadSchedule()
    {
        schedule = SaveSystem.LoadJson<Schedule>(SaveSystem.RootPath.Resources, "Saves", SCHEDULE_SAVE_DATA_NAME);
    }
    private void SaveSchedule()
    {
        SaveSystem.SaveJson(schedule, SaveSystem.RootPath.Resources, "Saves", SCHEDULE_SAVE_DATA_NAME, true);
    }

}
