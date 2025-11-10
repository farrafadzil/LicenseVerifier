using System;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public static class LicenseVerificationService
    {
        public static bool VerifySignature(byte[] licenseDataBase64, byte[] signatureBase64, string publicKeyBase64)
        {
            try
            {
                // Decode public key Base64 → raw bytes
                byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);

                using RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(publicKeyBytes, out _); 

                // Verify signature using SHA256 + PKCS1 padding
                return rsa.VerifyData(licenseDataBase64, signatureBase64, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Signature verification exception: {ex.Message}");
                return false;
            }
        }
        
    }
}
