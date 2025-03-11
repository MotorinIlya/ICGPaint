namespace src.Service.Events;

public class PolygonEvent(int countAngle, int angleMeasure, int radius) : IEvent
{
    private int _countAngle = countAngle;
    private int _angleMeasure = angleMeasure;
    private int _radius = radius;

    public int CountAngle => _countAngle;
    public int AngleMeasure => _angleMeasure;
    public int Radius => _radius;
}