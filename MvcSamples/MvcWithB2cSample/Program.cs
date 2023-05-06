using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Web;
using System;
using Microsoft.Extensions.Options;

namespace MvcWithB2cSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // default の実装
        //builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        //    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

        // カスタマイズして OnRemoteFailure で b2c のエラー時のハンドリングをする
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                builder.Configuration.Bind("b2c", options);
                options.Events.OnRemoteFailure = context =>
                {
                    if (context.Failure != null && context.Failure.Message.Contains("AADB2C90091"))
                    {
                        context.Response.Redirect("/Home/Privacy");
                    }
                    else
                    {
                        context.Response.Redirect("/");
                    }
                    return Task.FromResult(0);
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
        builder.Services.AddRazorPages()
            .AddMicrosoftIdentityUI();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}

//public class CustomAuthenticationEvents : OpenIdConnectEvents
//{
//    public Func<RemoteFailureContext, Task> CustomOnRemoteFailure { get; set; } = context =>
//    {
//        if (context.Failure != null && context.Failure.Message.Contains("AADB2C90091"))
//        {
//            context.Response.Redirect("/Home/Privacy");
//        }
//        else
//        {
//            context.Response.Redirect("/");
//        }
//        return Task.FromResult(0);
//    };
//    public override Task RemoteFailure(RemoteFailureContext context) => CustomOnRemoteFailure(context);
//}