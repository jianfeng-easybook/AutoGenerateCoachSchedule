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
            IF EXISTS (
                SELECT 1
                FROM dbo.Coach_Seat
                WHERE Schedule_GUID = @Schedule_GUID
            )
            BEGIN
                SELECT 0;
                RETURN;
            END

            INSERT INTO dbo.Coach_Seat
            (
                Coach_ID,
                Schedule_GUID,
                Seat_Position_X,
                Seat_Position_Y,
                Seat_Number,
                Seat_Status,
                Deck,
                IsBed,
                Rowspan,
                Colspan,
                DeckName,
                DeckID,
                CabinID,
                SeatFeatures,
                Seat_Price,
                Release_Date,
                Create_Date,
                Create_User,
                Update_Date,
                Update_User
            )
            SELECT
                c.Coach_ID,
                @Schedule_GUID,
                cst.Seat_Position_X,
                cst.Seat_Position_Y,
                cst.Seat_Number,
                cst.Status,
                cst.Deck,
                cst.IsBed,
                cst.Rowspan,
                cst.Colspan,
                cst.DeckName,
                cst.DeckID,
                cst.CabinID,
                cst.SeatFeatures,
                NULL,
                NULL,
                GETDATE(),
                @RunUser,
                GETDATE(),
                @RunUser
            FROM dbo.Coach_Seat_Template cst
            INNER JOIN dbo.CoachType ct
                ON cst.CoachTypeID = ct.CoachTypeID
            INNER JOIN dbo.Coaches c
                ON c.CoachTypeID = ct.CoachTypeID
            WHERE c.Coach_ID = @Coach_ID;

            SELECT @@ROWCOUNT;";

            await using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.Add("@Coach_ID", SqlDbType.Int).Value = coachId;
            cmd.Parameters.Add("@Schedule_GUID", SqlDbType.VarChar, 50).Value = scheduleGuid;
            cmd.Parameters.Add("@RunUser", SqlDbType.VarChar, 100).Value = runUser;

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }
    }
}
