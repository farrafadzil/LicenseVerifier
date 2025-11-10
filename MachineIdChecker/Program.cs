using System;
using Shared; 

namespace MachineIdChecker
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Machine ID Checker";

            Console.WriteLine("========================================");
            Console.WriteLine("        MACHINE ID CHECKER");
            Console.WriteLine("========================================\n");

            // Get the machine ID from helper
            string machineId = MachineInfoHelper.GetMachineId();

            Console.WriteLine($"Machine ID: {machineId}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
