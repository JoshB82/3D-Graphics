using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    /// <summary>
    /// Handles creation of a custom mesh.
    /// </summary>
    public sealed class Custom : Mesh
    {
        public Vector3D origin;
        public Vector3D Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                World_Origin = new Vertex(origin.X, origin.Y, origin.Z);
                Translation = new Vector3D(origin.X, origin.Y, origin.Z);
            }
        }

        public Custom(Vector3D origin, Vector3D direction, Vector3D direction_up,
            Vertex[] vertices,
            Edge[] edges,
            Face[] faces)
        {
            Origin = origin;
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = vertices;
            Edges = edges;
            Faces = faces;

            Debug.WriteLine($"Custom mesh created at ({origin.X}, {origin.Y}, {origin.Z})");
        }

        public Custom(Vector3D origin, Vector3D direction, Vector3D direction_up,
            Vertex[] vertices,
            Edge[] edges,
            Face[] faces,
            Bitmap[] textures,
            Texture_Vertex[] texture_vertices)
        {
            Origin = origin;
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = vertices;
            Edges = edges;
            Faces = faces;
            Textures = textures;
            Texture_Vertices = texture_vertices;

            Debug.WriteLine($"Custom mesh created at ({origin.X}, {origin.Y}, {origin.Z})");
        }
    }
}