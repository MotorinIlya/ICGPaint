using Avalonia.Input;
using src.View.Drawing;

namespace src.Model.Tools;


public class FillTool(DrawingPanel panel, Drawer drawer) : ITool
{
    private Drawer _drawer = drawer;
    private DrawingPanel _panel = panel;
    
    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var start = e.GetPosition(_panel);
        _drawer.FillArea(_panel.Bitmap, start);
    }

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e) {}

    public void OnPointerMoved(object? sender, PointerEventArgs e) {}
}
