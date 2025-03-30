using DynamicTimeTableGenerator.Model.TimeTable;
using Microsoft.AspNetCore.Mvc;

namespace DynamicTimeTableGenerator.Controllers
{
    public class TimeTableController : Controller
    {
        private static TimeTableModel _timeTableData = new TimeTableModel();
        public IActionResult Index()
        {
            return View(_timeTableData);
        }
        [HttpPost]
        public IActionResult Index(TimeTableModel model)
        {
            if (ModelState.IsValid)
            {
                _timeTableData = model;
                return RedirectToAction("EnterSubjectHours");
            }
            return View(model);
        }

        public IActionResult EnterSubjectHours()
        {
            if (_timeTableData.TotalSubjects <= 0)
            {

                TempData["ErrorMessage"] = "Please enter valid data before entering subject data and its hours.";
                return RedirectToAction("Index");
            }


            // If subjects are not initialized, initialize them
            if (_timeTableData.SubjectHours == null || !_timeTableData.SubjectHours.Any())
            {
                _timeTableData.SubjectHours = new List<SubjectHoursModel>();
                for (int i = 0; i < _timeTableData.TotalSubjects; i++)
                {
                    _timeTableData.SubjectHours.Add(new SubjectHoursModel());
                }
            }

            return View(_timeTableData);
        }

        [HttpPost]
        public IActionResult EnterSubjectHours(TimeTableModel model)
        {
            int totalEnteredHours = model.SubjectHours.Sum(s => s.Hours);

            if (totalEnteredHours != _timeTableData.TotalHours)
            {
                ModelState.AddModelError("", "Total hours of subjects must match the calculated total hours.");
                return View(model);
            }

            _timeTableData.SubjectHours = model.SubjectHours;
            return RedirectToAction("GenerateTimetable");
        }
        public IActionResult GenerateTimetable()
        {
            _timeTableData.GeneratedTimetable = GenerateDynamicTimeTable(_timeTableData);
            if (_timeTableData == null || _timeTableData.GeneratedTimetable.Count == 0)
            {
                TempData["ErrorMessage"] = "Please enter valid data before generating a timetable.";
                return RedirectToAction("Index");
            }
            return View(_timeTableData);
        }

        public IActionResult Reset()
        {
            _timeTableData = new TimeTableModel(); // Reset model data
            return RedirectToAction("Index");
        }

        private List<List<string>> GenerateDynamicTimeTable(TimeTableModel model)
        {
            var subjectList = new List<string>();

            // Correct way to generate subject list
            foreach (var subject in model.SubjectHours)
            {
                subjectList.AddRange(Enumerable.Repeat(subject.SubjectName, subject.Hours));
            }

            Random rand = new Random();
            subjectList = subjectList.OrderBy(x => rand.Next()).ToList();  // Shuffle subjects randomly

            List<List<string>> timetable = new List<List<string>>();
            int index = 0;

            for (int row = 0; row < model.SubjectsPerDay; row++)
            {
                List<string> daySubjects = new List<string>();
                for (int col = 0; col < model.WorkingDays; col++)
                {
                    if (index < subjectList.Count)
                    {
                        daySubjects.Add(subjectList[index++]);
                    }
                }
                timetable.Add(daySubjects);
            }

            return timetable;
        }

    }
}
