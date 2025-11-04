using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? DomainUserId { get; set; }
    }
}
