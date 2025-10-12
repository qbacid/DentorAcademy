# User Management System Documentation

## Overview
A complete user management system has been scaffolded for the DentorAcademy platform with the following features:

## Features Implemented

### 1. **Extended ApplicationUser Model**
Added the following fields to track user behavior:
- `MustChangePassword` - Forces users to change their password on first login
- `LastPasswordChangeDate` - Tracks when passwords were last changed
- `CreatedAt` - Timestamp when the user was created
- `LastLoginDate` - Tracks the last successful login

### 2. **Roles Defined**
The following roles have been created and are automatically seeded at application startup:
- **Admin** - Full system access including user management
- **Student** - Regular user taking quizzes
- **Teacher** - Can create and manage educational content
- **Authenticated User** - Basic authenticated access
- **Course Manager** - Manages courses and content

### 3. **User Management Service** (`UserManagementService.cs`)
Provides comprehensive user management functionality:
- `GetAllUsersAsync()` - List all users with their roles
- `GetUserByIdAsync(userId)` - Get specific user details
- `CreateUserAsync(dto)` - Create new users with optional random password generation
- `UpdateUserAsync(dto)` - Update user information and roles
- `DeleteUserAsync(userId)` - Remove users from the system
- `ResetPasswordAsync(userId)` - Generate new random password and force change
- `GenerateRandomPassword()` - Creates secure random passwords (12 characters with uppercase, lowercase, digits, and special characters)

### 4. **User Management UI** (`/user-management`)
Admin-only page featuring:
- **User List Table** displaying:
  - Username and email
  - Email confirmation status
  - Assigned roles
  - Password change requirement status
  - Creation date and last login
  - Action buttons (Edit, Reset Password, Delete)

- **Create/Edit User Modal** with:
  - Username and email fields
  - Option to generate random password (forced change on first login)
  - Manual password entry option
  - Multi-select role assignment
  - Validation and error handling

- **Password Reset Feature**:
  - Generates secure random password
  - Displays password to admin (shown only once)
  - Forces user to change on next login

### 5. **Change Password Page** (`/change-password`)
Allows users to change their password with:
- Current password verification
- New password with confirmation
- Password policy enforcement
- Automatic redirect after successful change
- Required for users with `MustChangePassword` flag

### 6. **Password Policy**
Simple and secure password requirements:
- Minimum 6 characters
- Must contain:
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one digit
  - At least one special character (!@#$%^&*)

### 7. **Login Flow Enhancement**
Updated login process to:
- Check if user must change password after successful authentication
- Redirect to `/change-password` if flag is set
- Track last login date
- Prevent access to other pages until password is changed

### 8. **Navigation Integration**
Added "User Management" link to the sidebar navigation:
- Visible only to Admin role users
- Uses Bootstrap icon (bi-people)
- Follows the same styling as other menu items

## Usage

### Creating a New User
1. Navigate to `/user-management` (Admin only)
2. Click "Create New User" button
3. Enter username and email
4. Choose to generate random password or set manual password
5. Select appropriate roles
6. Click "Create User"
7. If random password generated, copy it and provide to the user

### Resetting User Password
1. Go to user management page
2. Find the user in the list
3. Click the key icon (Reset Password)
4. Confirm the action
5. Copy the generated password displayed in the alert
6. Provide the new password to the user
7. User will be forced to change it on next login

### Changing Your Password
1. Log in with current credentials
2. If forced to change, automatically redirected to `/change-password`
3. Enter current password
4. Enter and confirm new password
5. Submit to update
6. Automatically redirected to home page

## Database Migration
A migration named `AddUserManagementFields` has been created and applied, adding the new columns to the `AspNetUsers` table:
- `must_change_password` (boolean)
- `last_password_change_date` (timestamp)
- `created_at` (timestamp)
- `last_login_date` (timestamp)

## Security Features
- **Random Password Generation**: Uses cryptographically secure random number generator
- **Forced Password Change**: Ensures users change temporary passwords
- **Role-Based Access Control**: User management restricted to Admin role
- **Password Policy Enforcement**: Ensures strong passwords
- **Email Uniqueness**: Prevents duplicate accounts
- **Lockout Support**: Account lockout after failed login attempts

## Files Created/Modified

### New Files:
- `/DTOs/UserDto.cs`
- `/DTOs/CreateUserDto.cs`
- `/DTOs/UpdateUserDto.cs`
- `/DTOs/UserManagementResult.cs`
- `/Services/UserManagementService.cs`
- `/Components/Pages/UserManagement.razor`
- `/Components/Pages/UserManagement.razor.css`
- `/Components/Pages/ChangePassword.razor`
- `/Components/Pages/ChangePassword.razor.css`
- `/Components/Shared/PasswordChangeEnforcer.razor` (optional middleware)

### Modified Files:
- `/Models/ApplicationUser.cs` - Added user management fields
- `/Program.cs` - Registered UserManagementService, updated password policy, ensured all roles are created
- `/Components/Layout/NavMenu.razor` - Added User Management link
- `/Components/Pages/Login.razor` - Added password change check and last login tracking

## Next Steps
- Integrate with Azure AD for enterprise authentication (future enhancement)
- Add Google/Facebook authentication providers
- Implement email notifications for password resets
- Add user profile management
- Implement audit logging for user management actions

