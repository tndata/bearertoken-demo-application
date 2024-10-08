using BearerToken___demo_project;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Security.Claims;

//**************************************************************************************
//Written by Tore Nestenius
//
//Visit my blog at http://nestenius.se to read the blog post about this handler.    
//
//Need help?
//explore my training and consulting services in .NET, Identity, Security, and
//architecture, please visit https://www.tn-data.se
//**************************************************************************************


var builder = WebApplication.CreateBuilder(args);

//Configure the authentication middleware
string defaultSchema = BearerTokenDefaults.AuthenticationScheme;
builder.Services.AddAuthentication(defaultSchema)
.AddBearerToken(o =>
{
    //Use our transparent protector
    o.BearerTokenProtector = new TicketDataFormat(new MyDataProtector().CreateProtector(""));
    o.RefreshTokenProtector = new TicketDataFormat(new MyDataProtector().CreateProtector(""));
});

//Uncomment this code to use the Cookie handler instead
//string defaultSchema = CookieAuthenticationDefaults.AuthenticationScheme;
//builder.Services.AddAuthentication(defaultSchema)
//.AddCookie();

var app = builder.Build();

app.UseAuthentication();


app.MapGet("/login", async context =>
{
    //SignIn the user. As a result you will be presented 
    var claims = new Claim[]
    {
                    //Standard claims
                    new(ClaimTypes.Name, "Tore Nestenius"),
                    new (ClaimTypes.Country, "Sweden"),
                    new (ClaimTypes.Email, "tore@tn-data.se"),

                    //Custom claims
                    new ("JobTitle", "Consultant and trainer"),
                    new ("JobLevel", "Senior"),
                    new ("webpage", "https://www.tn-data.se"),
    };

    var identity = new ClaimsIdentity(claims: claims, authenticationType: defaultSchema);

    var user = new ClaimsPrincipal(identity: identity);

    var authProperties = new AuthenticationProperties
    {
        IsPersistent = true
    };

    await context.SignInAsync(defaultSchema, user, authProperties);

    //Uncomment if you use cookies
    await context.Response.WriteAsync("<!DOCTYPE html><body>");
    await context.Response.WriteAsync("<h1>Logged in!</h1>");
});

app.MapGet("/logout", async context =>
{
    //The BearerToken handler does not handle SignOut, so this will not do anything.
    await context.SignOutAsync(defaultSchema);

});


app.MapGet("/", async context =>
{
    //Homepage
    await context.Response.WriteAsync($"<!DOCTYPE html><body>");

    await context.Response.WriteAsync("Hello World!<br>");

    if (!context.User.Identity?.IsAuthenticated ?? false)
        await context.Response.WriteAsync(@"<a href=""/login"">Login</a>");
    else
        await context.Response.WriteAsync(@"<a href=""/logout"">Logout</a>");

    DumpUser(context);
});


app.Run();


//Helper method to print out the user identity to the page
static void DumpUser(HttpContext context)
{
    ClaimsPrincipal user = context.User;

    if (user != null && user.Identities != null)
    {
        context.Response.WriteAsync("<h3>Identities</h3>");
        foreach (ClaimsIdentity identity in user.Identities)
        {
            context.Response.WriteAsync($"<h4>identity</h4>");
            context.Response.WriteAsync($"Name: {user?.Identity?.Name ?? "null"}<br>");
            context.Response.WriteAsync($"Is Authenticated: {user?.Identity?.IsAuthenticated}<br>");
            context.Response.WriteAsync($"AuthenticationType: {user?.Identity?.AuthenticationType ?? "null"}<br>");
        }
    }

    if (user != null && user.Claims != null)
    {
        context.Response.WriteAsync("<h4>Claims</h4>");
        context.Response.WriteAsync(@"<table>");
        context.Response.WriteAsync(@"<thead><tr><th>Type</th><th>Value</th><th>Issuer</th></tr></thead>");

        foreach (Claim claim in user.Claims)
        {
            context.Response.WriteAsync($"<tr>");
            context.Response.WriteAsync($"<td>{claim.Type}</td>");
            context.Response.WriteAsync($"<td>{claim.Value}</td>");
            context.Response.WriteAsync($"<td>{claim.Issuer}</td>");
            context.Response.WriteAsync($"</tr>");
        }

        context.Response.WriteAsync($"</table>");
    }
}
