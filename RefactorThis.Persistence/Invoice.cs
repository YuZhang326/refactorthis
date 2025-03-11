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
					return "No payment needed";
				}
				throw new InvalidOperationException(
					"It has an amount of 0 and it has payments."
				);
			}

			// if already fully paid
			if (AmountPaid == Amount)
			{
				return "Invoice already fully paid";
			}

			// Determine how much is still owed
			decimal amountRemaining = Amount - AmountPaid;

			// if we already habe partial payments, check for overpayment
			if (Payments != null && Payments.Count > 0)
			{
				if (payment.Amount > amountRemaining)
				{
					return "The payment is greater than the amount remaining";
				}
			}
			else
			{
				if (payment.Amount > Amount)
				{
					return "The payment is greater than the total amount";
				}
			}

			// apply the payment
			AmountPaid += payment.Amount;
			Payments.Add(payment);

			// if commercual, also apply 14% tax
			if (Type == InvoiceType.Commercial)
			{
				TaxAmount += payment.Amount * 0.14m;
			}

			// figure out if it's fully paid now
			if (AmountPaid == Amount)
			{
				return Payments.Count == 1
					? "Invoice fully paid"
					: "FInal payment received";
			}
			else
			{
				// not fully paid yet
				return Payments.Count == 1
					? "Invoice is partially paid"
					: "Another partial payment received";
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