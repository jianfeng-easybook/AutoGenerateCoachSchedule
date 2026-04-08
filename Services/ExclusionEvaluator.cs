using System.Globalization;

namespace AutoGenerateCoachSchedule.Services
{
    public class ExclusionEvaluator
    {
        public bool IsExcluded(DateTime date, string? excludedDays, string? excludedDates)
        {
            var days = ParseExcludedDays(excludedDays);
            if (days.Contains(date.DayOfWeek))
                return true;

            var dates = ParseExcludedDates(excludedDates);
            if (dates.Contains(date.Date))
                return true;

            return false;
        }

        private static HashSet<DayOfWeek> ParseExcludedDays(string? excludedDays)
        {
            var result = new HashSet<DayOfWeek>();

            if (string.IsNullOrWhiteSpace(excludedDays))
                return result;

            var parts = excludedDays.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var raw in parts)
            {
                var value = raw.Trim();

                if (int.TryParse(value, out var numeric) && numeric >= 0 && numeric <= 6)
                {
                    result.Add((DayOfWeek)numeric);
                    continue;
                }

                if (Enum.TryParse<DayOfWeek>(value, true, out var enumValue))
                {
                    result.Add(enumValue);
                    continue;
                }

                if (TryParseShortDayName(value, out var shortDay))
                {
                    result.Add(shortDay);
                }
            }

            return result;
        }

        private static bool TryParseShortDayName(string value, out DayOfWeek dayOfWeek)
        {
            switch (value.Trim().ToLowerInvariant())
            {
                case "sun":
                    dayOfWeek = DayOfWeek.Sunday;
                    return true;
                case "mon":
                    dayOfWeek = DayOfWeek.Monday;
                    return true;
                case "tue":
                case "tues":
                    dayOfWeek = DayOfWeek.Tuesday;
                    return true;
                case "wed":
                    dayOfWeek = DayOfWeek.Wednesday;
                    return true;
                case "thu":
                case "thur":
                case "thurs":
                    dayOfWeek = DayOfWeek.Thursday;
                    return true;
                case "fri":
                    dayOfWeek = DayOfWeek.Friday;
                    return true;
                case "sat":
                    dayOfWeek = DayOfWeek.Saturday;
                    return true;
                default:
                    dayOfWeek = default(DayOfWeek);
                    return false;
            }
        }

        private static HashSet<DateTime> ParseExcludedDates(string? excludedDates)
        {
            var result = new HashSet<DateTime>();

            if (string.IsNullOrWhiteSpace(excludedDates))
                return result;

            var parts = excludedDates.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (DateTime.TryParse(part.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    result.Add(parsed.Date);
                }
            }

            return result;
        }
    }
}