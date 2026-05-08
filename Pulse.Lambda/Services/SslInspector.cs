using System.Net.Security;
using System.Net.Sockets;

namespace Pulse.Lambda.Services;

public class SslInspector
{
    private const int ExpiryThresholdDays = 7;

    public async Task<(bool IsExpiringSoon, DateTime ExpiryDate, string Issuer)> InspectAsync(string url)
    {
        var uri = new Uri(url);

        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(uri.Host, 443);

        using var sslStream = new SslStream(tcpClient.GetStream());
        await sslStream.AuthenticateAsClientAsync(uri.Host);

        var certificate = sslStream.RemoteCertificate!;
        var expiryDate = DateTime.Parse(certificate.GetExpirationDateString());
        var issuer = certificate.Issuer;
        var isExpiringSoon = expiryDate <= DateTime.UtcNow.AddDays(ExpiryThresholdDays);

        return (isExpiringSoon, expiryDate, issuer);
    }
}