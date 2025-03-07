using Avalonia.Controls;

using src.View.Drawing;
namespace src.View;

public partial class MainWindow : Window
{
    private DrawingPanel _drawingPanel;
    public MainWindow()
    {
        InitializeComponent();
        Title = "ICGPaint";

        Width = 1920;
        Height = 1080;
        WindowState = WindowState.Maximized;

        _drawingPanel = new();
        Grid.SetRow(_drawingPanel, 1);
        MainGrid.Children.Add(_drawingPanel);
    }
}