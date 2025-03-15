using Avalonia.Controls;
using Avalonia.Media;
using src.Model;
using src.Model.Tools;
using src.Service;
using src.Service.Events;
using src.View;

namespace src.Controller;


public class MainController
{
    private MainWindow _window;
    private Drawer _drawer;
    private LineTool _lineTool;
    private PencilTool _pencilTool;
    private FillTool _fillTool;
    private PolygonTool _polygonTool;

    public MainController(MainWindow window)
    {
        _window = window;
        _drawer = new Drawer(window.Panel);
        _lineTool = new(_window.Panel, _drawer);
        _pencilTool = new(_window.Panel, _drawer);
        _fillTool = new(_window.Panel, _drawer);
        _polygonTool = new(_window.Panel, _drawer, 3, 0, 10);
    }

    public void Update(IEvent gameEvent)
    {
        if (gameEvent is PencilEvent)
        {
            _window.SetTool(_pencilTool);
        }
        else if (gameEvent is LineEvent)
        {
            _window.SetTool(_lineTool);
        }
        else if (gameEvent is FillEvent)
        {
            _window.SetTool(_fillTool);
        }
        else if (gameEvent is PolygonEvent e)
        {
            _polygonTool.SetParameters(e.CountAngle, e.AngleMeasure, e.Radius);
            _window.SetTool(_polygonTool);
        }
        else if (gameEvent is ChangeColorEvent change)
        {
            _drawer.SetColor(change.Col);
        }
        else if (gameEvent is ClearEvent)
        {
            _drawer.Clear(_window.Panel.Bitmap);
        }
    }
}