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

        public Plane(Vector3D origin, Vector3D direction, Vector3D normal, double length, double width)
        {
            Length = length;
            Width = width;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, normal);

            Model_Vertices = new Vector4D[4]
            {
                new Vector4D(0, 0, 0), // 0
                new Vector4D(1, 0, 0), // 1
                new Vector4D(1, 0, 1), // 2
                new Vector4D(0, 0, 1) // 3
            };

            Spots = new Spot[4]
            {
                new Spot(Model_Vertices[0]), // 0
                new Spot(Model_Vertices[1]), // 1
                new Spot(Model_Vertices[2]), // 2
                new Spot(Model_Vertices[3]) // 3
            };

            Edges = new Edge[5]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1]), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2]), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2]) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3]), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3]) // 4
            };

            Faces = new Face[2]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2]), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3]) // 1
            };

            Spot_Colour = Color.Blue;
            Edge_Colour = Color.Black;
            Face_Colour = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); // Green

            Debug.WriteLine($"Plane created at {origin}");
        }

        public Plane(Vector3D origin, Vector3D direction, Vector3D normal, double length, double width, Bitmap texture)
        {
            Length = length;
            Width = width;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, normal);

            Model_Vertices = new Vector4D[4]
            {
                new Vector4D(0, 0, 0), // 0
                new Vector4D(1, 0, 0), // 1
                new Vector4D(1, 0, 1), // 2
                new Vector4D(0, 0, 1) // 3
            };

            Texture_Vertices = new Vector3D[4]
            {
                // WHY Z=1?
                new Vector3D(0, 0, 1), // 0
                new Vector3D(1, 0, 1), // 1
                new Vector3D(1, 1, 1), // 2
                new Vector3D(0, 1, 1) // 3
            };

            Spots = new Spot[4]
            {
                new Spot(Model_Vertices[0]), // 0
                new Spot(Model_Vertices[1]), // 1
                new Spot(Model_Vertices[2]), // 2
                new Spot(Model_Vertices[3]) // 3
            };

            Edges = new Edge[5]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1]), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2]), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2]) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3]), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3]) // 4
            };

            Faces = new Face[2]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Texture_Vertices[0], Texture_Vertices[1], Texture_Vertices[2], texture), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Texture_Vertices[0], Texture_Vertices[2], Texture_Vertices[3], texture) // 1
            };

            Textures = new Bitmap[1]
            {
                texture // 0
            };

            Spot_Colour = Color.Blue;
            Edge_Colour = Color.Black;

            Debug.WriteLine($"Plane created at {origin}");
        }
    }
}