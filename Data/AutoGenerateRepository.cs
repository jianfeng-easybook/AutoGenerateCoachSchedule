using AutoGenerateCoachSchedule.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;

namespace AutoGenerateCoachSchedule.Data
{
    public class AutoGenerateRepository
    {
        private readonly IConfiguration _configuration;

        public AutoGenerateRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("TestDBConn");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string is missing.");

            return connectionString;
        }

        public async Task<IList<AutoGenerateSchedule>> GetActiveSchedulesAsync(string connString, CancellationToken cancellationToken)
        {
            var result = new List<AutoGenerateSchedule>();

            const string sql = @"
            SELECT
                AutoGenerateSchedules_ID,
                Schedule_GUID,
                SequenceGUID,
                Coach_ID,
                From_Date,
                To_Date,
                From_SubPlaceID,
                To_SubPlaceID,
                From_SubPlaceName,
                To_SubPlaceName,
                Departure_Date,
                ExcludedDays,
                ExcludedDates,
                Status,
                LastRunDate,
                RecurrenceType
            FROM dbo.AutoGenerateSchedules
            WHERE Status = 1";

            await using var conn = new SqlConnection(connString);
            await using var cmd = new SqlCommand(sql, conn);

            await conn.OpenAsync(cancellationToken);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new AutoGenerateSchedule
                {
                    AutoGenerateSchedules_ID = GetInt32(reader, "AutoGenerateSchedules_ID")!.Value,
                    Schedule_GUID = GetString(reader, "Schedule_GUID"),
                    SequenceGUID = GetString(reader, "SequenceGUID"),
                    Coach_ID = GetInt32(reader, "Coach_ID"),
                    From_Date = GetDateTime(reader, "From_Date"),
                    To_Date = GetDateTime(reader, "To_Date"),
                    From_SubPlaceID = GetString(reader, "From_SubPlaceID"),
                    To_SubPlaceID = GetString(reader, "To_SubPlaceID"),
                    From_SubPlaceName = GetString(reader, "From_SubPlaceName"),
                    To_SubPlaceName = GetString(reader, "To_SubPlaceName"),
                    Departure_Date = GetDateTime(reader, "Departure_Date"),
                    ExcludedDays = GetString(reader, "ExcludedDays"),
                    ExcludedDates = GetString(reader, "ExcludedDates"),
                    Status = GetInt32(reader, "Status"),
                    LastRunDate = GetDateTime(reader, "LastRunDate"),
                    RecurrenceType = GetInt32(reader, "RecurrenceType")
                });
            }

            return result;
        }

        public async Task<IList<AutoGenerateCoachScheduleTemplate>> GetTemplatesAsync(
            int autoGenerateSchedulesId,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            var result = new List<AutoGenerateCoachScheduleTemplate>();

            const string sql = @"
            SELECT
               [AutoGenerateCoachSchedule_ID]
              ,[CoachSchedule_ID]
              ,[Coach_ID]
              ,[Departure_Date]
              ,[TicketCost]
              ,[FromPlace]
              ,[FromSubPlace]
              ,[ToPlace]
              ,[ToSubPlace]
              ,[Create_Date]
              ,[Create_User]
              ,[Update_Date]
              ,[Update_User]
              ,[Time_Stamp]
              ,[GUID]
              ,[Publish_Date]
              ,[Currency]
              ,[FromSubPlaceAddID]
              ,[ToSubPlaceAddID]
              ,[Status]
              ,[Display]
              ,[MealProvided]
              ,[Schedule_DiscountType]
              ,[Schedule_Discount]
              ,[Schedule_AdminChargeType]
              ,[Schedule_AdminCharge]
              ,[AutoSeat]
              ,[SubCompany]
              ,[Remark]
              ,[phyCoachID]
              ,[actualCoachTypeID]
              ,[Paper_Price]
              ,[HideFromAgent]
              ,[TicketCostCSD]
              ,[AgentCommission]
              ,[GroupGUID]
              ,[Freeze]
              ,[websiteRemark]
              ,[Template_ID]
              ,[AutoOff]
              ,[AutoOff_Value]
              ,[AutoOff_Type]
              ,[AutoOff_Enable]
              ,[Active_Status]
              ,[SequenceGUID]
              ,[CoachType_Alias]
              ,[OnlineTicketCurrency]
              ,[OnlineTicketCost]
              ,[OnlineTicketCostCSD]
              ,[TransType]
              ,[SubCompany_ID]
              ,[Platform_Number]
              ,[OnlineRouteTrip_Adult]
              ,[OnlineRouteTrip_CSD]
              ,[CounterRouteTrip_Adult]
              ,[CounterRouteTrip_CSD]
              ,[SeatAvailability]
              ,[SeatFeaturesPrice]
              ,[TransferNote]
              ,[SeatFeaturesPrices]
              ,[IsOpenTime]
              ,[IsOpenDay]
              ,[PortTax]
              ,[TotalSeats]
              ,[AutoGenerateSchedules_ID]
              ,[Schedule_Discount_TwoWay]
              ,[Schedule_DiscountType_TwoWay]
              ,[IsVisibleEIMA]
            FROM dbo.AutoGenerateCoachScheduleTemplate
            WHERE AutoGenerateSchedules_ID = @AutoGenerateSchedules_ID;";

            await using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.Add("@AutoGenerateSchedules_ID", SqlDbType.Int).Value = autoGenerateSchedulesId;

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(new AutoGenerateCoachScheduleTemplate
                {
                    AutoGenerateCoachSchedule_ID = GetInt32(reader, "AutoGenerateCoachSchedule_ID")!.Value,
                    CoachSchedule_ID = GetInt32(reader, "CoachSchedule_ID"),
                    Coach_ID = GetInt32(reader, "Coach_ID"),
                    Departure_Date = GetDateTime(reader, "Departure_Date"),
                    TicketCost = GetDecimal(reader, "TicketCost"),

                    FromPlace = GetString(reader, "FromPlace"),
                    FromSubPlace = GetString(reader, "FromSubPlace"),
                    ToPlace = GetString(reader, "ToPlace"),
                    ToSubPlace = GetString(reader, "ToSubPlace"),

                    Create_Date = GetDateTime(reader, "Create_Date"),
                    Create_User = GetString(reader, "Create_User"),
                    Update_Date = GetDateTime(reader, "Update_Date"),
                    Update_User = GetString(reader, "Update_User"),

                    Time_Stamp = GetBytes(reader, "Time_Stamp"),

                    GUID = GetString(reader, "GUID"),
                    Publish_Date = GetDateTime(reader, "Publish_Date"),
                    Currency = GetString(reader, "Currency"),

                    FromSubPlaceAddID = GetInt32(reader, "FromSubPlaceAddID"),
                    ToSubPlaceAddID = GetInt32(reader, "ToSubPlaceAddID"),
                    Status = GetInt32(reader, "Status"),
                    Display = GetInt32(reader, "Display"),
                    MealProvided = GetInt32(reader, "MealProvided"),

                    Schedule_DiscountType = GetString(reader, "Schedule_DiscountType"),
                    Schedule_Discount = GetDecimal(reader, "Schedule_Discount"),

                    Schedule_AdminChargeType = GetString(reader, "Schedule_AdminChargeType"),
                    Schedule_AdminCharge = GetDecimal(reader, "Schedule_AdminCharge"),

                    AutoSeat = GetInt32(reader, "AutoSeat"),
                    SubCompany = GetString(reader, "SubCompany"),
                    Remark = GetString(reader, "Remark"),
                    phyCoachID = GetInt32(reader, "phyCoachID"),
                    actualCoachTypeID = GetInt32(reader, "actualCoachTypeID"),

                    Paper_Price = GetString(reader, "Paper_Price"),
                    HideFromAgent = GetString(reader, "HideFromAgent"),

                    TicketCostCSD = GetDecimal(reader, "TicketCostCSD"),
                    AgentCommission = GetDecimal(reader, "AgentCommission"),

                    GroupGUID = GetString(reader, "GroupGUID"),
                    Freeze = GetBoolean(reader, "Freeze"),

                    websiteRemark = GetString(reader, "websiteRemark"),
                    Template_ID = GetInt32(reader, "Template_ID"),
                    AutoOff = GetInt32(reader, "AutoOff"),
                    AutoOff_Value = GetInt32(reader, "AutoOff_Value"),
                    AutoOff_Type = GetString(reader, "AutoOff_Type"),
                    AutoOff_Enable = GetBoolean(reader, "AutoOff_Enable"),
                    Active_Status = GetInt32(reader, "Active_Status"),

                    SequenceGUID = GetString(reader, "SequenceGUID"),
                    CoachType_Alias = GetString(reader, "CoachType_Alias"),

                    OnlineTicketCurrency = GetString(reader, "OnlineTicketCurrency"),
                    OnlineTicketCost = GetDecimal(reader, "OnlineTicketCost"),
                    OnlineTicketCostCSD = GetDecimal(reader, "OnlineTicketCostCSD"),

                    TransType = GetString(reader, "TransType"),
                    SubCompany_ID = GetInt32(reader, "SubCompany_ID"),
                    Platform_Number = GetString(reader, "Platform_Number"),

                    OnlineRouteTrip_Adult = GetDecimal(reader, "OnlineRouteTrip_Adult"),
                    OnlineRouteTrip_CSD = GetDecimal(reader, "OnlineRouteTrip_CSD"),
                    CounterRouteTrip_Adult = GetDecimal(reader, "CounterRouteTrip_Adult"),
                    CounterRouteTrip_CSD = GetDecimal(reader, "CounterRouteTrip_CSD"),

                    SeatAvailability = GetString(reader, "SeatAvailability"),
                    SeatFeaturesPrice = GetString(reader, "SeatFeaturesPrice"),
                    TransferNote = GetString(reader, "TransferNote"),
                    SeatFeaturesPrices = GetString(reader, "SeatFeaturesPrices"),

                    IsOpenTime = GetBoolean(reader, "IsOpenTime"),
                    IsOpenDay = GetBoolean(reader, "IsOpenDay"),

                    PortTax = GetDecimal(reader, "PortTax"),
                    TotalSeats = GetInt32(reader, "TotalSeats"),

                    AutoGenerateSchedules_ID = GetInt32(reader, "AutoGenerateSchedules_ID"),
                    Schedule_Discount_TwoWay = GetDecimal(reader, "Schedule_Discount_TwoWay"),
                    Schedule_DiscountType_TwoWay = GetString(reader, "Schedule_DiscountType_TwoWay"),
                    IsVisibleEIMA = GetBoolean(reader, "IsVisibleEIMA")
                });
            }

            return result;
        }

        public async Task UpdateLastRunDateAsync(
            int autoGenerateSchedulesId,
            DateTime lastRunDate,
            string runUser,
            SqlConnection conn,
            SqlTransaction tx,
            CancellationToken cancellationToken)
        {
            const string sql = @"
            UPDATE dbo.AutoGenerateSchedules
            SET LastRunDate = @LastRunDate,
                Update_Date = GETDATE(),
                Update_User = @Update_User
            WHERE AutoGenerateSchedules_ID = @AutoGenerateSchedules_ID;";

            await using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.Add("@LastRunDate", SqlDbType.Date).Value = lastRunDate.Date;
            cmd.Parameters.Add("@Update_User", SqlDbType.VarChar, 50).Value = runUser;
            cmd.Parameters.Add("@AutoGenerateSchedules_ID", SqlDbType.Int).Value = autoGenerateSchedulesId;

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static int? GetInt32(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is int intValue)
                return intValue;

            if (value is short shortValue)
                return shortValue;

            if (value is long longValue)
                return checked((int)longValue);

            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                if (int.TryParse(stringValue, out var parsed))
                    return parsed;
            }

            throw new FormatException(
                string.Format("Column '{0}' contains value '{1}' that cannot be converted to int.", column, value));
        }

        private static DateTime? GetDateTime(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is DateTime dt)
                return dt;

            if (DateTime.TryParse(Convert.ToString(value), out var parsed))
                return parsed;

            throw new FormatException(
                string.Format("Column '{0}' contains value '{1}' that cannot be converted to DateTime.", column, value));
        }

        private static string? GetString(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            return Convert.ToString(reader.GetValue(ordinal));
        }

        private static bool? GetBoolean(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is bool boolValue)
                return boolValue;

            if (value is byte byteValue)
                return byteValue != 0;

            if (value is short shortValue)
                return shortValue != 0;

            if (value is int intValue)
                return intValue != 0;

            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                if (bool.TryParse(stringValue, out var parsedBool))
                    return parsedBool;

                if (stringValue == "1")
                    return true;

                if (stringValue == "0")
                    return false;
            }

            throw new FormatException(
                string.Format("Column '{0}' contains value '{1}' that cannot be converted to bool.", column, value));
        }

        private static byte[]? GetBytes(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return null;

            return (byte[])reader.GetValue(ordinal);
        }

        private static decimal? GetDecimal(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);

            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is decimal decimalValue)
                return decimalValue;

            if (value is double doubleValue)
                return Convert.ToDecimal(doubleValue);

            if (value is float floatValue)
                return Convert.ToDecimal(floatValue);

            if (value is int intValue)
                return Convert.ToDecimal(intValue);

            if (value is long longValue)
                return Convert.ToDecimal(longValue);

            if (value is short shortValue)
                return Convert.ToDecimal(shortValue);

            if (value is byte byteValue)
                return Convert.ToDecimal(byteValue);

            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;

                decimal parsed;
                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed))
                    return parsed;

                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out parsed))
                    return parsed;
            }

            throw new FormatException(
                string.Format("Column '{0}' contains value '{1}' that cannot be converted to decimal.", column, value));
        }
    }
}