namespace CubeEnergy.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = GenerateReferenceNumber();
        public decimal Amount { get; set; }
        public string PayerName { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string PackageType { get; set; }
        public int Days { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Total { get; set; }

        private static string GenerateReferenceNumber()
        {
            Random random = new Random();
            int number = random.Next(100000, 999999);
            return $"WXRF-{number}";
        }
    }
}
