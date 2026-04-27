using Microsoft.Data.SqlClient;
using System.Data;

namespace AutoGenerateCoachSchedule.Data
{
    public class CoachScheduleSequenceRowModel
    {
        public int PlaceId { get; set; }
        public int SubPlaceId { get; set; }
        public int SequenceNumber { get; set; }
    }

    public class CoachScheduleSequenceRepository
    {
        public async Task<int> ReplaceByScheduleGuidAsync(
            string scheduleGuid,
            IReadOnlyList<CoachScheduleSequenceRowModel> rows,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(scheduleGuid))
            {
                throw new ArgumentException("Schedule GUID cannot be null or whitespace.", nameof(scheduleGuid));
            }

            const string deleteSql = @"
            DELETE FROM dbo.CoachScheduleSequence
            WHERE ScheduleGUID = @ScheduleGUID;";

            await using (var deleteCmd = new SqlCommand(deleteSql, conn, tx))
            {
                deleteCmd.Parameters.Add("@ScheduleGUID", SqlDbType.VarChar, 50).Value = scheduleGuid;
                await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            if (rows.Count == 0)
            {
                return 0;
            }

            const string insertSql = @"
            INSERT INTO dbo.CoachScheduleSequence
            (
                SequenceGUID,
                FromPlaceID,
                FromSubPlaceID,
                SequenceNumber,
                ScheduleGUID
            )
            VALUES
            (
                NULL,
                @FromPlaceID,
                @FromSubPlaceID,
                @SequenceNumber,
                @ScheduleGUID
            );";

            var insertedCount = 0;

            foreach (var row in rows)
            {
                await using var insertCmd = new SqlCommand(insertSql, conn, tx);
                insertCmd.Parameters.Add("@FromPlaceID", SqlDbType.Int).Value = row.PlaceId;
                insertCmd.Parameters.Add("@FromSubPlaceID", SqlDbType.Int).Value = row.SubPlaceId;
                insertCmd.Parameters.Add("@SequenceNumber", SqlDbType.Int).Value = row.SequenceNumber;
                insertCmd.Parameters.Add("@ScheduleGUID", SqlDbType.VarChar, 50).Value = scheduleGuid;

                insertedCount += await insertCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return insertedCount;
        }

        public async Task<int> ReplaceBySequenceGuidAsync(
            string sequenceGuid,
            IReadOnlyList<CoachScheduleSequenceRowModel> rows,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sequenceGuid))
            {
                throw new ArgumentException("Sequence GUID cannot be null or whitespace.", nameof(sequenceGuid));
            }

            const string deleteSql = @"
            DELETE FROM dbo.CoachScheduleSequence
            WHERE SequenceGUID = @SequenceGUID;";

            await using (var deleteCmd = new SqlCommand(deleteSql, conn, tx))
            {
                deleteCmd.Parameters.Add("@SequenceGUID", SqlDbType.VarChar, 50).Value = sequenceGuid;
                await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            if (rows.Count == 0)
            {
                return 0;
            }

            const string insertSql = @"
            INSERT INTO dbo.CoachScheduleSequence
            (
                SequenceGUID,
                FromPlaceID,
                FromSubPlaceID,
                SequenceNumber,
                ScheduleGUID
            )
            VALUES
            (
                @SequenceGUID,
                @FromPlaceID,
                @FromSubPlaceID,
                @SequenceNumber,
                NULL
            );";

            var insertedCount = 0;

            foreach (var row in rows)
            {
                await using var insertCmd = new SqlCommand(insertSql, conn, tx);
                insertCmd.Parameters.Add("@SequenceGUID", SqlDbType.VarChar, 50).Value = sequenceGuid;
                insertCmd.Parameters.Add("@FromPlaceID", SqlDbType.Int).Value = row.PlaceId;
                insertCmd.Parameters.Add("@FromSubPlaceID", SqlDbType.Int).Value = row.SubPlaceId;
                insertCmd.Parameters.Add("@SequenceNumber", SqlDbType.Int).Value = row.SequenceNumber;

                insertedCount += await insertCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            return insertedCount;
        }
    }
}
