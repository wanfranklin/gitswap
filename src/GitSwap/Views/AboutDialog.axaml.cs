using Avalonia.Controls;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GitSwap.Views;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();

        var systemInfoText = this.FindControl<TextBlock>("SystemInfoText");
        if (systemInfoText is not null)
        {
            var os = RuntimeInformation.OSDescription;
            var arch = RuntimeInformation.OSArchitecture;
            var ram = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);

            systemInfoText.Text = $"{os} ({arch}) — {ram:F1} GB RAM";
        }

        var buildDateText = this.FindControl<TextBlock>("BuildDateText");
        if (buildDateText is not null)
        {
            var buildDate = GetBuildDate();
            buildDateText.Text = buildDate.ToString("ddMMyyyyHHmmss");
        }

        var closeButton = this.FindControl<Button>("CloseButton");
        if (closeButton is not null)
            closeButton.Click += (_, _) => Close();
    }

    private static DateTime GetBuildDate()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        if (!string.IsNullOrEmpty(location))
            return System.IO.File.GetLastWriteTime(location);

        return DateTime.MinValue;
    }
}
