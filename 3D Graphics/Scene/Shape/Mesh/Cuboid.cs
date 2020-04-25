using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    /// <summary>
    /// Handles creation of a cuboid mesh.
    /// </summary>
    public sealed class Cuboid : Mesh
    {
        public double length, width, height;
        public double Length
        {
            get { return length; }
            set
            {
                length = value;
                Scaling = new Vector3D(length, width, height);
            }
        }
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                Scaling = new Vector3D(length, width, height);
            }
        }
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                Scaling = new Vector3D(length, width, height);
            }
        }

        public Cuboid(Vector3D origin, Vector3D direction, Vector3D direction_up, double length, double width, double height,
            Color? vertex_colour = null,
            Color? edge_colour = null,
            Color? face_colour = null)
        {
            Length = length;
            Width = width;
            Height = height;

            Vertex_Colour = vertex_colour ?? Color.Blue;
            Edge_Colour = edge_colour ?? Color.Black;
            Face_Colour = face_colour ?? Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); // Green

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = new Vertex[8]
            {
                new Vertex(0, 0, 0, Vertex_Colour), // 0
                new Vertex(1, 0, 0, Vertex_Colour), // 1
                new Vertex(1, 1, 0, Vertex_Colour), // 2
                new Vertex(0, 1, 0, Vertex_Colour), // 3
                new Vertex(0, 0, 1, Vertex_Colour), // 4
                new Vertex(1, 0, 1, Vertex_Colour), // 5
                new Vertex(1, 1, 1, Vertex_Colour), // 6
                new Vertex(0, 1, 1, Vertex_Colour) // 7
            };

            Edges = new Edge[18]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1], Edge_Colour), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2], Edge_Colour), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2], Edge_Colour), // 2
                new Edge(Model_Vertices[2], Model_Vertices[3], Edge_Colour), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3], Edge_Colour), // 4
                new Edge(Model_Vertices[1], Model_Vertices[5], Edge_Colour), // 5
                new Edge(Model_Vertices[5], Model_Vertices[6], Edge_Colour), // 6
                new Edge(Model_Vertices[1], Model_Vertices[6], Edge_Colour), // 7
                new Edge(Model_Vertices[2], Model_Vertices[6], Edge_Colour), // 8
                new Edge(Model_Vertices[4], Model_Vertices[5], Edge_Colour), // 9
                new Edge(Model_Vertices[4], Model_Vertices[7], Edge_Colour), // 10
                new Edge(Model_Vertices[5], Model_Vertices[7], Edge_Colour), // 11
                new Edge(Model_Vertices[6], Model_Vertices[7], Edge_Colour), // 12
                new Edge(Model_Vertices[0], Model_Vertices[4], Edge_Colour), // 13
                new Edge(Model_Vertices[3], Model_Vertices[4], Edge_Colour),  // 14
                new Edge(Model_Vertices[3], Model_Vertices[7], Edge_Colour), // 15
                new Edge(Model_Vertices[3], Model_Vertices[6], Edge_Colour), // 16
                new Edge(Model_Vertices[1], Model_Vertices[4], Edge_Colour) // 17
            };

            Faces = new Face[12]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Face_Colour), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Face_Colour), // 1
                new Face(Model_Vertices[1], Model_Vertices[6], Model_Vertices[2], Face_Colour), // 2
                new Face(Model_Vertices[1], Model_Vertices[5], Model_Vertices[6], Face_Colour), // 3
                new Face(Model_Vertices[4], Model_Vertices[7], Model_Vertices[5], Face_Colour), // 4
                new Face(Model_Vertices[5], Model_Vertices[7], Model_Vertices[6], Face_Colour), // 5
                new Face(Model_Vertices[0], Model_Vertices[3], Model_Vertices[4], Face_Colour), // 6
                new Face(Model_Vertices[4], Model_Vertices[3], Model_Vertices[7], Face_Colour), // 7
                new Face(Model_Vertices[7], Model_Vertices[3], Model_Vertices[6], Face_Colour), // 8
                new Face(Model_Vertices[6], Model_Vertices[3], Model_Vertices[2], Face_Colour), // 9
                new Face(Model_Vertices[4], Model_Vertices[5], Model_Vertices[0], Face_Colour), // 10
                new Face(Model_Vertices[5], Model_Vertices[1], Model_Vertices[0], Face_Colour) // 11
            };

            Debug.WriteLine($"Cuboid created at {origin}");
        }

        public Cuboid(Vector3D origin, Vector3D direction, Vector3D direction_up, double length, double width, double height, Bitmap texture,
            Color? vertex_colour = null,
            Color? edge_colour = null)
        {
            Length = length;
            Width = width;
            Height = height;

            Vertex_Colour = vertex_colour ?? Color.Blue;
            Edge_Colour = edge_colour ?? Color.Black;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = new Vertex[8]
            {
                new Vertex(0, 0, 0, Vertex_Colour), // 0
                new Vertex(1, 0, 0, Vertex_Colour), // 1
                new Vertex(1, 1, 0, Vertex_Colour), // 2
                new Vertex(0, 1, 0, Vertex_Colour), // 3
                new Vertex(0, 0, 1, Vertex_Colour), // 4
                new Vertex(1, 0, 1, Vertex_Colour), // 5
                new Vertex(1, 1, 1, Vertex_Colour), // 6
                new Vertex(0, 1, 1, Vertex_Colour) // 7
            };

            Texture_Vertices = new Texture_Vertex[4]
            {
                new Texture_Vertex(0, 0, 1), // 0
                new Texture_Vertex(1, 0, 1), // 1
                new Texture_Vertex(0, 1, 1), // 2
                new Texture_Vertex(1, 1, 1) // 3
            };

            Edges = new Edge[18]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1], Edge_Colour), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2], Edge_Colour), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2], Edge_Colour), // 2
                new Edge(Model_Vertices[2], Model_Vertices[3], Edge_Colour), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3], Edge_Colour), // 4
                new Edge(Model_Vertices[1], Model_Vertices[5], Edge_Colour), // 5
                new Edge(Model_Vertices[5], Model_Vertices[6], Edge_Colour), // 6
                new Edge(Model_Vertices[1], Model_Vertices[6], Edge_Colour), // 7
                new Edge(Model_Vertices[2], Model_Vertices[6], Edge_Colour), // 8
                new Edge(Model_Vertices[4], Model_Vertices[5], Edge_Colour), // 9
                new Edge(Model_Vertices[4], Model_Vertices[7], Edge_Colour), // 10
                new Edge(Model_Vertices[5], Model_Vertices[7], Edge_Colour), // 11
                new Edge(Model_Vertices[6], Model_Vertices[7], Edge_Colour), // 12
                new Edge(Model_Vertices[0], Model_Vertices[4], Edge_Colour), // 13
                new Edge(Model_Vertices[3], Model_Vertices[4], Edge_Colour),  // 14
                new Edge(Model_Vertices[3], Model_Vertices[7], Edge_Colour), // 15
                new Edge(Model_Vertices[3], Model_Vertices[6], Edge_Colour), // 16
                new Edge(Model_Vertices[1], Model_Vertices[4], Edge_Colour) // 17
            };

            Faces = new Face[12]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Texture_Vertices[3], Texture_Vertices[2], Texture_Vertices[0], texture), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture), // 1
                new Face(Model_Vertices[1], Model_Vertices[6], Model_Vertices[2], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture), // 2
                new Face(Model_Vertices[1], Model_Vertices[5], Model_Vertices[6], Texture_Vertices[3], Texture_Vertices[2], Texture_Vertices[0], texture), // 3
                new Face(Model_Vertices[4], Model_Vertices[7], Model_Vertices[5], Texture_Vertices[2], Texture_Vertices[0], Texture_Vertices[3], texture), // 4
                new Face(Model_Vertices[5], Model_Vertices[7], Model_Vertices[6], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture), // 5
                new Face(Model_Vertices[0], Model_Vertices[3], Model_Vertices[4], Texture_Vertices[2], Texture_Vertices[0], Texture_Vertices[3], texture), // 6
                new Face(Model_Vertices[4], Model_Vertices[3], Model_Vertices[7], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture), // 7
                new Face(Model_Vertices[7], Model_Vertices[3], Model_Vertices[6], Texture_Vertices[2], Texture_Vertices[0], Texture_Vertices[3], texture), // 8
                new Face(Model_Vertices[6], Model_Vertices[3], Model_Vertices[2], Texture_Vertices[3], Texture_Vertices[0], Texture_Vertices[1], texture), // 9
                new Face(Model_Vertices[4], Model_Vertices[5], Model_Vertices[0], Texture_Vertices[0], Texture_Vertices[1], Texture_Vertices[2], texture), // 10
                new Face(Model_Vertices[5], Model_Vertices[1], Model_Vertices[0], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture) // 11
            };

            Textures = new Bitmap[1]
            {
                texture // 0
            };

            Debug.WriteLine($"Cuboid created at {origin}";
        }
    }
}