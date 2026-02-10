namespace Duplicata.Domain.Entities
{
    public class Duplicata
    {
        public Guid Id { get; private set; }
        public string Numero { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime Vencimento { get; private set; }
        public DateTime CriadoEm { get; private set; }

        protected Duplicata() { } // EF

        public Duplicata(string numero, decimal valor, DateTime vencimento)
        {
            Id = Guid.NewGuid();
            Numero = numero;
            Valor = valor;
            Vencimento = vencimento;
            CriadoEm = DateTime.UtcNow;
        }
    }

}
