using System.Reflection;
using Domain.Entities.Persons;
using Domain.Entities.Users;
using Domain.ValueObject.Persons.Person;
using Domain.ValueObject.Users.Role;
using Domain.ValueObject.Users.User;

namespace AutoTallerManager.Tests.Helpers;

internal static class TestEntityBuilder
{
    // Construye un objeto de prueba con valores validos por defecto.
    public static User BuildUser(
        int userId = 1,
        int personId = 1,
        string firstName = "Jane",
        string lastName = "Doe",
        string password = "Admin123!",
        params string[] roles)
    {
        var person = new Person(new PersonFirstName(firstName), new PersonLastName(lastName))
        {
            Id = personId
        };

        var user = new User(person.Id, new PasswordHash(BCrypt.Net.BCrypt.HashPassword(password)))
        {
            Id = userId
        };

        SetPrivateProperty(user, nameof(User.Person), person);

        var roleNames = roles.Length == 0 ? ["Admin"] : roles;

        for (var index = 0; index < roleNames.Length; index++)
        {
            var role = new Role(new RoleName(roleNames[index]))
            {
                Id = index + 1
            };

            var userRole = new UserRole(user.Id, role.Id);
            SetPrivateProperty(userRole, nameof(UserRole.User), user);
            SetPrivateProperty(userRole, nameof(UserRole.Role), role);

            user.UserRoles.Add(userRole);
            role.UserRoles.Add(userRole);
        }

        return user;
    }

    private static void SetPrivateProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
    {
        var property = typeof(TTarget).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on type '{typeof(TTarget).Name}'.");

        property.SetValue(target, value);
    }
}
