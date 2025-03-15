using Avalonia.Media;

namespace src.Service.Events;

public class ChangeColorEvent(Color color) : IEvent
{
    private Color _color = color;
    public Color Col => _color;
}