using AutoGenerateCoachSchedule.Models;

namespace AutoGenerateCoachSchedule.Services
{
    public class CoachScheduleFactory
    {
        public CoachSchedule Create(
            PreparedScheduleRow preparedRow,
            string runUser)
        {
            var now = DateTime.Now;

            var template = preparedRow.SourceTemplate;
            var finalDepartureDateTime = preparedRow.DepartureDateTime;

            if (string.IsNullOrWhiteSpace(preparedRow.GeneratedGuid))
                throw new InvalidOperationException("Prepared row GeneratedGuid was not assigned.");

            if (string.IsNullOrWhiteSpace(preparedRow.GeneratedGroupGuid))
                throw new InvalidOperationException("Prepared row GeneratedGroupGuid was not assigned.");

            return new CoachSchedule
            {
                Coach_ID = template.Coach_ID,
                Departure_Date = finalDepartureDateTime,
                TicketCost = template.TicketCost,
                FromPlace = template.FromPlace,
                FromSubPlace = template.FromSubPlace,
                ToPlace = template.ToPlace,
                ToSubPlace = template.ToSubPlace,
                Create_Date = now,
                Create_User = runUser,
                Update_Date = now,
                Update_User = runUser,
                GUID = preparedRow.GeneratedGuid,
                Publish_Date = finalDepartureDateTime,
                Currency = template.Currency,
                FromSubPlaceAddID = template.FromSubPlaceAddID,
                ToSubPlaceAddID = template.ToSubPlaceAddID,
                Status = template.Status,
                Display = template.Display,
                MealProvided = template.MealProvided,
                Schedule_DiscountType = template.Schedule_DiscountType,
                Schedule_Discount = template.Schedule_Discount,
                Schedule_AdminChargeType = template.Schedule_AdminChargeType,
                Schedule_AdminCharge = template.Schedule_AdminCharge,
                AutoSeat = template.AutoSeat,
                SubCompany = template.SubCompany,
                Remark = template.Remark,
                phyCoachID = template.phyCoachID,
                actualCoachTypeID = template.actualCoachTypeID,
                Paper_Price = template.Paper_Price,
                HideFromAgent = template.HideFromAgent,
                TicketCostCSD = template.TicketCostCSD,
                AgentCommission = template.AgentCommission,
                GroupGUID = preparedRow.GeneratedGroupGuid,
                Freeze = template.Freeze,
                websiteRemark = template.websiteRemark,
                Template_ID = template.Template_ID,
                AutoOff = template.AutoOff,
                AutoOff_Value = template.AutoOff_Value,
                AutoOff_Type = template.AutoOff_Type,
                AutoOff_Enable = template.AutoOff_Enable,
                Active_Status = template.Active_Status,
                SequenceGUID = template.SequenceGUID,
                CoachType_Alias = template.CoachType_Alias,
                OnlineTicketCurrency = template.OnlineTicketCurrency,
                OnlineTicketCost = template.OnlineTicketCost,
                OnlineTicketCostCSD = template.OnlineTicketCostCSD,
                SubCompany_ID = template.SubCompany_ID,
                Platform_Number = template.Platform_Number,
                SeatAvailability = preparedRow.SeatAvailability,
                OnlineRouteTrip_Adult = template.OnlineRouteTrip_Adult,
                OnlineRouteTrip_CSD = template.OnlineRouteTrip_CSD,
                CounterRouteTrip_Adult = template.CounterRouteTrip_Adult,
                CounterRouteTrip_CSD = template.CounterRouteTrip_CSD,
                SeatFeaturesPrice = template.SeatFeaturesPrice,
                TransferNote = template.TransferNote,
                SeatFeaturesPrices = template.SeatFeaturesPrices,
                IsOpenTime = template.IsOpenTime,
                IsOpenDay = template.IsOpenDay,
                TransType = template.TransType,
                PortTax = template.PortTax,
                TotalSeats = template.TotalSeats,
                Schedule_Discount_TwoWay = template.Schedule_Discount_TwoWay,
                Schedule_DiscountType_TwoWay = template.Schedule_DiscountType_TwoWay,
                IsVisibleEIMA = template.IsVisibleEIMA
            };
        }

    }
}
