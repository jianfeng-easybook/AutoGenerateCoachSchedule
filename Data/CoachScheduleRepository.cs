using AutoGenerateCoachSchedule.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AutoGenerateCoachSchedule.Data
{
    public class CoachScheduleRepository
    {
        public async Task<bool> ExistsAsync(
            int? coachId,
            DateTime departureDate,
            string? fromPlace,
            string? fromSubPlace,
            string? toPlace,
            string? toSubPlace,
            string? sequenceGuid,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT COUNT(1)
            FROM dbo.CoachSchedule
            WHERE Coach_ID = @Coach_ID
              AND CAST(Departure_Date AS date) = @Departure_Date
              AND ISNULL(FromPlace, -1) = ISNULL(@FromPlace, -1)
              AND ISNULL(FromSubPlace, -1) = ISNULL(@FromSubPlace, -1)
              AND ISNULL(ToPlace, -1) = ISNULL(@ToPlace, -1)
              AND ISNULL(ToSubPlace, -1) = ISNULL(@ToSubPlace, -1)
              AND ISNULL(CAST(SequenceGUID AS varchar(50)), '') = ISNULL(CAST(@SequenceGUID AS varchar(50)), '');";

            await using var cmd = new SqlCommand(sql, conn, tx);

            cmd.Parameters.Add("@Coach_ID", SqlDbType.Int).Value = coachId;
            cmd.Parameters.Add("@Departure_Date", SqlDbType.Date).Value = departureDate.Date;
            cmd.Parameters.Add("@FromPlace", SqlDbType.Int).Value = (object?)fromPlace ?? DBNull.Value;
            cmd.Parameters.Add("@FromSubPlace", SqlDbType.Int).Value = (object?)fromSubPlace ?? DBNull.Value;
            cmd.Parameters.Add("@ToPlace", SqlDbType.Int).Value = (object?)toPlace ?? DBNull.Value;
            cmd.Parameters.Add("@ToSubPlace", SqlDbType.Int).Value = (object?)toSubPlace ?? DBNull.Value;
            cmd.Parameters.Add("@SequenceGUID", SqlDbType.UniqueIdentifier).Value = (object?)sequenceGuid ?? DBNull.Value;

            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) > 0;
        }

        public async Task InsertAsync(
            CoachSchedule item,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO dbo.CoachSchedule
            (
                Coach_ID,
                Departure_Date,
                TicketCost,
                FromPlace,
                FromSubPlace,
                ToPlace,
                ToSubPlace,
                Create_Date,
                Create_User,
                Update_Date,
                Update_User,
                GUID,
                Publish_Date,
                Currency,
                FromSubPlaceAddID,
                ToSubPlaceAddID,
                Status,
                Display,
                MealProvided,
                Schedule_DiscountType,
                Schedule_Discount,
                Schedule_AdminChargeType,
                Schedule_AdminCharge,
                AutoSeat,
                SubCompany,
                Remark,
                phyCoachID,
                actualCoachTypeID,
                Paper_Price,
                HideFromAgent,
                TicketCostCSD,
                AgentCommission,
                GroupGUID,
                Freeze,
                websiteRemark,
                Template_ID,
                AutoOff,
                AutoOff_Value,
                AutoOff_Type,
                AutoOff_Enable,
                Active_Status,
                SequenceGUID,
                CoachType_Alias,
                OnlineTicketCurrency,
                OnlineTicketCost,
                OnlineTicketCostCSD,
                SubCompany_ID,
                Platform_Number,
                SeatAvailability,
                OnlineRouteTrip_Adult,
                OnlineRouteTrip_CSD,
                CounterRouteTrip_Adult,
                CounterRouteTrip_CSD,
                SeatFeaturesPrice,
                TransferNote,
                SeatFeaturesPrices,
                IsOpenTime,
                IsOpenDay,
                TransType,
                PortTax,
                TotalSeats,
                Schedule_Discount_TwoWay,
                Schedule_DiscountType_TwoWay,
                IsVisibleEIMA
            )
            VALUES
            (
                @Coach_ID,
                @Departure_Date,
                @TicketCost,
                @FromPlace,
                @FromSubPlace,
                @ToPlace,
                @ToSubPlace,
                @Create_Date,
                @Create_User,
                @Update_Date,
                @Update_User,
                @GUID,
                @Publish_Date,
                @Currency,
                @FromSubPlaceAddID,
                @ToSubPlaceAddID,
                @Status,
                @Display,
                @MealProvided,
                @Schedule_DiscountType,
                @Schedule_Discount,
                @Schedule_AdminChargeType,
                @Schedule_AdminCharge,
                @AutoSeat,
                @SubCompany,
                @Remark,
                @phyCoachID,
                @actualCoachTypeID,
                @Paper_Price,
                @HideFromAgent,
                @TicketCostCSD,
                @AgentCommission,
                @GroupGUID,
                @Freeze,
                @websiteRemark,
                @Template_ID,
                @AutoOff,
                @AutoOff_Value,
                @AutoOff_Type,
                @AutoOff_Enable,
                @Active_Status,
                @SequenceGUID,
                @CoachType_Alias,
                @OnlineTicketCurrency,
                @OnlineTicketCost,
                @OnlineTicketCostCSD,
                @SubCompany_ID,
                @Platform_Number,
                @SeatAvailability,
                @OnlineRouteTrip_Adult,
                @OnlineRouteTrip_CSD,
                @CounterRouteTrip_Adult,
                @CounterRouteTrip_CSD,
                @SeatFeaturesPrice,
                @TransferNote,
                @SeatFeaturesPrices,
                @IsOpenTime,
                @IsOpenDay,
                @TransType,
                @PortTax,
                @TotalSeats,
                @Schedule_Discount_TwoWay,
                @Schedule_DiscountType_TwoWay,
                @IsVisibleEIMA
            );";

            await using var cmd = new SqlCommand(sql, conn, tx);

            cmd.Parameters.AddWithValue("@Coach_ID", item.Coach_ID);
            cmd.Parameters.AddWithValue("@Departure_Date", item.Departure_Date);
            cmd.Parameters.AddWithValue("@TicketCost", (object?)item.TicketCost ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromPlace", (object?)item.FromPlace ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromSubPlace", (object?)item.FromSubPlace ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToPlace", (object?)item.ToPlace ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToSubPlace", (object?)item.ToSubPlace ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Create_Date", item.Create_Date);
            cmd.Parameters.AddWithValue("@Create_User", item.Create_User);
            cmd.Parameters.AddWithValue("@Update_Date", item.Update_Date);
            cmd.Parameters.AddWithValue("@Update_User", item.Update_User);
            cmd.Parameters.AddWithValue("@GUID", item.GUID);
            cmd.Parameters.AddWithValue("@Publish_Date", (object?)item.Publish_Date ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Currency", (object?)item.Currency ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromSubPlaceAddID", (object?)item.FromSubPlaceAddID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToSubPlaceAddID", (object?)item.ToSubPlaceAddID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object?)item.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Display", (object?)item.Display ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MealProvided", (object?)item.MealProvided ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_DiscountType", (object?)item.Schedule_DiscountType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_Discount", (object?)item.Schedule_Discount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_AdminChargeType", (object?)item.Schedule_AdminChargeType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_AdminCharge", (object?)item.Schedule_AdminCharge ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoSeat", (object?)item.AutoSeat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubCompany", (object?)item.SubCompany ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remark", (object?)item.Remark ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@phyCoachID", (object?)item.phyCoachID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@actualCoachTypeID", (object?)item.actualCoachTypeID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Paper_Price", (object?)item.Paper_Price ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HideFromAgent", (object?)item.HideFromAgent ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TicketCostCSD", (object?)item.TicketCostCSD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AgentCommission", (object?)item.AgentCommission ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GroupGUID", (object?)item.GroupGUID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Freeze", (object?)item.Freeze ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@websiteRemark", (object?)item.websiteRemark ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Template_ID", (object?)item.Template_ID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoOff", (object?)item.AutoOff ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoOff_Value", (object?)item.AutoOff_Value ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoOff_Type", (object?)item.AutoOff_Type ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoOff_Enable", (object?)item.AutoOff_Enable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Active_Status", (object?)item.Active_Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SequenceGUID", (object?)item.SequenceGUID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CoachType_Alias", (object?)item.CoachType_Alias ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OnlineTicketCurrency", (object?)item.OnlineTicketCurrency ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OnlineTicketCost", (object?)item.OnlineTicketCost ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OnlineTicketCostCSD", (object?)item.OnlineTicketCostCSD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubCompany_ID", (object?)item.SubCompany_ID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Platform_Number", (object?)item.Platform_Number ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeatAvailability", (object?)item.SeatAvailability ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OnlineRouteTrip_Adult", (object?)item.OnlineRouteTrip_Adult ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OnlineRouteTrip_CSD", (object?)item.OnlineRouteTrip_CSD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CounterRouteTrip_Adult", (object?)item.CounterRouteTrip_Adult ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CounterRouteTrip_CSD", (object?)item.CounterRouteTrip_CSD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeatFeaturesPrice", (object?)item.SeatFeaturesPrice ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TransferNote", (object?)item.TransferNote ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeatFeaturesPrices", (object?)item.SeatFeaturesPrices ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsOpenTime", (object?)item.IsOpenTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsOpenDay", (object?)item.IsOpenDay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TransType", (object?)item.TransType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PortTax", (object?)item.PortTax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TotalSeats", (object?)item.TotalSeats ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_Discount_TwoWay", (object?)item.Schedule_Discount_TwoWay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Schedule_DiscountType_TwoWay", (object?)item.Schedule_DiscountType_TwoWay ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsVisibleEIMA", (object?)item.IsVisibleEIMA ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}