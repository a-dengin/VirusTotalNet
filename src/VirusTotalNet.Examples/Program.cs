using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirusTotalNet.Objects;
using VirusTotalNet.ResponseCodes;
using VirusTotalNet.Results;

namespace VirusTotalNet.Examples;

internal class Program
{
    private static async Task Main(string[] args)
    {
        VirusTotal virusTotal = new VirusTotal("YOUR API KEY HERE");

        //Use HTTPS instead of HTTP
        virusTotal.UseTLS = true;

        //Create the EICAR test virus. See http://www.eicar.org/86-0-Intended-use.html
        byte[] eicar = Encoding.ASCII.GetBytes(@"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*");

        //Check if the file has been scanned before.
        FileReport fileReport = await virusTotal.GetFileReportAsync(eicar);

        bool hasFileBeenScannedBefore = fileReport.ResponseCode == FileReportResponseCode.Present;

        Console.WriteLine("File has been scanned before: " + (hasFileBeenScannedBefore ? "Yes" : "No"));

        //If the file has been scanned before, the results are embedded inside the report.
        if (hasFileBeenScannedBefore)
        {
            PrintScan(fileReport);
        }
        else
        {
            ScanResult fileResult = await virusTotal.ScanFileAsync(eicar, "EICAR.txt");
            PrintScan(fileResult);
        }

        Console.WriteLine();

        string scanUrl = "http://www.google.com/";

        UrlReport urlReport = await virusTotal.GetUrlReportAsync(scanUrl);

        bool hasUrlBeenScannedBefore = urlReport.ResponseCode == UrlReportResponseCode.Present;
        Console.WriteLine("URL has been scanned before: " + (hasUrlBeenScannedBefore ? "Yes" : "No"));

        //If the url has been scanned before, the results are embedded inside the report.
        if (hasUrlBeenScannedBefore)
        {
            PrintScan(urlReport);
        }
        else
        {
            UrlScanResult urlResult = await virusTotal.ScanUrlAsync(scanUrl);
            PrintScan(urlResult);
        }
    }

    private static void PrintScan(UrlScanResult scanResult)
    {
        Console.WriteLine("Scan ID: " + scanResult.ScanId);
        Console.WriteLine("Message: " + scanResult.VerboseMsg);
        Console.WriteLine();
    }

    private static void PrintScan(ScanResult scanResult)
    {
        Console.WriteLine("Scan ID: " + scanResult.ScanId);
        Console.WriteLine("Message: " + scanResult.VerboseMsg);
        Console.WriteLine();
    }

    private static void PrintScan(FileReport fileReport)
    {
        Console.WriteLine("Scan ID: " + fileReport.ScanId);
        Console.WriteLine("Message: " + fileReport.VerboseMsg);

        if (fileReport.ResponseCode == FileReportResponseCode.Present)
        {
            foreach (KeyValuePair<string, ScanEngine> scan in fileReport.Scans)
            {
                Console.WriteLine("{0,-25} Detected: {1}", scan.Key, scan.Value.Detected);
            }
        }

        Console.WriteLine();
    }

    private static void PrintScan(UrlReport urlReport)
    {
        Console.WriteLine("Scan ID: " + urlReport.ScanId);
        Console.WriteLine("Message: " + urlReport.VerboseMsg);

        if (urlReport.ResponseCode == UrlReportResponseCode.Present)
        {
            foreach (KeyValuePair<string, UrlScanEngine> scan in urlReport.Scans)
            {
                Console.WriteLine("{0,-25} Detected: {1}", scan.Key, scan.Value.Detected);
            }
        }

        Console.WriteLine();
    }
}