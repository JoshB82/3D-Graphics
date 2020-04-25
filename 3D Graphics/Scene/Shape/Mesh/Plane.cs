using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    /// <summary>
    /// Handles creation of a plane mesh.
    /// </summary>
    public sealed class Plane : Mesh
    {
        public double length, width;
        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                Scaling = new Vector3D(length, 1, width);
            }
        }
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                Scaling = new Vector3D(length, 1, width);
            }
        }

        public Plane(Vector3D origin, Vector3D direction, Vector3D normal, double length, double width,
            Color? vertex_colour = null,
            Color? edge_colour = null,
            Color? face_colour = null)
        {
            Length = length;
            Width = width;

            Vertex_Colour = vertex_colour ?? Color.Blue;
            Edge_Colour = edge_colour ?? Color.Black;
            Face_Colour = face_colour ?? Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); // Green

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, normal);

            Model_Vertices = new Vertex[4]
            {
                new Vertex(0, 0, 0, Vertex_Colour), // 0
                new Vertex(1, 0, 0, Vertex_Colour), // 1
                new Vertex(1, 0, 1, Vertex_Colour), // 2
                new Vertex(0, 0, 1, Vertex_Colour) // 3
            };

            Edges = new Edge[5]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1], Edge_Colour), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2], Edge_Colour), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2], Edge_Colour) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3], Edge_Colour), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3], Edge_Colour) //4
            };

            Faces = new Face[2]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Face_Colour), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Face_Colour) // 1
            };

            Debug.WriteLine($"Plane created at {origin}");
        }

        public Plane(Vector3D origin, Vector3D direction, Vector3D normal, double length, double width, Bitmap texture,
            Color? vertex_colour = null,
            Color? edge_colour = null)
        {
            Length = length;
            Width = width;

            Vertex_Colour = vertex_colour ?? Color.Blue;
            Edge_Colour = edge_colour ?? Color.Black;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, normal);

            Model_Vertices = new Vertex[4]
            {
                new Vertex(0, 0, 0, Vertex_Colour), // 0
                new Vertex(1, 0, 0, Vertex_Colour), // 1
                new Vertex(1, 0, 1, Vertex_Colour), // 2
                new Vertex(0, 0, 1, Vertex_Colour) // 3
            };

            Texture_Vertices = new Texture_Vertex[4]
            {
                // WHY Z=1?
                new Texture_Vertex(0, 0, 1), // 0
                new Texture_Vertex(1, 0, 1), // 1
                new Texture_Vertex(0, 1, 1), // 2
                new Texture_Vertex(1, 1, 1) // 3
            };

            Edges = new Edge[5]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1], Edge_Colour), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2], Edge_Colour), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2], Edge_Colour) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3], Edge_Colour), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3], Edge_Colour) //4
            };

            Faces = new Face[2]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Texture_Vertices[3], Texture_Vertices[2], Texture_Vertices[0], texture), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture) // 1
            };

            Textures = new Bitmap[1]
            {
                texture // 0
            };

            Debug.WriteLine($"Plane created at {origin}");
        }
    }
}