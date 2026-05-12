using EduAsistant.Models;

namespace EduAsistant.Services
{
    public class StudyPlannerService
    {
        public List<StudySession> GenerateStudySessions(List<Topic> topics, DateTime startDate, DateTime examDate)
        {
            var generatedSessions = new List<StudySession>();
            int totalDaysAvailable = (examDate.Date - startDate.Date).Days;

            if (totalDaysAvailable <= 0)
            {
                totalDaysAvailable = 1;
            }

            int currentDayOffset = 0;
            foreach (var topic in topics)
            {
                DateTime sessionDate = startDate.AddDays(currentDayOffset % totalDaysAvailable);

                var session = new StudySession
                {
                    ScheduledDate = sessionDate,
                    DurationMinutes = topic.TotalEstimatedMinutes,
                    IsCompleted = false,
                    CustomTitle = "Learning: " + topic.Name,
                    Topic = topic
                };

                topic.StudySessions.Add(session);
                generatedSessions.Add(session);

                currentDayOffset++;
            }

            return generatedSessions;
        }
    }
}