namespace NoWasteOfMoney.Models.Entities
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Normal = "User";

        public static readonly HashSet<string> AllowedRoles = 
            new HashSet<string> { Admin, Normal };

        public static bool IsValid(string role) => AllowedRoles.Contains(role);
    }
}
