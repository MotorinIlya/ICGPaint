using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using src.View.Drawing;

namespace src.Model.Tools;


public class PencilTool(DrawingPanel panel, Drawer drawer) : ITool
{
    private bool _isDrawing = false;
    private Point _previousPosition;
    private Drawer _drawer = drawer;
    private DrawingPanel _panel = panel;

    public void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        _isDrawing = true;
        _previousPosition = e.GetPosition(_panel);
        //_drawer.DrawPixel(_panel.Bitmap, e.GetPosition(_panel));
    }

    public void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isDrawing = false;
    }

    public void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (_isDrawing)
        {
            var currentPosition = e.GetPosition(_panel);
            _drawer.DrawLine(_panel.Bitmap, _previousPosition, currentPosition);
            _previousPosition = currentPosition;
        }
    }
}