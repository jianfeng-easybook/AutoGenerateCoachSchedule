namespace AutoGenerateCoachSchedule.Models
{
    public class AutoGenerateCoachScheduleTemplate
    {
        public int AutoGenerateCoachSchedule_ID { get; set; }
        public int? CoachSchedule_ID { get; set; }
        public int? Coach_ID { get; set; }
        public DateTime? Departure_Date { get; set; }
        public decimal? TicketCost { get; set; }

        public string? FromPlace { get; set; }
        public string? FromSubPlace { get; set; }
        public string? ToPlace { get; set; }
        public string? ToSubPlace { get; set; }

        public DateTime? Create_Date { get; set; }
        public string? Create_User { get; set; }
        public DateTime? Update_Date { get; set; }
        public string? Update_User { get; set; }

        public byte[]? Time_Stamp { get; set; }

        public string? GUID { get; set; }
        public DateTime? Publish_Date { get; set; }
        public string? Currency { get; set; }

        public int? FromSubPlaceAddID { get; set; }
        public int? ToSubPlaceAddID { get; set; }
        public int? Status { get; set; }
        public int? Display { get; set; }
        public int? MealProvided { get; set; }

        public string? Schedule_DiscountType { get; set; }
        public decimal? Schedule_Discount { get; set; }

        public string? Schedule_AdminChargeType { get; set; }
        public decimal? Schedule_AdminCharge { get; set; }

        public int? AutoSeat { get; set; }
        public string? SubCompany { get; set; }
        public string? Remark { get; set; }
        public int? phyCoachID { get; set; }
        public int? actualCoachTypeID { get; set; }

        public string? Paper_Price { get; set; }
        public string? HideFromAgent { get; set; }

        public decimal? TicketCostCSD { get; set; }
        public decimal? AgentCommission { get; set; }

        public string? GroupGUID { get; set; }
        public bool? Freeze { get; set; }

        public string? websiteRemark { get; set; }
        public int? Template_ID { get; set; }
        public int? AutoOff { get; set; }
        public int? AutoOff_Value { get; set; }
        public string? AutoOff_Type { get; set; }
        public bool? AutoOff_Enable { get; set; }
        public int? Active_Status { get; set; }

        public string? SequenceGUID { get; set; }
        public string? CoachType_Alias { get; set; }

        public string? OnlineTicketCurrency { get; set; }
        public decimal? OnlineTicketCost { get; set; }
        public decimal? OnlineTicketCostCSD { get; set; }

        public string? TransType { get; set; }
        public int? SubCompany_ID { get; set; }
        public string? Platform_Number { get; set; }

        public decimal? OnlineRouteTrip_Adult { get; set; }
        public decimal? OnlineRouteTrip_CSD { get; set; }
        public decimal? CounterRouteTrip_Adult { get; set; }
        public decimal? CounterRouteTrip_CSD { get; set; }

        public string? SeatAvailability { get; set; }
        public string? SeatFeaturesPrice { get; set; }
        public string? TransferNote { get; set; }
        public string? SeatFeaturesPrices { get; set; }

        public bool? IsOpenTime { get; set; }
        public bool? IsOpenDay { get; set; }

        public decimal? PortTax { get; set; }
        public int? TotalSeats { get; set; }

        public int? AutoGenerateSchedules_ID { get; set; }
        public decimal? Schedule_Discount_TwoWay { get; set; }
        public string? Schedule_DiscountType_TwoWay { get; set; }
        public bool? IsVisibleEIMA { get; set; }
    }
}