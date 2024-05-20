using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Orders.Incomings
{
    public class OrderBillingDetailDto : IDtoNormalization
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [IsValidPhoneNumber]
        public string PhoneNumber { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        public bool SaveInformation { get; set; }

        public string OrderNote { get; set; }

        public void NormalizeAllProperties()
        {
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            Email = Email.Trim();
            PhoneNumber = PhoneNumber.Trim();
            DeliveryAddress = DeliveryAddress.Trim();
            OrderNote = string.IsNullOrEmpty(OrderNote) ? "Không" : OrderNote.Trim();
        }
    }
}
