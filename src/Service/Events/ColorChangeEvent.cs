using Avalonia.Media;

namespace src.Service.Events;

public class ColorChangeEvent(Color color) : IEvent
{
    private Color _color = color;
    public Color ColorForSet => _color;
}