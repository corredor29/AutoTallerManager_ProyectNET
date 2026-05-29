using Mapster;
using Application.DTOs.Customers;
using Application.DTOs.Persons;
using Domain.Entities.Customers;
using Domain.Entities.Persons;

namespace Application.Mapping
{
    public static class MapsterConfig
    {
        public static void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Person, PersonDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.FirstName, src => src.FirstName.Value)
                .Map(dest => dest.LastName, src => src.LastName.Value)
                .Map(dest => dest.RegisteredAt, src => src.RegisteredAt);

            config.NewConfig<Customer, CustomerDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.PersonId, src => src.PersonId)
                .Map(dest => dest.IsActive, src => src.Status.IsActive)
                .Map(dest => dest.Person, src => src.Person);
        }
    }
}
