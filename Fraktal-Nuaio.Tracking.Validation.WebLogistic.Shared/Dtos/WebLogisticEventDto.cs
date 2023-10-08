namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Dtos
{
    public class WebLogisticEventDto
    {
        public string EventType { get; set; } = null!;
        public bool ClaimAvailable { get; set; }
        public int Order { get; set; }
        public string ReturnReasonName { get; set; } = null!;
        public int StatusCode { get; set; }
        public string StatusDate { get; set; } = null!;
        public string StatusName { get; set; } = null!;
        public string Ucid { get; set; } = null!;
    }
}
