namespace CubeEnergy.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountId { get; set; } = GenerateAccountId();
        public string Country { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public decimal UnitBalance { get; set; } = 0;
        public decimal BillsUse { get; set; } = 0;
        public string? RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private static string GenerateAccountId()
        {
            Random random = new Random();
            long accountId = random.NextInt64(1000000000, 10000000000); // Generate a long number
            return "CE" + accountId.ToString(); // Convert to string and add prefix
        }
    }
}
