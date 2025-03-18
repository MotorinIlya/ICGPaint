using Avalonia.Controls;
using src.Controller;
using src.View.Drawing;
using src.Service.Events;
using Avalonia.Interactivity;
using src.Model.Tools;
using src.View.PolygonCreate;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.IO;

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
        if (sender is ToggleButton button && button.IsChecked == true)
        {
            _clickedButton = button;
            _controller.Update(new PencilEvent());
            Fill.IsChecked = false;
            Line.IsChecked = false;
            Polygon.IsChecked = false;
        }
    }

    private void PencilMenuItemClick(object? sender, RoutedEventArgs e) =>
            Pencil.IsChecked = true;

    private void OnLineToolClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton button && button.IsChecked == true)
        {
            _clickedButton = button;
            _controller.Update(new LineEvent());
            Pencil.IsChecked = false;
            Fill.IsChecked = false;
            Polygon.IsChecked = false;
        }
    }

    private void OnFillToolClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton button && button.IsChecked == true)
        {
            _clickedButton = button;
            _controller.Update(new FillEvent());
            Pencil.IsChecked = false;
            Line.IsChecked = false;
            Polygon.IsChecked = false;
        }
    }

    private void OnPolygonToolClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton clickedButton)
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
                        clickedButton.IsChecked = false;
                        if (_clickedButton is not null && _clickedButton != clickedButton)
                        {
                            _clickedButton.IsChecked = true;
                            _drawingPanel.ClearTool();
                        }
                    }
                    else
                    {
                        if (_clickedButton is not null)
                        {
                            _clickedButton.IsChecked = false;
                        }  
                        _clickedButton = clickedButton;
                    }
                };
                polygonWindow.Show(this);
            }
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

    private void OnLoadImageClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new LoadImageEvent());
    }

    private void OnSaveImageClick(object? sender, RoutedEventArgs e)
    {
        _controller.Update(new SaveImageEvent());
    }
}