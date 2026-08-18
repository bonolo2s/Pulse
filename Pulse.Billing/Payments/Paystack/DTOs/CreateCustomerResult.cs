using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public record CreateCustomerResult(
        string CustomerCode
    );
}