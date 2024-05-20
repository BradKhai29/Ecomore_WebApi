namespace WebApi.DTOs.Implementation.Orders.Outgoings
{
    public class OrderDto
    {
        /// <summary>
        ///     Gets and sets the order code for this order.
        /// </summary>
        /// <remarks>
        ///     This order code can be used for 
        ///     later payment cancel or payment approve.
        /// </remarks>
        public long OrderCode { get; set; }

        public IEnumerable<OrderItemDto> Items { get; set; }

        /// <summary>
        ///     Gets and sets the userId of this order.
        /// </summary>
        /// <remarks>
        ///     If the user has signed in, this property 
        ///     will be the same as the signed-in user.
        ///     <br/>
        ///     If the user has not signed in, this property
        ///     will be set with default value <see cref="DataAccess.DataSeedings.AppUsers.DoNotRemove.Id"/>.
        /// </remarks>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Gets and sets the guestId of this order.
        /// </summary>
        /// <remarks>
        ///     If the user has signed in, this property 
        ///     will be the same as the signed-in user.
        ///     <br/>
        ///     If the user has not signed in, this property
        ///     will be set by the value in guest_id cookie.
        /// </remarks>
        public Guid GuestId { get; set; }

        /// <summary>
        ///     Gets and sets the first name of the customer who
        ///     will purchase this order.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets and sets the last name of the customer who
        ///     will purchase this order.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Gets and sets the email of the customer who
        ///     will purchase this order.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets and sets the phone number of the customer who
        ///     will purchase this order.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Gets and sets the delivery address of the customer who
        ///     will purchase this order.
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        ///     This property will decide if the customer want
        ///     to save this information for their next payment or not.
        /// </summary>
        /// <remarks>
        ///     This property is only applied for the signed-in user.
        /// </remarks>
        public bool SaveInformation { get; set; }

        /// <summary>
        ///     Gets and sets the order note of the customer who
        ///     will purchase this order.
        /// </summary>
        public string OrderNote { get; set; }

        /// <summary>
        ///     Gets and sets the created time of this order.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     Gets and sets the expired time of this order.
        /// </summary>
        /// <remarks>
        ///     After the expired time, this order 
        ///     will be removed and cannot be purchased.
        /// </remarks>
        public DateTime ExpiredAt { get; set; }
    }
}
