using Avalonia.Controls;
using src.Controller;
using src.View.Drawing;
using src.Service.Events;
using Avalonia.Interactivity;
using src.Model.Tools;
using src.View.PolygonCreate;
using Avalonia.Media;

namespace src.View;

public partial class MainWindow : Window
{
    private DrawingPanel _drawingPanel;
    private MainController _controller;
    private bool _isCreatePolygonWindow = false;
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

        Pencil.IsCheckedChanged += OnPencilToolClick;
        Line.IsCheckedChanged += OnLineToolClick;
        Fill.IsCheckedChanged += OnFillToolClick;
        Polygon.IsCheckedChanged += OnPolygonToolClick;
        BlackColor.Click += OnBlackClick;
        RedColor.Click += OnRedCLick;
        GreenColor.Click += OnGreenClick;
        BlueColor.Click += OnBlueClick;
        Clear.Click += OnClearClick;
        ThicknessSlider.ValueChanged += OnSliderChanged;
    }

    public void SetTool(ITool tool)
    {
        _drawingPanel.SetTool(tool);
    }

    private void OnPencilToolClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new PencilEvent());
        Line.IsChecked = false;
        Fill.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnLineToolClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new LineEvent());
        Pencil.IsChecked = false;
        Fill.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnFillToolClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new FillEvent());
        Pencil.IsChecked = false;
        Line.IsChecked = false;
        Polygon.IsChecked = false;
    }

    private void OnPolygonToolClick(object? sender, RoutedEventArgs e)
    {
        if (Polygon.IsChecked == true && !_isCreatePolygonWindow)
        {
            _isCreatePolygonWindow = true;
            var polygonWindow = new PolygonCreateWindow(_controller);
            polygonWindow.Closed += (s, args) => _isCreatePolygonWindow = false;

            polygonWindow.Show(this);
            Pencil.IsChecked = false;
            Line.IsChecked = false;
            Fill.IsChecked = false;
        }
    }

    private void OnBlackClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Black));
    }

    private void OnRedCLick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Red));
    }

    private void OnGreenClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Green));
    }

    private void OnBlueClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Blue));
    }

    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ClearEvent());
    }

    private void OnSliderChanged(object? sender, RoutedEventArgs e)
    {
        if ( sender is Slider slider)
        {
            var val = (int)slider.Value;
            _controller.Update(new SliderEvent(val));
        }
    }
}