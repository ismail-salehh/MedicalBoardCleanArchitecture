namespace MedicalBoard.Domain.Constants;

public static class PermissionCodes
{
    public const string UserView = "User.View";
    public const string UserCreate = "User.Create";
    public const string UserEdit = "User.Edit";
    public const string UserDeactivate = "User.Deactivate";

    public const string RoleView = "Role.View";
    public const string RoleEdit = "Role.Edit";
    public const string RoleAssignPermission = "Role.AssignPermission";

    public const string DoctorView = "Doctor.View";
    public const string DoctorCreate = "Doctor.Create";
    public const string DoctorEdit = "Doctor.Edit";
    public const string DoctorDeactivate = "Doctor.Deactivate";

    public const string PatientView = "Patient.View";
    public const string PatientCreate = "Patient.Create";
    public const string PatientEdit = "Patient.Edit";

    public const string AppointmentView = "Appointment.View";
    public const string AppointmentCreate = "Appointment.Create";
    public const string AppointmentEdit = "Appointment.Edit";
    public const string AppointmentConfirm = "Appointment.Confirm";
    public const string AppointmentCancel = "Appointment.Cancel";
    public const string AppointmentComplete = "Appointment.Complete";
    public const string AppointmentManageAll = "Appointment.ManageAll";

    public const string ReportView = "Report.View";

    public static readonly IReadOnlyList<string> All = new[]
    {
        UserView, UserCreate, UserEdit, UserDeactivate,
        RoleView, RoleEdit, RoleAssignPermission,
        DoctorView, DoctorCreate, DoctorEdit, DoctorDeactivate,
        PatientView, PatientCreate, PatientEdit,
        AppointmentView, AppointmentCreate, AppointmentEdit, AppointmentConfirm,
        AppointmentCancel, AppointmentComplete, AppointmentManageAll,
        ReportView
    };
}
