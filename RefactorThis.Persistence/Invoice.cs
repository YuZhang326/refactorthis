using System;
using System.Collections.Generic;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		private readonly InvoiceRepository _repository;
		public Invoice(InvoiceRepository repository)
		{
			_repository = repository;
			// init
			Payments = new List<Payment>();
		}

		public void Save()
		{
			_repository.SaveInvoice(this);
		}

        public string AddPayment(Payment payment)
        {
            // handle the "no payment needed"
            if (Amount == 0)
            {
                if (Payments == null || Payments.Count == 0)
                {
                    return "no payment needed";
                }
                throw new InvalidOperationException(
                    "The invoice is in an invalid state, it has an amount of 0 and it has payments."
                );
            }

            // if already fully paid
            if (AmountPaid == Amount)
            {
                return "invoice was already fully paid";
            }

            // Determine how much is still owed
            decimal amountRemaining = Amount - AmountPaid;

            if (Payments.Count == 0)
            {
                if (payment.Amount > Amount)
                    return "the payment is greater than the invoice amount";

                AmountPaid += payment.Amount;
                Payments.Add(payment);

                if (Type == InvoiceType.Commercial)
                    TaxAmount += payment.Amount * 0.14m;

                if (AmountPaid == Amount)
                    return "invoice was already fully paid";
                else
                    return "invoice is now partially paid";
            }
            else
            {
                if (payment.Amount > amountRemaining)
                    return "the payment is greater than the partial amount remaining";

                AmountPaid += payment.Amount;
                Payments.Add(payment);

                if (Type == InvoiceType.Commercial)
                    TaxAmount += payment.Amount * 0.14m;

                if (AmountPaid == Amount)
                    return "final partial payment received, invoice is now fully paid";

                return "another partial payment received, still not fully paid";
            }
        }

        public decimal Amount { get; set; }
		public decimal AmountPaid { get; set; }
		public decimal TaxAmount { get; set; }
		public List<Payment> Payments { get; set; }

		public InvoiceType Type { get; set; }
	}

	public enum InvoiceType
	{
		Standard,
		Commercial
	}
}