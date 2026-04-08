namespace AutoGenerateCoachSchedule.Services
{
    public class GenerationWindowResolver
    {
        public IList<DateTime> GetTargetDates(int? recurrenceType, DateTime today)
        {
            var result = new List<DateTime>();

            if (!recurrenceType.HasValue)
                return result;

            switch (recurrenceType.Value)
            {
                case 1:
                    result.Add(today.Date);
                    break;

                case 2:
                    for (var i = 0; i < 7; i++)
                    {
                        result.Add(today.Date.AddDays(i));
                    }
                    break;

                case 3:
                    var nextMonth = today.Date.AddMonths(1);
                    var startOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                    var endOfNextMonth = new DateTime(
                        nextMonth.Year,
                        nextMonth.Month,
                        DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));

                    for (var date = startOfNextMonth; date <= endOfNextMonth; date = date.AddDays(1))
                    {
                        result.Add(date);
                    }
                    break;
            }

            return result;
        }
    }
}