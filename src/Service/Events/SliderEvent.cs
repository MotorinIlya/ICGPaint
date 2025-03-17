namespace src.Service.Events;

public class SliderEvent(int val) : IEvent
{
    private int _thinkness = val;
    public int Thinkness => _thinkness;
}