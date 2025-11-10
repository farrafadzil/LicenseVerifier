using Shared;
using System;
using System.Text.Json;
using System.Windows.Forms;
using MyApplication.Resources; // pastikan namespace ikut folder resources kau

namespace MyApplication
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // 1️. Load dari resource file
                string publicKey = PublicKeys.PublicKey;               // simpan public key string dalam .resx
                string licenseJson = LicenseDataSignature.LicenseJson; // simpan license JSON dalam .resx

                // 2️. Deserialize license JSON dari resource
                var licenseModel = JsonSerializer.Deserialize<License>(licenseJson);
                if (licenseModel == null)
                {
                    MessageBox.Show("License file corrupt or missing.",
                        "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string licenseData = licenseModel.LicenseData;
                string signature = licenseModel.Signature;

                // 3️. Verify license in background
                bool valid = VerifyLicense.VerifyLicenses(licenseData, signature, publicKey);

                // 4️. Kalau invalid → terus block
                if (!valid)
                {
                    MessageBox.Show("License invalid. \nPlease contact support.",
                        "License Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // stop app from launching
                }

                // 5️. License ok → terus run main form
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // 6️. Fallback – kalau ada error masa verify
                MessageBox.Show($"License check error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
