namespace AutoGenerateCoachSchedule.Services
{
    public class RecurrenceDueEvaluator
    {
        public bool IsDue(int? recurrenceType, DateTime? lastRunDate, DateTime today)
        {
            if (!recurrenceType.HasValue)
                return false;

            if (!lastRunDate.HasValue)
                return true;

            switch (recurrenceType.Value)
            {
                case 1:
                    return lastRunDate.Value.Date <= today.Date.AddDays(-1);

                case 2:
                    return lastRunDate.Value.Date <= today.Date.AddDays(-7);

                case 3:
                    return lastRunDate.Value.Date <= today.Date.AddMonths(-1);

                default:
                    return false;
            }
        }
    }
}