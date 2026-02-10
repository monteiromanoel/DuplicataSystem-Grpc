using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicata.Application.DTOs
{
    public class CreateDuplicataDto
    {
        public string Numero { get; set; } = default!;
        public decimal Valor { get; set; }
        public DateTime Vencimento { get; set; }
    }
}
