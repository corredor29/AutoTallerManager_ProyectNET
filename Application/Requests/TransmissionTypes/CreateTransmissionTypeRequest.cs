using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.TransmissionTypes
{
    public sealed class CreateTransmissionTypeRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; } = string.Empty;
    }
}