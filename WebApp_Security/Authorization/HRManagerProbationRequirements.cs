using Microsoft.AspNetCore.Authorization;

public class HRManagerProbationRequirement : IAuthorizationRequirement
{
    public int ProbationMonths { get; }
    public HRManagerProbationRequirement(int probationMonths)
    {
        ProbationMonths = probationMonths;
    }
}

public class HRManagerProbationReuqirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
    {
        if (context.User.HasClaim(x => x.Type == "EmployeementDate") is false)
            return Task.CompletedTask;

        var empDate = DateTime.Parse(context.User.FindFirst("EmployeementDate").Value);
        var period = DateTime.Now - empDate;

        if (period.Days > 30 * requirement.ProbationMonths)
            context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}
