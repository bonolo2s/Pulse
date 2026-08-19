using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Pulse.Billing.Payments.Paystack
{
    public static class PaystackWebhookValidator
    {
        // Reserved for IP whitelisting
        private static readonly HashSet<string> AllowedIps = new()
        {
            //
        };

        public static bool IsIpWhitelisted(string? remoteIp)
        {
            if (string.IsNullOrEmpty(remoteIp))
                return false;

            return AllowedIps.Contains(remoteIp);
        }

        public static bool IsSignatureValid(string rawBody, string? signatureHeader, string secretKey)
        {
            if (string.IsNullOrEmpty(signatureHeader))
                return false;

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));
            var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();

            return computedSignature == signatureHeader.ToLowerInvariant();
        }
    }
}