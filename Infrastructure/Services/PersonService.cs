using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.DTOs.Persons;
using Application.Requests.Persons;
using Domain.Entities.Persons;
using Domain.ValueObject.Persons.Person;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public sealed class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly AutoTallerDbContext _dbContext;

        public PersonService(IPersonRepository personRepository,
                            AutoTallerDbContext dbContext)
        {
            _personRepository = personRepository;
            _dbContext        = dbContext;
        }

        public async Task<IEnumerable<PersonDto>> GetAllAsync()
        {
            var persons = await _personRepository.GetAllAsync();
            return persons.Select(MapToDto);
        }

        public async Task<PersonDto?> GetByIdAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            return person is null ? null : MapToDto(person);
        }

        public async Task<PersonDto> CreateAsync(CreatePersonRequest request)
        {
            var person = new Person(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName)
            );

            await _personRepository.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            return MapToDto(person);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePersonRequest request)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person is null) return false;

            person.Update(
                new PersonFirstName(request.FirstName),
                new PersonLastName(request.LastName)
            );

            _personRepository.Update(person);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person is null) return false;

            if (await _dbContext.Customers.AnyAsync(x => x.PersonId == id))
                throw new InvalidOperationException($"Person {id} is a customer and cannot be deleted.");

            if (await _dbContext.Users.AnyAsync(x => x.PersonId == id))
                throw new InvalidOperationException($"Person {id} is a user and cannot be deleted.");

            _personRepository.Remove(person);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private static PersonDto MapToDto(Person person) => new()
        {
            Id           = person.Id,
            FirstName    = person.FirstName.Value,
            LastName     = person.LastName.Value,
            RegisteredAt = person.RegisteredAt
        };
    }
}