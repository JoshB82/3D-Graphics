using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    public sealed class Distant_Light : Light
    {
        public Distant_Light(Vector3D position, Vector3D direction, Color? colour, double intensity)
        {
            Translation = position;
            World_Origin = new Vector4D(position);
            World_Direction = direction;
            Colour = colour ?? Color.White;
            Intensity = intensity;

            Debug.WriteLine($"Distant light created at ({position.X}, {position.Y}, {position.Z})");
        }

        public Distant_Light(Vector3D position, Mesh pointed_at, Color? colour, double intensity) : this(position, new Vector3D(pointed_at.World_Origin) - position, colour, intensity) { }
    }
}
