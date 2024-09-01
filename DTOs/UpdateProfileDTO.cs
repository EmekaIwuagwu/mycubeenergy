namespace CubeEnergy.DTOs
{
    public class UpdateProfileDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }
}
