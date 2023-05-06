using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lab1
{
    public partial class MainWindow : Window
    {
        private SystemInfo systemInfo;
        private OsInfo osInfo;
        private FirewallInfo firewallInfo;
        public MainWindow()
        {
            InitializeComponent();
            systemInfo = new SystemInfo();
            osInfo = new OsInfo();
            GetFirewallData();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += UpdateSystemData;
            timer.Start();
            CpuInfo.Text = systemInfo.GetCpuInfo();
            DriveData.ItemsSource = systemInfo.GetDrivesInfo();
            AntivirusDataGrid.ItemsSource = systemInfo.GetSystemAntivirus();
            OsUpdatesDataGrid.ItemsSource = osInfo.Updates;
            OsVersionDesc.Text = osInfo.ToString();
        }

        private void GetFirewallData()
        {
            firewallInfo = new FirewallInfo();
            FirewallDesc.Text = $"Profile type: {firewallInfo.ProfileType}\nActive: {firewallInfo.IsEnabled}";
            FirewallPortsDataGrid.ItemsSource = firewallInfo.GetPorts();
            FirewallAppsDataGrid.ItemsSource = firewallInfo.GetApps();
        }

        private void UpdateSystemData(object? sender, EventArgs e)
        {
            var cpuData = systemInfo.GetCpuUsage();
            CpuUsagePercentage.Text = cpuData.ToString("0.00") + "%";
            CpuUsage.Value = cpuData;
            RamUsage.Value = (1 - systemInfo.GetRamUsage() / systemInfo.GetTotalRam()) * 100;
            RamUsagePercentage.Text = RamUsage.Value.ToString("0.00") + "%";
            RamInfo.Text = $"Total: {systemInfo.GetTotalRam()} MB\nAvailable: {systemInfo.GetRamUsage()} MB";
        }

        private void RefreshDrivesButtonOnClick(object sender, RoutedEventArgs e)
        {
            DriveData.ItemsSource = systemInfo.GetDrivesInfo();
        }

        private void RefreshAntivirusButtonOnClick(object sender, RoutedEventArgs e)
        {
            AntivirusDataGrid.ItemsSource = systemInfo.GetSystemAntivirus();
        }

        private void RefreshUpdatesButtonOnClick(object sender, RoutedEventArgs e)
        {
            osInfo = new OsInfo();
            OsUpdatesDataGrid.ItemsSource = osInfo.Updates;
            OsVersionDesc.Text = osInfo.ToString();
        }

        private void RefreshFirewallButtonOnClick(object sender, RoutedEventArgs e)
        {
            GetFirewallData();
        }
    }
}