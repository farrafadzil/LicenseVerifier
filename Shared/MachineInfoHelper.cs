using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public class MachineInfoHelper
    {
        // untuk dapatkan machine id unik dari hardware
        public static string GetMachineId()
        {
            try
            {
                // Combine few hardware IDs to make a fingerprint
                string cpuId = GetHardwareId("Win32_Processor", "ProcessorId");
                string hardDiskId = GetHardwareId("Win32_DiskDrive", "SerialNumber");
                string motherboardId = GetHardwareId("Win32_BaseBoard", "SerialNumber");

                string rawId = $"{cpuId}-{hardDiskId}-{motherboardId}";

                // Hash guna SHA256 supaya machine ID tu tak boleh diteka
                string hashed = ComputeSha256(rawId);
                return hashed;
            }
            catch
            {
                return "UNKNOWN";
            }
        }

        // Fungsi ni guna WMI untuk ambil property dari class tertentu
        private static string GetHardwareId(string wmiClass, string wmiProperty)
        {
            try
            {
                using (var mc = new ManagementClass(wmiClass))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        var value = mo[wmiProperty]?.ToString();
                        if (!string.IsNullOrEmpty(value))
                            return value;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        // Hash string (hardware ID gabungan tadi) guna SHA256
        private static string ComputeSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
            }
        }
    }
}
