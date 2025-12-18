using Ergasia_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Ergasia_API.Authorization.Requirements;

public class SameUserOrAdminHandler(UserManager<User> accountManager) : AuthorizationHandler<SameUserOrAdminRequirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement sameUserOrAdminRequirement, string resourceUserId)
    {
        var currentUserId = GetUserId(context);
        if (IsValidUserId(currentUserId, resourceUserId) || UserIsAdmin(context))
            context.Succeed(sameUserOrAdminRequirement);
        
        return Task.CompletedTask;
    }

    private string? GetUserId(AuthorizationHandlerContext context)
    {
        return accountManager.GetUserId(context.User);
    }

    private static bool IsValidUserId(string? userId, string resourceUserId)
    {
        return !string.IsNullOrEmpty(userId) && resourceUserId == userId;
    }

    private static bool UserIsAdmin(AuthorizationHandlerContext context)
    {
        return context.User.IsInRole("Admin");
    }
}