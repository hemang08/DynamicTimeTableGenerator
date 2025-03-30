using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTableGenerator.Model.TimeTable
{
    public class SubjectHoursModel
    {
        public string SubjectName { get; set; }
        public int Hours { get; set; }
    }

    public class TimeTableModel
    {
        [Required]
        [Range(1, 7, ErrorMessage = "No of Working Days must be between 1 and 7")]
        public int WorkingDays { get; set; }

        [Required]
        [Range(1, 8, ErrorMessage = "No of Subjects per day must be less than 9")]
        public int SubjectsPerDay { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total Subjects must be a positive number")]
        public int TotalSubjects { get; set; }

        public int TotalHours => WorkingDays * SubjectsPerDay;

        public List<SubjectHoursModel> SubjectHours { get; set; } = new List<SubjectHoursModel>();

        public List<List<string>> GeneratedTimetable { get; set; } = new List<List<string>>();
    }

}
