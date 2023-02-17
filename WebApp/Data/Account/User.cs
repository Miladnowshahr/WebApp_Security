using Microsoft.AspNetCore.Identity;

public class User:IdentityUser
{
    public string Departement { get; set; }
    public string Position { get; set; }
}