using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class PreparedScheduleGuidAssigner
    {
        public void Assign(IList<PreparedScheduleRow> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            var generatedGuid = Guid.NewGuid().ToString();
            var generatedGroupGuid = Guid.NewGuid().ToString();

            foreach (var row in rows)
            {
                row.GeneratedGuid = generatedGuid;
                row.GeneratedGroupGuid = generatedGroupGuid;
            }
        }
    }
}