using Microsoft.AspNetCore.Authorization;

namespace Ergasia_API.Authorization.Requirements;

public sealed class SameUserOrAdminRequirement : IAuthorizationRequirement { }