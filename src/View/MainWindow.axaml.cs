using Avalonia.Controls;
using src.Controller;
using src.View.Drawing;
using src.Service.Events;
using Avalonia.Interactivity;
using src.Model.Tools;
using src.View.PolygonCreate;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using src.View.About;
using src.View.ColorPicker;

namespace src.View;

public partial class MainWindow : Window
{
    private DrawingPanel _drawingPanel;
    private MainController _controller;
    private bool _isCreatePolygonWindow = false;
    private ToggleButton? _clickedButton;
    public DrawingPanel Panel => _drawingPanel;

    public MainWindow()
    {
        InitializeComponent();
        Title = "ICGPaint";

        Width = 1280;
        Height = 720;
        WindowState = WindowState.Maximized;

        _drawingPanel = new();
        
        var scrollViewer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = _drawingPanel
        };
        Grid.SetRow(scrollViewer, 2);
        MainGrid.Children.Add(scrollViewer);
        
        _controller = new(this);

        Pencil.Click += OnPencilToolClick;
        Line.Click += OnLineToolClick;
        Fill.Click += OnFillToolClick;
        Polygon.Click += OnPolygonToolClick;
        BlackColor.Click += OnBlackClick;
        RedColor.Click += OnRedCLick;
        GreenColor.Click += OnGreenClick;
        BlueColor.Click += OnBlueClick;
        Clear.Click += OnClearClick;
        ThicknessSlider.ValueChanged += OnSliderChanged;
        ThinknessTextBox.TextChanged += (s, e) => VerifyTextBox(ThinknessTextBox, ThicknessSlider);
    }

    public void SetTool(ITool tool)
    {
        _drawingPanel.SetTool(tool);
    }

    private void SelectTool(ToggleButton selectedTool)
    {
        var tools = new[] { Pencil, Line, Fill, Polygon };
        
        foreach (var tool in tools)
        {
            if (tool != selectedTool && tool.IsChecked == true)
            {
                tool.IsChecked = false;
            }
        }
        
        selectedTool.IsChecked = true;

        if (selectedTool == Pencil) 
        {
            _clickedButton = Pencil;
            _controller.Update(new PencilEvent());
        }
        else if (selectedTool == Line) 
        {
            _clickedButton = Line;
            _controller.Update(new LineEvent());
        }
        else if (selectedTool == Fill) 
        {
            _clickedButton = Fill;
            _controller.Update(new FillEvent());
        }
        else if (selectedTool == Polygon) 
        {
            HandlePolygonTool();
        }
    }

    private void OnPencilToolClick(object? sender, RoutedEventArgs e) 
        => SelectTool(Pencil);

    private void OnLineToolClick(object? sender, RoutedEventArgs e) 
        => SelectTool(Line);

    private void OnFillToolClick(object? sender, RoutedEventArgs e) 
        => SelectTool(Fill);

    private void OnPolygonToolClick(object? sender, RoutedEventArgs e) 
        => SelectTool(Polygon);

    private void HandlePolygonTool()
    {
        if (Polygon.IsChecked == true && !_isCreatePolygonWindow)
        {
            _isCreatePolygonWindow = true;
            var polygonWindow = new PolygonCreateWindow(_controller);
            polygonWindow.Data += (s, result) => 
            {   
                _isCreatePolygonWindow = false;
                if (!result)
                {
                    Polygon.IsChecked = false;
                    if (_clickedButton != null && _clickedButton != Polygon)
                    {
                        _clickedButton.IsChecked = true;
                    }
                }
                else
                {
                    _clickedButton = Polygon;
                }
            };
            polygonWindow.Show(this);
        }
    }

    private void OnBlackClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Black));
        CurrentColorDisplay.Background = new SolidColorBrush(Colors.Black);
    }

    private void OnRedCLick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Red));
        CurrentColorDisplay.Background = new SolidColorBrush(Colors.Red);
    }

    private void OnGreenClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Green));
        CurrentColorDisplay.Background = new SolidColorBrush(Colors.Green);
    }

    private void OnBlueClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new ChangeColorEvent(Colors.Blue));
        CurrentColorDisplay.Background = new SolidColorBrush(Colors.Blue);
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

    private void OnLoadImageClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new LoadImageEvent());
    }

    private void OnSaveImageClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new SaveImageEvent());
    }

    private void OnAboutClick(object? sender, RoutedEventArgs e)
    {
        var about = new AboutWindow();
        about.ShowDialog(this);
    }

    private void OnColorPickerClick(object? sender, RoutedEventArgs e)
    {
        var colorWindow = new ColorPickerWindow();
        colorWindow.Data += (s, result) =>
        {
            _controller.Update(new ColorChangeEvent(result));
            CurrentColorDisplay.Background = new SolidColorBrush(result);
        };
        colorWindow.Show();
    }

    private void VerifyTextBox(TextBox box, Slider slider)
    {
        if (string.IsNullOrEmpty(box.Text))
        {
            slider.Value = slider.Minimum;
            return;
        }
        if (!int.TryParse(box.Text, out var v) || v > slider.Maximum)
        {
            var tmp = box.Text;
            tmp = tmp?[..^1];
            box.Text =  tmp;
        }
    }
}