using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;

namespace HangfireDemo;

public class BasicAuthDashboard : IDashboardAuthorizationFilter
{
    private readonly string _user;
    private readonly string _password;

    public BasicAuthDashboard(string user, string password)
    {
        _user = user;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        var header = http.Request.Headers["Authorization"].ToString();

        if (header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var decoded = Encoding.UTF8.GetString(
                    Convert.FromBase64String(header["Basic ".Length..].Trim()));
                var index = decoded.IndexOf(':');

                if (index > 0)
                {
                    var user = decoded[..index];
                    var pass = decoded[(index + 1)..];

                    if (SafeEquals(user, _user) & SafeEquals(pass, _password))
                        return true;
                }
            }
            catch (FormatException) { }
        }

        http.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        return false;
    }

    private static bool SafeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
