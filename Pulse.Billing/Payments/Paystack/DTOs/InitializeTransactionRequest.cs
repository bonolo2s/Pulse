using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public record InitializeTransactionRequest(
        string Email,
        decimal Amount,
        string Currency,
        string CallbackUrl,
        string Plan
    );
}
