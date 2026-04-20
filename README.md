# BlindMatch | System Setup Guide

Welcome to the BlindMatch project. Follow these steps to initialize the database and run the application on your local machine.

## 🛠️ System Setup Instructions

1. **Open the Project:** Open the `BlindMatch.sln` solution file in Visual Studio, or open the root folder in Visual Studio Code.
2. **Database Configuration:** Open the `appsettings.json` file inside the `BlindMatch.Web` project. Verify that the `DefaultConnection` string is pointing to your local Microsoft SQL Server instance.
3. **Generate the Database:** You must run Entity Framework Core migrations to build the SQL tables. 
   * If using **Visual Studio Package Manager Console**, run:
     ```powershell
     Update-Database
     ```
   * If using the **Command Line / Terminal**, run:
     ```bash
     dotnet ef database update
     ```
4. **Run the Application:** Start the server. On the very first boot, the custom `DbInitializer` will automatically seed the database with the required security roles and the master Admin account.

---

## 🔐 Default Login Credentials

The system is locked down using Role-Based Access Control (RBAC). Use the following pre-configured accounts to test the different dashboards. 

**Master Password for ALL accounts:** `TempPass123!`

| System Role | Email / Username | Access Level |
| :--- | :--- | :--- |
| **Module Leader (Admin)** | `admin@blindmatch.com` | Full system oversight, manual user approval, and force-unmatch capabilities. |
| **Supervisor** | `supervisor@blindmatch.com` | Blind review dashboard, research area filtering, and project claiming. |
| **Student** | `student@blindmatch.com` | Project submission, editing (while pending), and match-reveal tracking. |
Passwords 
admin@blindmatch.com | Admin123!
rdsdranasinghe@gmail.com | Admin123!
supervisor@test.com | Password123!

