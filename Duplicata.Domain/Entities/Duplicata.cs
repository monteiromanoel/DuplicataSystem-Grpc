using Duplicata.Domain.Enums;
using Duplicata.Domain.Exceptions;

namespace Duplicata.Domain.Entities
{
    public class Duplicata
    {
        public Guid Id { get; private set; }
        public string Numero { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime Vencimento { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DuplicataStatus Status { get; private set; }

        private Duplicata() { } // EF

        public Duplicata(string numero, decimal valor, DateTime vencimento)
        {
            Id = Guid.NewGuid();
            Numero = numero;
            Valor = valor;
            Vencimento = DateTime.SpecifyKind(vencimento, DateTimeKind.Utc);
            CriadoEm = DateTime.UtcNow;
            Status = DuplicataStatus.ARegistrar;
        }

        // === Regras de Negócio ===

        public void Registrar()
        {
            if (Status != DuplicataStatus.ARegistrar)
                throw new DomainException("Duplicata não pode ser registrada.");

            Status = DuplicataStatus.Registrada;
        }

        public void Baixar()
        {
            if (Status != DuplicataStatus.Registrada)
                throw new DomainException("Só duplicata registrada pode ser baixada.");

            Status = DuplicataStatus.Baixada;
        }

        public void Cancelar()
        {
            if (Status == DuplicataStatus.Baixada)
                throw new DomainException("Duplicata baixada não pode ser cancelada.");

            Status = DuplicataStatus.Cancelada;
        }
    }

}
