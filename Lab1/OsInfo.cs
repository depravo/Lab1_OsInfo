using System;
using System.Collections.Generic;
using WUApiLib;

namespace Lab1;

public struct OsUpdate
{
    public string Title { get; set; }
	public bool IsInstalled { get; set; }
	public bool IsDownloaded { get; set; }

	public OsUpdate(string title, bool isInstalled, bool isDownloaded)
	{
		Title = title;
		IsInstalled = isInstalled;
		IsDownloaded = isDownloaded;
	}

}

public class OsInfo
{
    public Version OsVersion { get; set; }
    public PlatformID OsPlatform { get; set; }
    public string ServicePack { get; set; }
    public string OsVersionString { get; set; }
    public int MajorVersion { get; set; }
    public short MajorRevision { get; set; }
    public int MinorVersion { get; set; }
    public short MinorRevision { get; set; }
    public int Build { get; set; }
    public List<OsUpdate> Updates { get; set; }
    
    public OsInfo()
    {
        Updates = GetUpdates();
        OperatingSystem os = Environment.OSVersion;

        OsVersion = os.Version;
        OsPlatform = os.Platform;
        ServicePack = os.ServicePack;
        OsVersionString = os.VersionString;
        
        Version ver = os.Version;
        MajorVersion = ver.Major;
        MajorRevision = ver.MajorRevision;
        MinorVersion = ver.Minor;
        MinorRevision = ver.MinorRevision;
        Build = ver.Build;
    }

	public static List<OsUpdate> GetUpdates()
	{
		UpdateSession session = new UpdateSession();
		IUpdateSearcher searcher = session.CreateUpdateSearcher();
		searcher.Online = false;
		List<OsUpdate> result = new List<OsUpdate>();

		ISearchResult searchResult = searcher.Search("Type='Software'");
		foreach (IUpdate update in searchResult.Updates)
		{
			result.Add(new OsUpdate(update.Title, update.IsInstalled, update.IsDownloaded));
		}

		return result;
	}

	public override string ToString()
    {
        return
            $"OS Version: {OsVersion}\nOS Platform: {OsPlatform}\nOS Service Pack: {ServicePack}\nOS Version String: {OsVersionString}\n" +
            $"Major version: {MajorVersion}\nMajor Revision: {MajorRevision}\nMinor version: {MinorVersion}\nMinor Revision: {MinorRevision}\nBuild: {Build}";
    }
}