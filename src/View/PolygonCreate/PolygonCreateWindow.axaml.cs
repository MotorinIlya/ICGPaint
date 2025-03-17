using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using src.Controller;
using src.Service.Events;

namespace src.View.PolygonCreate;

public partial class PolygonCreateWindow : Window
{
    private MainController _controller;
    public event EventHandler<bool>? Data;

    public PolygonCreateWindow(MainController controller)
    {
        InitializeComponent();
        _controller = controller;
        AngleTextBox.TextChanged += (s, e) => VerifyTextBox(AngleTextBox, AngleSlider);
        TurnTextBox.TextChanged += (s, e) => VerifyTextBox(TurnTextBox, TurnSlider);
        RadiusTextBox.TextChanged += (s, e) => VerifyTextBox(RadiusTextBox, RadiusSlider);
        SaveButton.Click += SaveButtonClick;
        CancelButton.Click += CancelButtonClick;
        PolygonCheckBox.IsCheckedChanged += OnCheckBoxChanged;
        StarCheckBox.IsCheckedChanged += OnCheckBoxChanged;
    }

    private void OnCheckBoxChanged(object? sender, RoutedEventArgs e)
    {
        var currentCheckBox = sender as CheckBox;
        var otherCheckBox = currentCheckBox == PolygonCheckBox ? StarCheckBox : PolygonCheckBox;

        otherCheckBox.IsCheckedChanged -= OnCheckBoxChanged;
        otherCheckBox.IsChecked = false;
        otherCheckBox.IsCheckedChanged += OnCheckBoxChanged;
    }

    private void SaveButtonClick(object? sender, RoutedEventArgs e)
    {
        if (PolygonCheckBox.IsChecked is bool check && check)
        {
            _controller.Update(new PolygonEvent(
                    (int)AngleSlider.Value, 
                    (int)TurnSlider.Value, 
                    (int)RadiusSlider.Value));
            Data?.Invoke(this, true);
            Close();
        }
        else if (StarCheckBox.IsChecked is bool starCheck && starCheck)
        {
            _controller.Update(new StarEvent(
                    (int)AngleSlider.Value, 
                    (int)TurnSlider.Value, 
                    (int)RadiusSlider.Value));
            Data?.Invoke(this, true);
            Close();
        }
    }

    private void CancelButtonClick(object? sender, RoutedEventArgs e)
    {
        Data?.Invoke(this, false);
        Close();
    }

    private void VerifyTextBox(TextBox box, Slider slider)
    {
        if (string.IsNullOrEmpty(box.Text))
        {
            slider.Value = slider.Minimum;
            return;
        }
        if (!int.TryParse(box.Text, out _))
        {
            var tmp = box.Text;
            tmp = tmp?[..^1];
            box.Text =  tmp;
        }
    }
}