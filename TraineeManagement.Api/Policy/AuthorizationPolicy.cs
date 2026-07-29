using TraineeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;


namespace TraineeManagement.Api.Policy;

public static class AuthorizationPolicy
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(nameof(UserRole.Admin)));

            options.AddPolicy("AdminOrMentor", policy =>
                policy.RequireRole(
                    nameof(UserRole.Admin),
                    nameof(UserRole.Mentor)));

            options.AddPolicy("AdminOrMentorOrTrainee", policy =>
                policy.RequireRole(
                    nameof(UserRole.Admin),
                    nameof(UserRole.Mentor),
                    nameof(UserRole.Trainee)));
        });

        return services;
    }
}