using Microsoft.AspNetCore.Authentication.JwtBearer;
using Prism.ProAssistant.Business;

namespace Prism.ProAssistant.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBearer(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = "https://byprism.b2clogin.com/byprism.onmicrosoft.com/B2C_1_PRO_ASSISTANT/v2.0/";
                jwtOptions.Audience = EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_AD_CLIENT_ID");
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = AuthenticationFailed
                };
            });

        Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // TODO : LOG
            return Task.FromResult(0);
        }
    }
}