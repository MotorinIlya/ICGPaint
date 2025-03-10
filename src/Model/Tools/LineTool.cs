using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using src.View.Drawing;

namespace src.Model.Tools;


public class LineTool(DrawingPanel panel, Drawer drawer) : ITool
{
    private Point _previousPosition;
    private Drawer _drawer = drawer;
    private DrawingPanel _panel = panel;
    private bool _isDrawing = false;

    public void OnPointerMoved(object sender, PointerEventArgs e){}

    public void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!_isDrawing)
        {
            _isDrawing = true;
            _previousPosition = e.GetPosition(_panel);
        }
        else
        {
            _isDrawing = false;
            _drawer.DrawLine(_panel.Bitmap, _previousPosition, e.GetPosition(_panel));
        }
    }

    public void OnPointerReleased(object sender, PointerReleasedEventArgs e){}
}