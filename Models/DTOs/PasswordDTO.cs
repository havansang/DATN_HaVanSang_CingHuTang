namespace CingHuTang.Models.DTOs
{
    public class PasswordDTO
    {
        public int AccountID { get; set; }
        public string OldPassword{ get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
