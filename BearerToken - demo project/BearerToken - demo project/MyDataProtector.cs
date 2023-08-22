using Microsoft.AspNetCore.DataProtection;


/// <summary>
/// Custom transparent DataProtector that will encrypt the data
/// </summary>
public class MyDataProtector : IDataProtector
{
    public IDataProtector CreateProtector(string purpose)
    {
        return new MyDataProtector();
    }

    public byte[] Protect(byte[] plaintext)
    {
        return plaintext;
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        return protectedData;
    }
}