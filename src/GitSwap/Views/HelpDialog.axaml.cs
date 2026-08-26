using Avalonia.Controls;

namespace GitSwap.Views;

public partial class HelpDialog : Window
{
    private readonly Panel[] _panels;

    public HelpDialog()
    {
        InitializeComponent();

        _panels =
        [
            this.FindControl<StackPanel>("PanelVisaoGeral")!,
            this.FindControl<StackPanel>("PanelAdicionar")!,
            this.FindControl<StackPanel>("PanelEditar")!,
            this.FindControl<StackPanel>("PanelAlternar")!,
            this.FindControl<StackPanel>("PanelRemover")!,
            this.FindControl<StackPanel>("PanelContaAtiva")!,
            this.FindControl<StackPanel>("PanelDados")!,
        ];

        var closeButton = this.FindControl<Button>("CloseButton");
        if (closeButton is not null)
            closeButton.Click += (_, _) => Close();

        ShowPanel(0);
    }

    private void TopicsList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox list) return;
        if (list.SelectedIndex < 0) return;
        ShowPanel(list.SelectedIndex);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < _panels.Length; i++)
        {
            if (_panels[i] is not null)
                _panels[i].IsVisible = i == index;
        }
    }
}
