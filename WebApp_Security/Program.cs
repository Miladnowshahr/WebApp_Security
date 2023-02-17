using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication().AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan= TimeSpan.FromMinutes(5);
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("HRManagerOnly",
        policy => policy.RequireClaim("Department", "HR")
                        .RequireClaim("Manager")
                        .Requirements.Add(new HRManagerProbationRequirement(3)));

    option.AddPolicy("AdminOnly", policy =>
                                  policy.RequireClaim("Admin"));

    option.AddPolicy("MustBelongToHRDepartment",
           policy => policy.RequireClaim("Department", "HR"));
});

builder.Services.AddHttpClient("OurWebApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5290");
});
builder.Services.AddSession(option =>
{

});

builder.Services.AddSingleton<IAuthorizationHandler,HRManagerProbationReuqirementHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession():
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
