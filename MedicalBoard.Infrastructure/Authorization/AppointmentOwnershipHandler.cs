using System.Security.Claims;
using MedicalBoard.Domain.Constants;
using MedicalBoard.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace MedicalBoard.Infrastructure.Authorization;

// Milestone 11 -- resource-based authorization: a Doctor may access an Appointment
// only when it is assigned to them, unless they hold Appointment.ManageAll.
public class AppointmentOwnershipRequirement : IAuthorizationRequirement { }

public class AppointmentOwnershipHandler : AuthorizationHandler<AppointmentOwnershipRequirement, Appointment>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AppointmentOwnershipRequirement requirement,
        Appointment resource)
    {
        if (context.User.HasClaim("permission", PermissionCodes.AppointmentManageAll))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var doctorIdClaim = context.User.FindFirstValue("DoctorId");
        if (int.TryParse(doctorIdClaim, out var doctorId) && doctorId == resource.DoctorId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}