using Castaway.Math;

namespace Castaway.Rendering.Lighting;

public class PointLight : Light
{
    public Vector3 Color;
    public Vector3 Position;

    public PointLight(Vector3 position, Vector3 color)
    {
        Position = position;
        Color = color;
    }
}