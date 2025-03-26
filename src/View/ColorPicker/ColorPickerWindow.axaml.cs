using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using Avalonia.Controls.Primitives;

namespace src.View.ColorPicker;

public partial class ColorPickerWindow : Window
{
    public Color SelectedColor { get; private set; } = Colors.Black;
    public EventHandler<Color>? Data;

    public ColorPickerWindow()
    {
        InitializeComponent();
        UpdateColorPreview();
        
        redSlider.ValueChanged += OnSliderValueChanged;
        greenSlider.ValueChanged += OnSliderValueChanged;
        blueSlider.ValueChanged += OnSliderValueChanged;
        
        okButton.Click += OnOkClick;
        cancelButton.Click += OnCancelClick;
    }

    private void OnSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        UpdateColorPreview();
    }

    private void UpdateColorPreview()
    {
        var color = Color.FromRgb(
            (byte)redSlider.Value,
            (byte)greenSlider.Value,
            (byte)blueSlider.Value
        );
        
        colorPreview.Background = new SolidColorBrush(color);
        SelectedColor = color;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Data?.Invoke(this, Color.FromArgb(
                            255, 
                            (byte)redSlider.Value, 
                            (byte)greenSlider.Value, 
                            (byte)blueSlider.Value));
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}