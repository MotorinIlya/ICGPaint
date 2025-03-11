using Avalonia.Controls;
using Avalonia.Media;

using src.Controller;
using src.View.Drawing;
using src.Service.Events;
using Avalonia.Interactivity;
using src.Model.Tools;
using src.View.PolygonCreate;

namespace src.View;

public partial class MainWindow : Window
{
    private DrawingPanel _drawingPanel;
    private MainController _controller;
    public DrawingPanel Panel => _drawingPanel;

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
        
        _controller = new(this);

        Pencil.Checked += OnPencilToolClick;
        Line.Checked += OnLineToolClick;
        Fill.Checked += OnFillToolClick;
        Polygon.Checked += OnPolygonToolClick;
    }

    public void SetTool(ITool tool)
    {
        _drawingPanel.SetTool(tool);
    }

    private void OnPencilToolClick(object sender, RoutedEventArgs e)
    {
        _controller.Update(new PencilEvent());
        Line.IsChecked = false;
        Fill.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnLineToolClick(object sender, RoutedEventArgs e)
    {
        _controller.Update(new LineEvent());
        Pencil.IsChecked = false;
        Fill.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnFillToolClick(object sender, RoutedEventArgs e)
    {
        _controller.Update(new FillEvent());
        Pencil.IsChecked = false;
        Line.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnPolygonToolClick(object sender, RoutedEventArgs e)
    {
        var polygonWindow = new PolygonCreateWindow(_controller);
        polygonWindow.ShowDialog(this);
        Pencil.IsChecked = false;
        Line.IsChecked = false;
        Fill.IsChecked = false;
    }
}