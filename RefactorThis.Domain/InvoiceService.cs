using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoiceService(InvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			// fetch the invoice
			var inv = _invoiceRepository.GetInvoice(payment.Reference);
			if (inv == null)
			{
				throw new InvalidOperationException("There is no invoice matching this payment");
			}

			// let the invoice handle how the payment is alllied
			var responseMessage = inv.AddPayment(payment);

			// changes to the DB
			inv.Save();

			return responseMessage;
		}
	}
}