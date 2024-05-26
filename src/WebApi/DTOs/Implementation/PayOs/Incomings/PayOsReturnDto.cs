using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;

namespace DTOs.Implementation.PayOs.Incomings
{
    public record PayOsReturnDto : IDtoNormalization
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public bool Cancel { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public long OrderCode { get; set; }

        public void NormalizeAllProperties()
        {
            Code = Code.Trim();
            Id = Id.Trim();
            Status = Status.Trim();
        }
    }
}
