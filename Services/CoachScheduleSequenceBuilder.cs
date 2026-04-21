using AutoGenerateCoachSchedule.Data;
using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class CoachScheduleSequenceBuilder
    {
        public IReadOnlyList<CoachScheduleSequenceRowModel> Build(IReadOnlyList<CoachSchedule> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return Array.Empty<CoachScheduleSequenceRowModel>();
            }

            var orderedUnique = new List<(int PlaceId, int SubPlaceId)>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var row in rows)
            {
                TryAddNode(row.FromPlace, row.FromSubPlace, orderedUnique, seen);
                TryAddNode(row.ToPlace, row.ToSubPlace, orderedUnique, seen);
            }

            var result = new List<CoachScheduleSequenceRowModel>(orderedUnique.Count);
            for (var i = 0; i < orderedUnique.Count; i++)
            {
                var node = orderedUnique[i];
                result.Add(new CoachScheduleSequenceRowModel
                {
                    PlaceId = node.PlaceId,
                    SubPlaceId = node.SubPlaceId,
                    SequenceNumber = i + 1
                });
            }

            return result;
        }

        private static void TryAddNode(
            string? place,
            string? subPlace,
            List<(int PlaceId, int SubPlaceId)> orderedUnique,
            HashSet<string> seen)
        {
            if (!int.TryParse(place, out var placeId) || !int.TryParse(subPlace, out var subPlaceId))
            {
                return;
            }

            var key = $"{placeId}|{subPlaceId}";
            if (!seen.Add(key))
            {
                return;
            }

            orderedUnique.Add((placeId, subPlaceId));
        }
    }
}
