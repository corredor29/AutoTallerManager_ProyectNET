using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MileageHistory
{
    public sealed class UpdateMileageHistoryRequest
    {
        [StringLength(500)]
        public string? Notes { get; init; }
    }
}