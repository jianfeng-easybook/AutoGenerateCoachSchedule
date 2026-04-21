using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class PreparedScheduleIdentityAssigner
    {
        public void Assign(IList<PreparedScheduleRow> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            var guidByTemplateGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var groupGuidByTemplateGroupGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var sequenceGuidByTemplateSequenceGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                var templateGuid = row.SourceTemplate.GUID;
                var templateGuidKey = string.IsNullOrWhiteSpace(templateGuid)
                    ? Guid.NewGuid().ToString()
                    : templateGuid;

                if (!guidByTemplateGuid.TryGetValue(templateGuidKey, out var generatedGuid))
                {
                    generatedGuid = Guid.NewGuid().ToString();
                    guidByTemplateGuid[templateGuidKey] = generatedGuid;
                }

                row.GeneratedGuid = generatedGuid;

                var templateGroupGuid = row.SourceTemplate.GroupGUID;
                var templateGroupGuidKey = string.IsNullOrWhiteSpace(templateGroupGuid)
                    ? Guid.NewGuid().ToString()
                    : templateGroupGuid;

                if (!groupGuidByTemplateGroupGuid.TryGetValue(templateGroupGuidKey, out var generatedGroupGuid))
                {
                    generatedGroupGuid = Guid.NewGuid().ToString();
                    groupGuidByTemplateGroupGuid[templateGroupGuidKey] = generatedGroupGuid;
                }

                row.GeneratedGroupGuid = generatedGroupGuid;

                var templateSequenceGuid = row.SourceTemplate.SequenceGUID;
                if (string.IsNullOrWhiteSpace(templateSequenceGuid))
                {
                    row.GeneratedSequenceGuid = null;
                    continue;
                }

                if (!sequenceGuidByTemplateSequenceGuid.TryGetValue(templateSequenceGuid, out var generatedSequenceGuid))
                {
                    generatedSequenceGuid = Guid.NewGuid().ToString();
                    sequenceGuidByTemplateSequenceGuid[templateSequenceGuid] = generatedSequenceGuid;
                }

                row.GeneratedSequenceGuid = generatedSequenceGuid;
            }
        }
    }
}
