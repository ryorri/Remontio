using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Objects.DTOs.CalculationsDTO
{
    public class CreateCalculationDTO
    {
        public required string Name { get; set; }
        public required float Value { get; set; }
        public required CalculationsTypeEnum Type { get; set; }
    }
}
