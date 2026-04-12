using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class BatchDuplicateFilter
    {
        public IReadOnlyList<PreparedScheduleRow> Filter(IEnumerable<PreparedScheduleRow> rows)
        {
            return rows
                .GroupBy(x => string.Join("|", new[]
                {
                    x.DepartureDateTime.Date.ToString("yyyy-MM-dd"),
                    x.DuplicateRouteFromPlace ?? "",
                    x.DuplicateRouteFromSubPlace ?? "",
                    x.DuplicateRouteToPlace ?? "",
                    x.DuplicateRouteToSubPlace ?? "",
                    x.DuplicateSequenceGuid ?? ""
                }), StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First())
                .ToList();
        }
    }
}
