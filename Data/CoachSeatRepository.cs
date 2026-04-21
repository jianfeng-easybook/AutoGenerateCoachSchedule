using Microsoft.Data.SqlClient;
using System.Data;

namespace AutoGenerateCoachSchedule.Data
{
    public class CoachSeatRepository
    {
        public async Task<int> InsertFromTemplateAsync(
            int coachId,
            string scheduleGuid,
            string runUser,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO dbo.Coach_Seat
            (
                Seat_Position_X,
                Seat_Position_Y,
                Seat_Number,
                Seat_Status,
                Create_Date,
                Create_User,
                Update_Date,
                Update_User,
                Schedule_GUID,
                Deck
            )
            SELECT
                cst.Seat_Position_X,
                cst.Seat_Position_Y,
                cst.Seat_Number,
                CASE
                    WHEN cst.Status = 1 THEN 0
                    WHEN cst.Status = 0 THEN 3
                    WHEN cst.Status = 2 THEN 6
                    ELSE cst.Status
                END,
                GETDATE(),
                @RunUser,
                GETDATE(),
                @RunUser,
                @Schedule_GUID,
                cst.Deck
            FROM dbo.Coach_Seat_Template cst WITH (NOLOCK)
            INNER JOIN dbo.CoachType ct WITH (NOLOCK)
                ON cst.CoachType_ID = ct.CoachType_ID
            INNER JOIN dbo.Coaches c WITH (NOLOCK)
                ON c.CoachType_ID = ct.CoachType_ID
            WHERE c.Coach_ID = @Coach_ID
            AND NOT EXISTS
            (
                SELECT 1
                FROM dbo.Coach_Seat cs WITH (NOLOCK)
                WHERE cs.Seat_Position_X = cst.Seat_Position_X
                  AND cs.Seat_Position_Y = cst.Seat_Position_Y
                  AND cs.Schedule_GUID = @Schedule_GUID
                  AND cs.Deck = cst.Deck
            );

            SELECT @@ROWCOUNT;";

            await using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.Add("@Coach_ID", SqlDbType.Int).Value = coachId;
            cmd.Parameters.Add("@Schedule_GUID", SqlDbType.VarChar, 50).Value = scheduleGuid;
            cmd.Parameters.Add("@RunUser", SqlDbType.VarChar, 50).Value = runUser;

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return result == null ? 0 : Convert.ToInt32(result);
        }
    }
}