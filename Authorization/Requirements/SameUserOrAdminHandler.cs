using Ergasia_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Ergasia_API.Authorization.Requirements;

public class SameUserOrAdminHandler(UserManager<User> accountManager) : AuthorizationHandler<SameUserOrAdminRequirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement orAdminRequirement, string resourceUserId)
    {
        var currentUserId = accountManager.GetUserId(context.User);
        if (!string.IsNullOrEmpty(currentUserId) && currentUserId == resourceUserId)
            context.Succeed(orAdminRequirement);
        
        return Task.CompletedTask;
    }
}