using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 1)
        Console.Write("Podaj dane do zahashowania: ");
        string input = Console.ReadLine();
        byte[] data = Encoding.UTF8.GetBytes(input);
        var results = HashAlgorithms(data);
        foreach (var algorithm in results.Keys)
        {
            var (hashResult, hashTime) = results[algorithm];
            Console.WriteLine($"{algorithm}: {hashResult}, czas hashowania: {hashTime} s");
        }

        Console.WriteLine("=================");

        // 3)
        string filePath = "C:/Users/Asus/Desktop/Studia/ubuntu.iso";
        string fileHash = HashFile(filePath);
        Console.WriteLine($"Hash pliku {filePath}: {fileHash}");

        Console.WriteLine("=================");


        // 4)
        int[] dataSizes = { 1024, 2048, 4096, 8192 };
        var hashTimes = MeasureHashSpeed(dataSizes);

        Console.WriteLine("Czasy hashowania dla różnych rozmiarów danych:");
        for (int i = 0; i < dataSizes.Length; i++)
        {
            Console.WriteLine($"Rozmiar danych: {dataSizes[i]} bajtów, średni czas: {hashTimes[i]} s");
        }

        Console.ReadKey();
    }


    // Implementation of hashing with all functions from System.Security.Cryptography (substitute for hashlib)
    static Dictionary<string, (string hashResult, double hashTime)> HashAlgorithms(byte[] data)
    {
        var results = new Dictionary<string, (string, double)>();
        var algorithms = new List<string>(new string[] {
            "MD5", "SHA1", "SHA256", "SHA384", "SHA512"
        });

        foreach (var algorithm in algorithms)
        {
            using (var hashFunc = HashAlgorithm.Create(algorithm))
            {
                var stopwatch = Stopwatch.StartNew();
                byte[] hashBytes = hashFunc.ComputeHash(data);
                stopwatch.Stop();
                double hashTime = stopwatch.Elapsed.TotalSeconds;
                string hashResult = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                results[algorithm] = (hashResult, hashTime);
            }
        }

        return results;
    }


    //  Function to hash a file, tested on ubuntu.iso
    static string HashFile(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[8192]; // Needed to increese buffer memory space 
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
                }
                sha256.TransformFinalBlock(buffer, 0, 0);
            }
            return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
        }
    }

    // Testing speed of hash generation for various data sizes
    static List<double> MeasureHashSpeed(int[] dataSizes)
    {
        var hashTimes = new List<double>();
        var message = new byte[1024];
        var rand = new Random();

        foreach (var size in dataSizes)
        {
            rand.NextBytes(message);
            var stopwatch = Stopwatch.StartNew();
            using (var sha256 = SHA256.Create())
            {
                for (int i = 0; i < 1000; i++)
                {
                    sha256.ComputeHash(message, 0, Math.Min(size, message.Length));
                }
            }
            stopwatch.Stop();
            double hashTime = stopwatch.Elapsed.TotalSeconds / 1000;
            hashTimes.Add(hashTime);
        }
        return hashTimes;
    }
}
