namespace ImmCheck.Core.Models;

public static class AppRoles
{
    public const string Student = "Student";
    public const string DSO = "DSO";
    public const string Advisor = "Advisor";
    public const string Admin = "Admin";

    public static readonly string[] All = [Student, DSO, Advisor, Admin];
}
