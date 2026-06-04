using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Requests.Auth
{

    public sealed class GoogleTokenRequest
    {
        public string IdToken { get; init; } = string.Empty;
    }
}