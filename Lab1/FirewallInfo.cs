using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using WUApiLib;

namespace Lab1;

public struct FirewallApp
{
	public string Name { get; set; }
	public string IpVersion { get; set; }
	public FirewallApp(string name, string v)
	{
		Name = name;
		this.IpVersion = v;
	}
}

public struct FirewallPort
{
	public int Port { get; set; }
	public string Name { get; set; }
	public string IpVersion { get; set; }
	public FirewallPort(int port, string name, string v)
	{
		Port = port;
		Name = name;
		this.IpVersion = v;
	}
}

public class FirewallInfo
{
    public string ProfileType { get; set; }
    public bool IsEnabled { get; set; }
	public List<FirewallPort> Ports;
	public List<FirewallApp> Apps;

	public FirewallInfo()
    {
        Ports = new List<FirewallPort>();
        Apps = new List<FirewallApp>();
        Type netFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
        INetFwMgr manager = (INetFwMgr)Activator.CreateInstance(netFwMgrType);
        ProfileType = manager.CurrentProfileType.ToString();
        IsEnabled = manager.LocalPolicy.CurrentProfile.FirewallEnabled;

        foreach (INetFwOpenPort port in manager.LocalPolicy.CurrentProfile.GloballyOpenPorts)
        {
            Ports.Add(new FirewallPort(port.Port, port.Name, port.IpVersion.ToString()));
        }

        foreach (INetFwAuthorizedApplication app in manager.LocalPolicy.CurrentProfile.AuthorizedApplications)
        {
            Apps.Add(new FirewallApp(app.Name, app.IpVersion.ToString()));
        }
    }

	public List<FirewallPort> GetPorts() => Ports;
	public List<FirewallApp> GetApps() => Apps;
}