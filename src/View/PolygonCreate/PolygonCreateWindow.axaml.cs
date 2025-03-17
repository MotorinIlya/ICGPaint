using Avalonia.Controls;
using Avalonia.Interactivity;
using src.Controller;
using src.Service.Events;

namespace src.View.PolygonCreate;

public partial class PolygonCreateWindow : Window
{
    private MainController _controller;

    public PolygonCreateWindow(MainController controller)
    {
        InitializeComponent();
        _controller = controller;
        PolygonCheckBox.IsCheckedChanged += (s, e) => StarCheckBox.IsChecked = false;
        StarCheckBox.IsCheckedChanged += (s, e) => PolygonCheckBox.IsChecked = false;
        AngleTextBox.TextChanged += (s, e) => VerifyTextBox();
        TurnTextBox.TextChanged += (s, e) => VerifyTextBox();
        RadiusTextBox.TextChanged += (s, e) => VerifyTextBox();
        SaveButton.Click += SaveButtonClick;
    }

    private void SaveButtonClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new PolygonEvent(
                (int)AngleSlider.Value, 
                (int)TurnSlider.Value, 
                (int)RadiusSlider.Value));
    }

    private void VerifyTextBox()
    {
        if (!int.TryParse(AngleTextBox.Text, out _))
        {
            var tmp = AngleTextBox.Text;
            tmp = tmp?[..^1];
            AngleTextBox.Text =  tmp;
        }
    }
}