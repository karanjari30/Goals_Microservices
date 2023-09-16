using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.ViewModel
{
    public class QBOPaymentInsertUpdateReqViewModel
    {
        public string AccountingCompanyId { get; set; }
        public List<PaymentReqViewModel> Payments { get; set; }
    }

    public class PaymentReqViewModel
    {
        public string CustomerId { get; set; }
        public string PaymentNo { get; set; }
        public string PaymentMethodId { get; set; }
        public string DepositToAccountId { get; set; }
        [AtLeastOneInvoice]
        public List<PaymentInvoiceViewModel>? invoices { get; set; }
    }

    public class PaymentInvoiceViewModel
    {
        [Required]
        public string? erpInvoiceId { get; set; }
        [Required]
        public decimal? amountPaid { get; set; }
    }

    public class AtLeastOneInvoiceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var invoices = value as List<PaymentInvoiceViewModel>;

            if (invoices == null || invoices.Count == 0)
            {
                return new ValidationResult("At least one invoice is required.");
            }

            return ValidationResult.Success;
        }
    }
}
