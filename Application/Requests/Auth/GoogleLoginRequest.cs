using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests.Auth
{

    public sealed class GoogleLoginRequest
    {
        public string Email     { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName  { get; init; } = string.Empty;
    }
}