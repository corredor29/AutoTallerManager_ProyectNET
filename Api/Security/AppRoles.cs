namespace Api.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Mechanic = "Mechanic";
    public const string Receptionist = "Receptionist";

    public const string AdminOrMechanic = $"{Admin},{Mechanic}";
    public const string AdminOrReceptionist = $"{Admin},{Receptionist}";
    public const string Staff = $"{Admin},{Mechanic},{Receptionist}";
}
