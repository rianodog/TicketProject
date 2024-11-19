namespace TicketProject.Models
{
    public static class Enums
    {
        public enum UserRoles
        {
            Admin,
            User
        }

        public enum TicketType
        {
            Normal,
            VIP
        }

        public enum ResultStatus
        {
            Success,
            Sold_out,
            Error
        }

        public enum City
        {
            台北,
            台中,
            台南
        }
    }
}
