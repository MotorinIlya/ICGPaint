using Avalonia.Input;

namespace src.Model.Tools;


public interface ITool
{
    public void OnPointerPressed(object? sender, PointerPressedEventArgs e);

    public void OnPointerMoved(object? sender, PointerEventArgs e);

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e);
}