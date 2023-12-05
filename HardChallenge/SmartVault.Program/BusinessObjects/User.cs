using System;

namespace SmartVault.Program.BusinessObjects
{
    public partial class User
    {
        public string FullName => $"{FirstName} {LastName}";
        public DateTime createdOn { get; set; }
    }
}
