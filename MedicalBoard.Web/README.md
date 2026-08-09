## 1. Add connection string
For security the connection string does not exist in `appsettings.json`.
Add it manually by typing this command in the terminal:
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=medicalboard_db;Username=medicalboard_user;Password="
```

---

## 2. Apply Migrations
To build the migrations files:
```bash
dotnet ef migrations add MedicalBoardMigration --output-dir Migrations
```

To apply migrations to the database:
```bash
dotnet ef database update
```

---

## The ERD

```mermaid
erDiagram

    DEPARTMENT {
        int Id PK
        string Name
        string Code
        string Description
        bool IsActive
        datetime CreatedAt
    }

    DOCTOR {
        int Id PK
        string EmployeeNumber
        string FullName
        string Specialty
        string Phone
        string Email
        int DepartmentId FK
        bool IsActive
        datetime CreatedAt
    }

    PATIENT {
        int Id PK
        string PatientNumber
        string FullName
        string NationalIdentifier
        date DateOfBirth
        string Phone
        string Email
        bool IsActive
        datetime CreatedAt
    }

    USER {
        int Id PK
        string Username
        string Email
        string PasswordHash
        int DoctorId FK
        int DepartmentId FK
        bool IsActive
        datetime LastLoginAt
        datetime CreatedAt
    }

    ROLE {
        int Id PK
        string Name
        string Description
        bool IsActive
        datetime CreatedAt
    }

    PERMISSION {
        int Id PK
        string Code
        string Name
        string Description
        bool IsActive
    }

    USER_ROLE {
        int UserId PK, FK
        int RoleId PK, FK
        datetime CreatedAt
    }

    ROLE_PERMISSION {
        int RoleId PK, FK
        int PermissionId PK, FK
    }

    APPOINTMENT {
        int Id PK
        int DoctorId FK
        int PatientId FK
        int CreatedByUserId FK
        datetime AppointmentDate
        string Status
        string Notes
        datetime CreatedAt
        datetime UpdatedAt
        datetime CancelledAt
        string CancellationReason
    }

    %% One-to-Many
    DEPARTMENT ||--o{ DOCTOR : has
    DEPARTMENT ||--o{ USER : contains

    DOCTOR ||--o{ APPOINTMENT : attends
    PATIENT ||--o{ APPOINTMENT : books
    USER ||--o{ APPOINTMENT : creates

    %% One-to-One
    DOCTOR ||--o| USER : login

    %% Many-to-Many
    USER ||--o{ USER_ROLE : has
    ROLE ||--o{ USER_ROLE : assigned

    ROLE ||--o{ ROLE_PERMISSION : grants
    PERMISSION ||--o{ ROLE_PERMISSION : includes
```

## Files Written By Me:
Data/ApplicationContext.cs    
Data/Configurations/*.cs    
Enums/*.cs    
Models/*.cs    
Program.cs    
README.md

---

- you can verify the schema by `psql` CLI tool or by opening the database with `pgAdmin`.