using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class Users
    {
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public bool ChangePassword { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public bool IsLocked { get; set; }
        public int LogInAttemptCount { get; set; }
        public DateTime? LastFailedLogInAttempt { get; set; }
        public int? ClientId { get; set; }
        public int? BranchId { get; set; }
        public bool IsActive { get; set; }
        public int? EntryBy { get; set; }
        public DateTime EntryDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string T24userName { get; set; }

        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }

    public class StoreAppData
    {
        public string code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class ManageAppUserData
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UserRoleData
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UserRole { get; set; }
        public string UserRoleName { get; set; }
    }

    public class AddUserData
    {
        public string Username { get; set; }
        public int UserRole { get; set; }
    }
}
