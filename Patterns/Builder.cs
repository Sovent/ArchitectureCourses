using System;
using System.Collections.Generic;
using System.Linq;

namespace Patterns
{
    public class Order
    {
        public string Customer { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public class OrderItem
        {
            public OrderItem(string id)
            {
                Id = id;
            }

            public string Id { get; private set; }
        }
    }

    public class OrderBuilder
    {
        private readonly ICustomerDatabase _customerDatabase;
        private readonly IPhoneNumberValidator _phoneNumberValidator;
        private readonly IWarehouse _warehouse;
        private readonly Order _order = new Order();

        public OrderBuilder(ICustomerDatabase customerDatabase, IPhoneNumberValidator phoneNumberValidator, IWarehouse warehouse)
        {
            _customerDatabase = customerDatabase;
            _phoneNumberValidator = phoneNumberValidator;
            _warehouse = warehouse;
        }

        public OrderBuilder WithCustomer(string name)
        {
            if (!_customerDatabase.CustomerExist(name))
            {
                throw new InvalidOperationException("No customer with such a name");
            }

            _order.Customer = name;
            return this;
        }

        public OrderBuilder WithPhoneNumber(string phoneNumber)
        {
            if (!_phoneNumberValidator.IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException("Invalid phone number");
            }

            _order.PhoneNumber = phoneNumber;
            return this;
        }

        public OrderBuilder WithAddress(string address)
        {
            _order.Address = address;
            return this;
        }

        public OrderBuilder WithPhoneNumber(long phoneNumber)
        {
            return WithPhoneNumber(phoneNumber.ToString());
        }

        public OrderBuilder WithOrderItem(Order.OrderItem orderItem)
        {
            if (!_warehouse.HasItem(orderItem.Id))
            {
                throw new InvalidOperationException("No items of this id on warehouse");
            }

            if (!_order.OrderItems.Any(item => item.Id == orderItem.Id))
            {
                _order.OrderItems.Add(orderItem);
            }

            return this;
        }

        public Order Build()
        {
            var prerequisites = new Dictionary<Func<bool>, string>
            {
                {() => !_order.OrderItems.Any(), "No items in order" },
                {() => string.IsNullOrEmpty(_order.PhoneNumber), "Phone number is empty" },
                {() => string.IsNullOrEmpty(_order.Customer), "Customer is empty" }
            };

            foreach (var prerequisite in prerequisites.Where(prerequisite => prerequisite.Key()))
            {
                throw new InvalidOperationException(prerequisite.Value);
            }

            return _order;
        }
    }

    public interface IWarehouse
    {
        bool HasItem(string itemId);
    }

    public interface IPhoneNumberValidator
    {
        bool IsValidPhoneNumber(string phoneNumber);
    }

    public interface ICustomerDatabase
    {
        bool CustomerExist(string customerName);
    }
}