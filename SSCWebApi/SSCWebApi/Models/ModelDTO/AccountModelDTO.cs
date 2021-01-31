namespace SSCWebApi.Models.ModelDTO
{
    public class RegistrationDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordDTO
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class AppRolesDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AppUsersDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string UserRole { get; set; }
        public string RoleId { get; set; }
       
    }


}