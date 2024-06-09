using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.ShoppingCarts.Incomings
{
    public class InputShoppingCartDto
    {
        [Required]
        [IsValidGuid]
        public Guid CartId { get; set; }

        [Required]
        [IsValidGuid]
        public Guid GuestId { get; set; }

        public IList<AddCartItemDto> CartItems { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        ///     Deserialize the input <paramref name="jsonValue"/> string to rebuild 
        ///     a <see cref="InputShoppingCartDto"/> instance.
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        public static InputShoppingCartDto ConvertFromJson(string jsonValue)
        {
            if (string.IsNullOrEmpty(jsonValue))
            {
                return new InputShoppingCartDto
                {
                    CartId = Guid.NewGuid(),
                    CartItems = new List<AddCartItemDto>()
                };
            }

            try
            {
                var shoppingCart = JsonSerializer.Deserialize<InputShoppingCartDto>(jsonValue);

                return shoppingCart;
            }
            catch (Exception)
            {
                return new InputShoppingCartDto
                {
                    CartId = Guid.NewGuid(),
                    CartItems = new List<AddCartItemDto>()
                };
            }
        }

        public bool IsEmpty()
        {
            return (CartItems == null) || (CartItems.Count == 0);
        }

        public void Clear()
        {
            CartItems.Clear();
        }

        public IEnumerable<Guid> GetProductIdList()
        {
            if (IsEmpty())
            {
                return Enumerable.Empty<Guid>();
            }

            return CartItems.Select(x => x.ProductId).ToList();
        }

        public void RemoveItem(Guid productId)
        {
            if (Equals(CartItems, null) || IsEmpty())
            {
                return;
            }

            for (int itemIndex = 0; itemIndex < CartItems.Count; itemIndex++)
            {
                var existedItem = CartItems[itemIndex];

                if (existedItem.ProductId.Equals(productId))
                {
                    CartItems.RemoveAt(itemIndex);

                    break;
                }
            }
        }

        /// <summary>
        ///     Verify if the input <paramref name="guestId"/> is
        ///     similar with this shopping cart <see cref="GuestId"/>
        /// </summary>
        /// <param name="guestId">
        ///     The input customerId to verify.
        /// </param>
        /// <returns>
        ///     The boolean value that presents the verification result.
        /// </returns>
        public bool VerifyGuestId(Guid guestId)
        {
            return GuestId.Equals(guestId);
        }

        /// <summary>
        ///     Get the current item's quantity in this shopping cart.
        /// </summary>
        public int GetItemQuantity(Guid productId)
        {
            var product = CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (Equals(product, null))
            {
                return 0;
            }

            return product.Quantity;
        }

        /// <summary>
        ///     Add the input <paramref name="cartItem"/> to this shopping cart.
        /// </summary>
        /// <param name="cartItem"></param>
        public void AddItem(AddCartItemDto cartItem)
        {
            if (Equals(CartItems, null))
            {
                CartItems = new List<AddCartItemDto> { cartItem };

                return;
            }

            bool isItemExisted = false;
            for (int itemIndex = 0; itemIndex < CartItems.Count; itemIndex++)
            {
                var existedItem = CartItems[itemIndex];

                if (existedItem.ProductId == cartItem.ProductId)
                {
                    isItemExisted = true;

                    existedItem.Quantity += cartItem.Quantity;

                    break;
                }
            }

            if (!isItemExisted)
            {
                CartItems.Add(cartItem);
            }
        }

        /// <summary>
        ///     Decrease the input <paramref name="cartItem"/> to this shopping cart.
        /// </summary>
        /// <param name="cartItem"></param>
        public void DecreaseItem(DecreaseCartItemDto cartItem)
        {
            if (IsEmpty())
            {
                return;
            }

            for (int itemIndex = 0; itemIndex < CartItems.Count; itemIndex++)
            {
                var existedItem = CartItems[itemIndex];

                if (existedItem.ProductId == cartItem.ProductId)
                {
                    existedItem.Quantity -= cartItem.Quantity;

                    // If the left quantity is less than 0,
                    // remove that item from this shopping cart.
                    if (existedItem.Quantity <= 0)
                    {
                        CartItems.RemoveAt(itemIndex);
                    }

                    break;
                }
            }
        }
    }
}
