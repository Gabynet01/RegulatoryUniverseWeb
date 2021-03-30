using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class UserRoles
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Users User { get; set; }
    }
}
