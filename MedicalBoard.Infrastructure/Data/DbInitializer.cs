using MedicalBoard.Domain.Constants;
using MedicalBoard.Domain.Entities;
using MedicalBoard.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalBoard.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        IPasswordHasher<User> hasher)
    {
        // 1. Make sure the database schema exists and is up to date.
        await db.Database.MigrateAsync();

        // 2. Seed the permission catalog.
        await SeedPermissionsAsync(db);

        // 3. Seed roles.
        await SeedRolesAsync(db);

        // 4. Connect roles to permissions.
        await SeedRolePermissionsAsync(db);

        // 5. Seed the development administrator account.
        await SeedAdminUserAsync(db, hasher);

        // 6. Seed development/test domain data.
        await SeedDevelopmentDataAsync(db);
    }

    // ---------------------------------------------------------
    // PERMISSIONS
    // ---------------------------------------------------------

    private static async Task SeedPermissionsAsync(ApplicationDbContext db)
    {
        var permissionDefinitions = new Dictionary<string, (string Name, string? Description)>
        {
            [PermissionCodes.UserView] =
                ("View Users", "Allows viewing users."),

            [PermissionCodes.UserCreate] =
                ("Create Users", "Allows creating users."),

            [PermissionCodes.UserEdit] =
                ("Edit Users", "Allows editing users."),

            [PermissionCodes.UserDeactivate] =
                ("Deactivate Users", "Allows activating and deactivating users."),


            [PermissionCodes.RoleView] =
                ("View Roles", "Allows viewing roles."),

            [PermissionCodes.RoleEdit] =
                ("Edit Roles", "Allows creating and editing roles."),

            [PermissionCodes.RoleAssignPermission] =
                ("Assign Permissions", "Allows assigning permissions to roles."),


            [PermissionCodes.DoctorView] =
                ("View Doctors", "Allows viewing doctors."),

            [PermissionCodes.DoctorCreate] =
                ("Create Doctors", "Allows creating doctors."),

            [PermissionCodes.DoctorEdit] =
                ("Edit Doctors", "Allows editing doctors."),

            [PermissionCodes.DoctorDeactivate] =
                ("Deactivate Doctors", "Allows activating and deactivating doctors."),


            [PermissionCodes.PatientView] =
                ("View Patients", "Allows viewing patients."),

            [PermissionCodes.PatientCreate] =
                ("Create Patients", "Allows creating patients."),

            [PermissionCodes.PatientEdit] =
                ("Edit Patients", "Allows editing patients."),


            [PermissionCodes.AppointmentView] =
                ("View Appointments", "Allows viewing appointments."),

            [PermissionCodes.AppointmentCreate] =
                ("Create Appointments", "Allows creating appointments."),

            [PermissionCodes.AppointmentEdit] =
                ("Edit Appointments", "Allows editing appointments."),

            [PermissionCodes.AppointmentConfirm] =
                ("Confirm Appointments", "Allows confirming appointments."),

            [PermissionCodes.AppointmentCancel] =
                ("Cancel Appointments", "Allows cancelling appointments."),

            [PermissionCodes.AppointmentComplete] =
                ("Complete Appointments", "Allows completing appointments."),

            [PermissionCodes.AppointmentManageAll] =
                ("Manage All Appointments", "Allows managing appointments across the system."),


            [PermissionCodes.ReportView] =
                ("View Reports", "Allows viewing reports.")
        };

        foreach (var definition in permissionDefinitions)
        {
            var code = definition.Key;
            var name = definition.Value.Name;
            var description = definition.Value.Description;

            var permission = await db.Permissions
                .FirstOrDefaultAsync(p => p.Code == code);

            if (permission is null)
            {
                db.Permissions.Add(new Permission
                {
                    Code = code,
                    Name = name,
                    Description = description,
                    IsActive = true
                });
            }
            else
            {
                // Keep the database definition synchronized with the code.
                permission.Name = name;
                permission.Description = description;
                permission.IsActive = true;
            }
        }

        await db.SaveChangesAsync();
    }

    // ---------------------------------------------------------
    // ROLES
    // ---------------------------------------------------------

    private static async Task SeedRolesAsync(ApplicationDbContext db)
    {
        var administrator = await db.Roles
            .FirstOrDefaultAsync(r => r.Name == "Administrator");

        if (administrator is null)
        {
            db.Roles.Add(new Role
            {
                Name = "Administrator",
                Description = "Full system access",
                IsActive = true
            });

            await db.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------
    // ROLE → PERMISSION
    // ---------------------------------------------------------

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext db)
    {
        var administrator = await db.Roles
            .SingleAsync(r => r.Name == "Administrator");

        var permissions = await db.Permissions
            .Where(p => p.IsActive)
            .ToListAsync();

        foreach (var permission in permissions)
        {
            var alreadyAssigned = await db.RolePermissions
                .AnyAsync(rp =>
                    rp.RoleId == administrator.Id &&
                    rp.PermissionId == permission.Id);

            if (!alreadyAssigned)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = administrator.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await db.SaveChangesAsync();
    }

    // ---------------------------------------------------------
    // ADMIN USER
    // ---------------------------------------------------------

    private static async Task SeedAdminUserAsync(
        ApplicationDbContext db,
        IPasswordHasher<User> hasher)
    {
        var admin = await db.Users
            .FirstOrDefaultAsync(u => u.Username == "admin");

        if (admin is null)
        {
            admin = new User
            {
                Username = "admin",
                Email = "admin@medicalboard.local",
                IsActive = true
            };

            admin.PasswordHash =
                hasher.HashPassword(admin, "ChangeMe123!");

            db.Users.Add(admin);

            await db.SaveChangesAsync();
        }

        var administrator = await db.Roles
            .SingleAsync(r => r.Name == "Administrator");

        var alreadyAssigned = await db.UserRoles
            .AnyAsync(ur =>
                ur.UserId == admin.Id &&
                ur.RoleId == administrator.Id);

        if (!alreadyAssigned)
        {
            db.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = administrator.Id
            });

            await db.SaveChangesAsync();
        }
    }

    // ---------------------------------------------------------
    // DEVELOPMENT / TEST DATA
    // ---------------------------------------------------------

    private static async Task SeedDevelopmentDataAsync(
        ApplicationDbContext db)
    {
        await SeedDepartmentsAsync(db);
        await SeedDoctorsAsync(db);
        await SeedPatientsAsync(db);
        await SeedAppointmentsAsync(db);
    }

    private static async Task SeedDepartmentsAsync(
        ApplicationDbContext db)
    {
        if (await db.Departments.AnyAsync())
            return;

        db.Departments.AddRange(
            new Department
            {
                Name = "Cardiology",
                Code = "CARD",
                Description = "Cardiology department",
                IsActive = true
            },
            new Department
            {
                Name = "Neurology",
                Code = "NEUR",
                Description = "Neurology department",
                IsActive = true
            },
            new Department
            {
                Name = "Pediatrics",
                Code = "PED",
                Description = "Pediatrics department",
                IsActive = true
            }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedDoctorsAsync(
        ApplicationDbContext db)
    {
        if (await db.Doctors.AnyAsync())
            return;

        var cardiology = await db.Departments
            .SingleAsync(d => d.Code == "CARD");

        var neurology = await db.Departments
            .SingleAsync(d => d.Code == "NEUR");

        var pediatrics = await db.Departments
            .SingleAsync(d => d.Code == "PED");

        db.Doctors.AddRange(
            new Doctor
            {
                EmployeeNumber = "DOC-001",
                FullName = "Dr. Ahmad Hassan",
                Specialty = "Cardiology",
                Phone = "0790000001",
                Email = "ahmad.hassan@medicalboard.local",
                DepartmentId = cardiology.Id,
                IsActive = true
            },
            new Doctor
            {
                EmployeeNumber = "DOC-002",
                FullName = "Dr. Sara Khalil",
                Specialty = "Neurology",
                Phone = "0790000002",
                Email = "sara.khalil@medicalboard.local",
                DepartmentId = neurology.Id,
                IsActive = true
            },
            new Doctor
            {
                EmployeeNumber = "DOC-003",
                FullName = "Dr. Omar Ali",
                Specialty = "Pediatrics",
                Phone = "0790000003",
                Email = "omar.ali@medicalboard.local",
                DepartmentId = pediatrics.Id,
                IsActive = true
            }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedPatientsAsync(
        ApplicationDbContext db)
    {
        if (await db.Patients.AnyAsync())
            return;

        db.Patients.AddRange(
            new Patient
            {
                PatientNumber = "P000001",
                FullName = "John Smith",
                NationalIdentifier = "TEST-001",
                DateOfBirth = new DateOnly(1990, 5, 15),
                Phone = "0791000001",
                Email = "john.smith@example.local",
                IsActive = true
            },
            new Patient
            {
                PatientNumber = "P000002",
                FullName = "Jane Smith",
                NationalIdentifier = "TEST-002",
                DateOfBirth = new DateOnly(1985, 9, 22),
                Phone = "0791000002",
                Email = "jane.smith@example.local",
                IsActive = true
            },
            new Patient
            {
                PatientNumber = "P000003",
                FullName = "Michael Brown",
                NationalIdentifier = "TEST-003",
                DateOfBirth = new DateOnly(1978, 2, 10),
                Phone = "0791000003",
                Email = "michael.brown@example.local",
                IsActive = true
            }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedAppointmentsAsync(
        ApplicationDbContext db)
    {
        if (await db.Appointments.AnyAsync())
            return;

        var doctor1 = await db.Doctors
            .SingleAsync(d => d.EmployeeNumber == "DOC-001");

        var doctor2 = await db.Doctors
            .SingleAsync(d => d.EmployeeNumber == "DOC-002");

        var patient1 = await db.Patients
            .SingleAsync(p => p.PatientNumber == "P000001");

        var patient2 = await db.Patients
            .SingleAsync(p => p.PatientNumber == "P000002");

        var admin = await db.Users
            .SingleAsync(u => u.Username == "admin");

        db.Appointments.AddRange(
            new Appointment
            {
                DoctorId = doctor1.Id,
                PatientId = patient1.Id,
                AppointmentDate = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
                Status = AppointmentStatus.Pending,
                Notes = "Development test appointment",
                CreatedByUserId = admin.Id
            },
            new Appointment
            {
                DoctorId = doctor2.Id,
                PatientId = patient2.Id,
                AppointmentDate = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                Status = AppointmentStatus.Confirmed,
                Notes = "Development test appointment",
                CreatedByUserId = admin.Id
            }
        );

        await db.SaveChangesAsync();
    }
}