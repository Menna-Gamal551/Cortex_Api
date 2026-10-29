using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cortex_Api.Data.Models.DTOs
{
    public class UpdateAccountMetaDataDto
    {
        
            public int Id { get; set; }
            [MaxLength(150)]
            public string? UserName { get; set; }
            [EmailAddress]
            [MaxLength(254)]
            public string? Email { get; set; }

            [MaxLength(500)]
            public string? WebsiteURL { get; set; }
            [MaxLength(150)]
            public string ServiceName { get; set; }
            public bool HasTOTP { get; set; }
           

    }
}
