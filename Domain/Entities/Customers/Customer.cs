using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Persons;
using Domain.Entities.Vehicles;
using Domain.Entities.Appointments;
using Domain.ValueObject.Customers.Customer;
namespace Domain.Entities.Customers
{

    public sealed class Customer : BaseEntity
    {
        public int            PersonId { get; private set; }
        public CustomerStatus Status   { get; private set; } = null!;

        public Person                              Person       { get; private set; } = null!;
        public ICollection<VehicleOwnershipHistory> Ownerships   { get; private set; } = [];
        public ICollection<Appointment>            Appointments { get; private set; } = [];

        private Customer() { }

        public Customer(int personId)
        {
            PersonId = personId > 0 ? personId : throw new ArgumentException("PersonId must be greater than 0.");
            Status   = CustomerStatus.Active;
        }

        public void Activate()   => Status = CustomerStatus.Active;
        public void Deactivate() => Status = CustomerStatus.Inactive;
    }
}