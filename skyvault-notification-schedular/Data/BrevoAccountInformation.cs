namespace skyvault_notification_schedular.Data
{
    public class BrevoAccountInformation
    {
        public string CompanyName { get; set; } = String.Empty;
        public string CompanyEmailAddress { get; set; } = String.Empty;
        public List<BrevoPlan> Plans { get; set; } = [];
    }


}
