using Devices_tasks1_2_3.Data;
using Devices_tasks1_2_3.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);


// ----------------------------------------------------
// MVC
// ----------------------------------------------------

builder.Services.AddControllersWithViews();


// ----------------------------------------------------
// Repositories
// ----------------------------------------------------

builder.Services.AddScoped<DeviceRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<PhoneNumberRepository>();
builder.Services.AddScoped<PhoneNumberReservationRepository>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<UserRepository>();


// ----------------------------------------------------
// Cookie authentication
// ----------------------------------------------------

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";

        options.AccessDeniedPath = "/Account/Login";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;

        // Return 401 for API requests instead of
        // redirecting the API to the login page.
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);

            return Task.CompletedTask;
        };

        // Return 403 when an authenticated user
        // does not have permission.
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);

            return Task.CompletedTask;
        };
    });


// ----------------------------------------------------
// Require authentication for the whole application
// ----------------------------------------------------

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});


var app = builder.Build();


// ----------------------------------------------------
// HTTP request pipeline
// ----------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Authentication must come before authorization.
app.UseAuthentication();

app.UseAuthorization();


// ----------------------------------------------------
// MVC route
// ----------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();