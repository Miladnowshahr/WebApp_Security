using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data;
public class ApplicationDbContext:IdentityDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option ):base(option)
	{

	}
}