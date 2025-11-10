using System.Resources;
using Microsoft.Data.SqlClient;

if (args.Length == 0)
{
    Console.WriteLine("Please enter the license ID:");
    var licenseId = Console.ReadLine();
    if (string.IsNullOrEmpty(licenseId))
    {
        Console.WriteLine("License ID cannot be empty.");
        return 1;
    }
    args = new[] { licenseId };
}

var licenseId = args[0];
var connectionString = Environment.GetEnvironmentVariable("TimeLogDb") ??
   "Server=DESKTOP-T6PL4DU\\SQLEXPRESS01;Database=TimeLog_db;Trusted_Connection=True;TrustServerCertificate=True";

try
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();

    // Query to fetch license data and public key
    using var command = new SqlCommand(
        "SELECT LicenseData, PublicKey FROM Licenses WHERE LicenseId = @LicenseId", 
        connection);
    command.Parameters.AddWithValue("@LicenseId", licenseId);

    using var reader = await command.ExecuteReaderAsync();
    if (!reader.Read())
    {
        Console.WriteLine($"No license found with ID: {licenseId}");
        return 1;
    }

    var licenseData = reader.GetString(0);
    var publicKey = reader.GetString(1);

    // Get the path to the resource files
    var projectDir = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();
    var resourcesDir = Path.Combine(projectDir, "MyApplication", "Resources");
    Directory.CreateDirectory(resourcesDir);

    // Update PublicKeys.resx
    using (var writer = new ResXResourceWriter(Path.Combine(resourcesDir, "PublicKeys.resx")))
    {
        writer.AddResource("LicensePublicKey", publicKey);
    }

    // Update LicenseJson.txt
    File.WriteAllText(Path.Combine(resourcesDir, "LicenseJson.txt"), licenseData);

    Console.WriteLine("License data and public key have been successfully updated in resources.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error occurred: {ex.Message}");
    return 1;
}