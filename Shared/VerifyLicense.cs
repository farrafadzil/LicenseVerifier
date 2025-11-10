using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Shared
{
    public static class VerifyLicense
    {
        // Main method: checks if the license is valid and authentic
        public static bool VerifyLicenses(string licenseDataBase64, string signatureBase64, string publicKeyBase64)
        {
            try
            {
                Console.WriteLine("=== VERIFY LICENSE ===");

                // Step 1: Decode the license data (usually Base64) into raw bytes
                // The verification process requires raw bytes of the data that was signed.
                byte[] dataBytes;
                try
                {
                    // Try to decode Base64 → byte array
                    dataBytes = Convert.FromBase64String(licenseDataBase64);
                }
                catch
                {
                    // If it fails (maybe it's already plain JSON text), 
                  
                    dataBytes = Encoding.UTF8.GetBytes(licenseDataBase64);
                }

                // Step 2: Decode the signature (Base64 → byte[])
                // The signature is also stored as Base64, so we must decode it first.
                byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

                // Step 3: Verify authenticity (check digital signature)
                // Uses the public key to verify that the data + signature are valid.
                bool isAuthentic = LicenseVerificationService.VerifySignature(dataBytes, signatureBytes, publicKeyBase64);

                if (!isAuthentic)
                {
                    // If the signature verification fails → the license may have been tampered with.
                    Console.WriteLine("Signature verification failed.");
                    return false;
                }

                Console.WriteLine("Signature verified successfully.");

                // Step 4: Convert the license bytes back into a JSON string
                // Because the license was originally a JSON object when signed.
                string decodedLicenseJson = Encoding.UTF8.GetString(dataBytes);

                // Step 5: Deserialize the JSON back into a License object
                // This lets us access fields like ProductName, Machine ID, etc.
                License? license;
                try
                {
                    license = JsonSerializer.Deserialize<License>(decodedLicenseJson);
                    if (license == null)
                    {
                        Console.WriteLine("Invalid license JSON format.");
                        return false;
                    }

                    // Print out basic license info for debugging
                    Console.WriteLine($"\nLicense deserialized. \nProduct Name: {license.ProductName} \nMachine ID: {license.TargetMachineIdentity}");
                }
                catch (Exception ex)
                {
                    // If the JSON is broken or invalid, deserialization will fail
                    Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    return false;
                }

                // Step 6: Compare Machine ID
                // Check that the license is valid for this specific machine only.
                string currentMachineId = MachineInfoHelper.GetMachineId();
                if (!string.Equals(license.TargetMachineIdentity, currentMachineId, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Machine ID mismatch.");
                    return false;
                }

                // If all checks pass — the license is valid, authentic, and for this machine
                Console.WriteLine("\nLicense is valid and authentic.");
                return true;
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors (invalid key, encoding problems, etc.)
                Console.WriteLine($"License verification error: {ex.Message}");
                return false;
            }
        }
    }
}
