using Avalonia.Controls;

namespace GitSwap.Views;

public partial class GitConfigDialog : Window
{
    public GitConfigDialog()
    {
        InitializeComponent();

        var closeButton = this.FindControl<Button>("CloseButton");
        if (closeButton is not null)
            closeButton.Click += (_, _) => Close();
    }

    public void SetConfigText(string text)
    {
        var configText = this.FindControl<TextBlock>("ConfigText");
        if (configText is not null)
            configText.Text = text;
    }
}
