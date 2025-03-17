using Avalonia.Controls;
using Avalonia.Interactivity;
using src.Controller;
using src.Service.Events;

namespace src.View.PolygonCreate;

public partial class PolygonCreateWindow : Window
{
    private MainController _controller;

    [System.Obsolete]
    public PolygonCreateWindow(MainController controller)
    {
        InitializeComponent();
        _controller = controller;
        PolygonCheckBox.Checked += (s, e) => StarCheckBox.IsChecked = false;
        StarCheckBox.Checked += (s, e) => PolygonCheckBox.IsChecked = false;
        AngleTextBox.TextChanged += (s, e) => VerifyTextBox();
    }

    private void SaveButtonClick(object sender, RoutedEventArgs e)
    {
        // var resultCount = int.TryParse(AngleSlider.Value, out var countAngle);
        // var resultMeasure = int.TryParse(Measure.Text, out var measureAngle);
        // var resultRadius = int.TryParse(Radius.Text, out var radius);
        // if (resultCount && resultMeasure && resultRadius)
        // {
        //     _controller.Update(new PolygonEvent(countAngle, measureAngle % 360, radius));
        //     Close();
        // }
        // else
        // {
        //     ErrorTextBlock.Text = "Invalid input. Please enter a valid number.";
        // }
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