using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Documents;
using WUApiLib;

namespace Lab1;

public struct AntivirusData
{
	public string Name { get; set; }
	public string InstanceGuid { get; set; }
	public string PathToSignedProductExe { get; set; }
	public string PathToSignedReportingExe { get; set; }
	public string Timestamp { get; set; }
	public bool UpToDate { get; set; }
	public bool Enabled { get; set; }

	public AntivirusData(string name, string instanceGuid, string pathToSignedProductExe, string pathToSignedReportingExe, string timestamp, bool upToDate, bool enabled)
	{
		Name = name;
		InstanceGuid = instanceGuid;
		PathToSignedProductExe = pathToSignedProductExe;
		PathToSignedReportingExe = pathToSignedReportingExe;
		Timestamp = timestamp;
		UpToDate = upToDate;
		Enabled = enabled;
	}
}

public struct DriveData {
    public string Name { get; set; }
    public string DriveType { get; set; }
	public string FileSystem { get; set; }
	public float AvailableSpaceGb { get; set; }
	public float TotalDriveSizeGb { get; set; }
	public string AvailableSizePercentage { get; set; }

	public DriveData(string name, string driveType, string fileSystem, float availableSpace, float totalDriveSize) {
		Name = name;
		DriveType = driveType;
		FileSystem = fileSystem;
		AvailableSpaceGb = availableSpace;
		TotalDriveSizeGb = totalDriveSize;
		AvailableSizePercentage = (AvailableSpaceGb / TotalDriveSizeGb).ToString("P");
	}
}

public class SystemInfo
{
    private PerformanceCounter _cpuCounter;
    private PerformanceCounter _ramCounter;
    private string _cpuInfo;
    private double _totalRam;

    public SystemInfo()
    {
        _cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total", true);
        _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        _cpuInfo = RetrieveCpuInfo();
        _totalRam = Double.Round(RetrieveTotalRam());
    }

    private string RetrieveCpuInfo()
    {
        var cpu =
            new ManagementObjectSearcher("select * from Win32_Processor")
                .Get()
                .Cast<ManagementObject>()
                .First();

        var name = (string)cpu["Name"];
        var cores = (uint)cpu["NumberOfCores"];
        var threads = (uint)cpu["NumberOfLogicalProcessors"];

        name = name
            .Replace("(TM)", "™")
            .Replace("(tm)", "™")
            .Replace("(R)", "®")
            .Replace("(r)", "®")
            .Replace("(C)", "©")
            .Replace("(c)", "©")
            .Replace("    ", " ")
            .Replace("  ", " ");
        return $"{name}\nCores: {cores} Threads: {threads}";
    }

    public float GetCpuUsage()
    {
        return _cpuCounter.NextValue();
    }

    public string GetCpuInfo()
    {
        return _cpuInfo;
    }

    public float GetRamUsage()
    {
        return _ramCounter.NextValue();
    }

    public double GetTotalRam()
    {
        return _totalRam;
    }

    private double RetrieveTotalRam()
    {
        var ram =
            new ManagementObjectSearcher("Select * From Win32_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .First();

        return Convert.ToDouble(ram["TotalPhysicalMemory"]) / 1048576;
    }

    public List<DriveData> GetDrivesInfo()
    {
        const long bytesInGigabytes = 1073741824;
        List<DriveData> result = new List<DriveData>();

        DriveInfo[] allDrives = DriveInfo.GetDrives();

        foreach (DriveInfo d in allDrives)
        {
            DriveData drive = new DriveData();
            if (d.IsReady)
            {
                drive = new DriveData(d.Name, d.DriveType.ToString(), d.DriveFormat,
                    (float)d.TotalFreeSpace / bytesInGigabytes, (float)d.TotalSize / bytesInGigabytes);
            }
            else
            {
                drive.Name = d.Name;
                drive.DriveType = d.DriveType.ToString();
            }

            result.Add(drive);
        }

        return result;
    }

    public List<AntivirusData> GetSystemAntivirus()
    {
        var antivirusData =
            new ManagementObjectSearcher(@"root\SecurityCenter2", "SELECT * FROM AntiVirusProduct")
                .Get()
                .Cast<ManagementObject>();

        List<AntivirusData> result = new List<AntivirusData>();
        foreach (ManagementObject virusChecker in antivirusData)
        {
            var name = (string)virusChecker["displayName"];
            var instanceGuid = (string)virusChecker["instanceGuid"];
            var pathToSignedProductExe = (string)virusChecker["pathToSignedProductExe"];
            var pathToSignedReportingExe = (string)virusChecker["pathToSignedReportingExe"];
            var timestamp = (string)virusChecker["timestamp"];
            var productState = (uint)virusChecker["productState"];
            bool upToDate;
            bool enabled;
            switch (productState)
            {
                case 262144:
                {
                    upToDate = true;
                    enabled = false;
                    break;
                }
                case 266240:
                {
                    upToDate = true;
                    enabled = true;
                    break;
                }
                case 262160:
                {
                    upToDate = false;
                    enabled = false;
                    break;
                }
                case 266256:
                {
                    upToDate = false;
                    enabled = true;
                    break;
                }
                case 393216:
                {
                    upToDate = true;
                    enabled = false;
                    break;
                }
                case 393232:
                {
                    upToDate = false;
                    enabled = false;
                    break;
                }
                case 393488:
                {
                    upToDate = false;
                    enabled = false;
                    break;
                }
                case 397312:
                {
                    upToDate = true;
                    enabled = true;
                    break;
                }
                case 397328:
                {
                    upToDate = false;
                    enabled = true;
                    break;
                }
                case 393472:
                {
                    upToDate = true;
                    enabled = false;
                    break;
                }
                case 397584:
                {
                    upToDate = false;
                    enabled = true;
                    break;
                }
                case 397568:
                {
                    upToDate = true;
                    enabled = true;
                    break;
                }
                default:
                {
                    upToDate = false;
                    enabled = false;
                    break;
                }
            }

            result.Add(new AntivirusData(name, instanceGuid, pathToSignedProductExe, pathToSignedReportingExe,
                timestamp, upToDate, enabled));
        }

        return result;
    }
}