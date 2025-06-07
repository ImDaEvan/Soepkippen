using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class RsaKeyProvider
{
    public static RSA LoadPrivateKeyFromPem(string pemPath)
    {
        var privateKeyText = File.ReadAllText(pemPath);
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyText.ToCharArray());
        return rsa;
    }
}
