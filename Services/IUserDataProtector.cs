namespace CookiesAuth.Services
{
    public interface IUserDataProtector
    {
        string Protect(string plainText);
        string Unprotect(string protectedData);
    }
}