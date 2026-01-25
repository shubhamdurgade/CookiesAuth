using CookiesAuth.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CookiesAuth.Services
{
    public class UserDataProtector : IUserDataProtector
    {
        private readonly IDataProtectionProvider _provider;
        private readonly IOptionsMonitor<CookiesAuth.Configuration.DataProtectionOptions> _options;
        private IDataProtector _protector;
        private string? _currentPurpose;

        public UserDataProtector(IDataProtectionProvider provider, IOptionsMonitor<CookiesAuth.Configuration.DataProtectionOptions> options)
        {
            _provider = provider;
            _options = options;

            // create initial protector from current options
            _currentPurpose = _options.CurrentValue.ProtectorPurpose ?? "CookiesAuth.Protector";
            _protector = _provider.CreateProtector(_currentPurpose);

            // update protector if configuration changes at runtime
            _options.OnChange(OnOptionsChanged);
        }

        private void OnOptionsChanged(CookiesAuth.Configuration.DataProtectionOptions newOptions)
        {
            var newPurpose = newOptions.ProtectorPurpose ?? "CookiesAuth.Protector";
            if (newPurpose != _currentPurpose)
            {
                _currentPurpose = newPurpose;
                _protector = _provider.CreateProtector(_currentPurpose);
            }
        }

        public string Protect(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string Unprotect(string protectedData)
        {
            // bubble up CryptographicException to caller so controller can handle invalid/tampered payloads
            return _protector.Unprotect(protectedData);
        }
    }
}