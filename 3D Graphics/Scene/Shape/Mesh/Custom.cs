using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    /// <summary>
    /// Handles creation of a custom mesh.
    /// </summary>
    public sealed class Custom : Mesh
    {
        public Custom(Vector3D origin, Vector3D direction, Vector3D direction_up,
            Vector4D[] vertices,
            Spot[] spots,
            Edge[] edges,
            Face[] faces)
        {
            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = vertices;
            Spots = spots;
            Edges = edges;
            Faces = faces;

            Debug.WriteLine($"Custom mesh created at {origin}");
        }

        public Custom(Vector3D origin, Vector3D direction, Vector3D direction_up,
            Vector4D[] vertices,
            Vector3D[] texture_vertices,
            Spot[] spots,
            Edge[] edges,
            Face[] faces,
            Bitmap[] textures)
        {
            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = vertices;
            Spots = spots;
            Edges = edges;
            Faces = faces;
            Textures = textures;
            Texture_Vertices = texture_vertices;

            Debug.WriteLine($"Custom mesh created at {origin}");
        }
    }
}