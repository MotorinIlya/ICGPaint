using Avalonia.Controls;
using Avalonia.Interactivity;
using src.Controller;
using src.Service.Events;

namespace src.View.PolygonCreate;

// при закрытии окна ставить дефолтный штамп
public partial class PolygonCreateWindow : Window
{
    private MainController _controller;
    public PolygonCreateWindow(MainController controller)
    {
        InitializeComponent();
        Count.TextChanged += TextChanged;
        Measure.TextChanged += TextChanged;
        Radius.TextChanged += TextChanged;
        _controller = controller;
    }

    private void SaveButtonClick(object sender, RoutedEventArgs e)
    {
        var resultCount = int.TryParse(Count.Text, out var countAngle);
        var resultMeasure = int.TryParse(Measure.Text, out var measureAngle);
        var resultRadius = int.TryParse(Radius.Text, out var radius);
        if (resultCount && resultMeasure && resultRadius)
        {
            _controller.Update(new PolygonEvent(countAngle, measureAngle % 360, radius));
            Close();
        }
        else
        {
            ErrorTextBlock.Text = "Invalid input. Please enter a valid number.";
        }
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        ErrorTextBlock.Text = string.Empty;
    }
}