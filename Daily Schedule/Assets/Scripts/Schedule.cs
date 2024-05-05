using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct Schedule
{

    public enum DayOfTheWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }


    public List<DaySchedule> SundaySchedule;
    public List<DaySchedule> MondaySchedule;
    public List<DaySchedule> TuesdaySchedule;
    public List<DaySchedule> WednesdaySchedule;
    public List<DaySchedule> ThursdaySchedule;
    public List<DaySchedule> FridaySchedule;
    public List<DaySchedule> SaturdaySchedule;

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