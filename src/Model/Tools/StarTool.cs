using Avalonia.Input;
using src.View.Drawing;

namespace src.Model.Tools;

public class StarTool(DrawingPanel panel,
        Drawer drawer, 
        int countAngle, 
        int measureAngle,
        int radius) : ITool
{
    private DrawingPanel _panel = panel;
    private Drawer _drawer = drawer;
    private int _countAngle = countAngle;
    private int _measureAngle = measureAngle;
    private int _radius = radius;

    public void SetParameters(int countAngle, int measureAngle, int radius)
    {
        _countAngle = countAngle;
        _measureAngle = measureAngle;
        _radius = radius;
    }

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var center = e.GetPosition(_panel);
        _drawer.DrawStar(_panel.Bitmap, center, _countAngle, _measureAngle, _radius);
    }

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e) {}

    public void OnPointerMoved(object? sender, PointerEventArgs e) {}
}