using Microsoft.AspNetCore.Authorization;
namespace EnterpriseSaaS.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{ protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement) { if (context.User.HasClaim("permission", requirement.Permission)) context.Succeed(requirement); return Task.CompletedTask; } }
public static class PermissionPolicy { public const string Prefix = "permission:"; public static string For(string permission) => Prefix + permission; }
