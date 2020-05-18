using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    /// <summary>
    /// Handles creation of a cube mesh.
    /// </summary>
    public sealed class Cube : Mesh
    {
        private double side_length;
        public double Side_Length
        {
            get => side_length;
            set
            {
                side_length = value;
                Scaling = new Vector3D(side_length, side_length, side_length);
            }
        }

        public Cube(Vector3D origin, Vector3D direction, Vector3D direction_up, double side_length)
        {
            Side_Length = side_length;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = new Vector4D[8]
            {
                new Vector4D(0, 0, 0), // 0
                new Vector4D(1, 0, 0), // 1
                new Vector4D(1, 1, 0), // 2
                new Vector4D(0, 1, 0), // 3
                new Vector4D(0, 0, 1), // 4
                new Vector4D(1, 0, 1), // 5
                new Vector4D(1, 1, 1), // 6
                new Vector4D(0, 1, 1) // 7
            };

            Spots = new Spot[8]
            {
                new Spot(Model_Vertices[0]), // 0
                new Spot(Model_Vertices[1]), // 1
                new Spot(Model_Vertices[2]), // 2
                new Spot(Model_Vertices[3]), // 3
                new Spot(Model_Vertices[4]), // 4
                new Spot(Model_Vertices[5]), // 5
                new Spot(Model_Vertices[6]), // 6
                new Spot(Model_Vertices[7]) // 7
            };

            Edges = new Edge[18]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1]), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2]), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2]) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3]), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3]), // 4
                new Edge(Model_Vertices[1], Model_Vertices[5]), // 5
                new Edge(Model_Vertices[5], Model_Vertices[6]), // 6
                new Edge(Model_Vertices[1], Model_Vertices[6]) { Visible = false }, // 7
                new Edge(Model_Vertices[2], Model_Vertices[6]), // 8
                new Edge(Model_Vertices[4], Model_Vertices[5]), // 9
                new Edge(Model_Vertices[4], Model_Vertices[7]), // 10
                new Edge(Model_Vertices[5], Model_Vertices[7]) { Visible = false }, // 11
                new Edge(Model_Vertices[6], Model_Vertices[7]), // 12
                new Edge(Model_Vertices[0], Model_Vertices[4]), // 13
                new Edge(Model_Vertices[3], Model_Vertices[4]) { Visible = false }, // 14
                new Edge(Model_Vertices[3], Model_Vertices[7]), // 15
                new Edge(Model_Vertices[3], Model_Vertices[6]) { Visible = false }, // 16
                new Edge(Model_Vertices[1], Model_Vertices[4]) { Visible = false }// 17
            };

            Faces = new Face[12]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2]), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3]), // 1
                new Face(Model_Vertices[1], Model_Vertices[6], Model_Vertices[2]), // 2
                new Face(Model_Vertices[1], Model_Vertices[5], Model_Vertices[6]), // 3
                new Face(Model_Vertices[4], Model_Vertices[7], Model_Vertices[5]), // 4
                new Face(Model_Vertices[5], Model_Vertices[7], Model_Vertices[6]), // 5
                new Face(Model_Vertices[0], Model_Vertices[3], Model_Vertices[4]), // 6
                new Face(Model_Vertices[4], Model_Vertices[3], Model_Vertices[7]), // 7
                new Face(Model_Vertices[7], Model_Vertices[3], Model_Vertices[6]), // 8
                new Face(Model_Vertices[6], Model_Vertices[3], Model_Vertices[2]), // 9
                new Face(Model_Vertices[4], Model_Vertices[5], Model_Vertices[1]), // 10
                new Face(Model_Vertices[4], Model_Vertices[1], Model_Vertices[0]) // 11
            };

            Spot_Colour = Color.Blue;
            Edge_Colour = Color.Black;
            Face_Colour = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); // Green

            Debug.WriteLine($"Cube created at {origin}");
        }

        public Cube(Vector3D origin, Vector3D direction, Vector3D direction_up, double side_length, Bitmap texture)
        {
            Side_Length = side_length;

            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Model_Vertices = new Vector4D[8]
            {
                new Vector4D(0, 0, 0), // 0
                new Vector4D(1, 0, 0), // 1
                new Vector4D(1, 1, 0), // 2
                new Vector4D(0, 1, 0), // 3
                new Vector4D(0, 0, 1), // 4
                new Vector4D(1, 0, 1), // 5
                new Vector4D(1, 1, 1), // 6
                new Vector4D(0, 1, 1) // 7
            };

            Texture_Vertices = new Vector3D[4]
            {
                new Vector3D(0, 0, 1), // 0
                new Vector3D(1, 0, 1), // 1
                new Vector3D(1, 1, 1), // 2
                new Vector3D(0, 1, 1) // 3
            };

            Spots = new Spot[8]
            {
                new Spot(Model_Vertices[0]), // 0
                new Spot(Model_Vertices[1]), // 1
                new Spot(Model_Vertices[2]), // 2
                new Spot(Model_Vertices[3]), // 3
                new Spot(Model_Vertices[4]), // 4
                new Spot(Model_Vertices[5]), // 5
                new Spot(Model_Vertices[6]), // 6
                new Spot(Model_Vertices[7]) // 7
            };

            Edges = new Edge[18]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1]), // 0
                new Edge(Model_Vertices[1], Model_Vertices[2]), // 1
                new Edge(Model_Vertices[0], Model_Vertices[2]) { Visible = false }, // 2
                new Edge(Model_Vertices[2], Model_Vertices[3]), // 3
                new Edge(Model_Vertices[0], Model_Vertices[3]), // 4
                new Edge(Model_Vertices[1], Model_Vertices[5]), // 5
                new Edge(Model_Vertices[5], Model_Vertices[6]), // 6
                new Edge(Model_Vertices[1], Model_Vertices[6]) { Visible = false }, // 7
                new Edge(Model_Vertices[2], Model_Vertices[6]), // 8
                new Edge(Model_Vertices[4], Model_Vertices[5]), // 9
                new Edge(Model_Vertices[4], Model_Vertices[7]), // 10
                new Edge(Model_Vertices[5], Model_Vertices[7]) { Visible = false }, // 11
                new Edge(Model_Vertices[6], Model_Vertices[7]), // 12
                new Edge(Model_Vertices[0], Model_Vertices[4]), // 13
                new Edge(Model_Vertices[3], Model_Vertices[4]) { Visible = false }, // 14
                new Edge(Model_Vertices[3], Model_Vertices[7]), // 15
                new Edge(Model_Vertices[3], Model_Vertices[6]) { Visible = false }, // 16
                new Edge(Model_Vertices[1], Model_Vertices[4]) { Visible = false }// 17
            };

            Faces = new Face[12]
            {
                new Face(Model_Vertices[0], Model_Vertices[1], Model_Vertices[2], Texture_Vertices[1], Texture_Vertices[0], Texture_Vertices[3], texture), // 0
                new Face(Model_Vertices[0], Model_Vertices[2], Model_Vertices[3], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture), // 1
                new Face(Model_Vertices[1], Model_Vertices[6], Model_Vertices[2], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture), // 2
                new Face(Model_Vertices[1], Model_Vertices[5], Model_Vertices[6], Texture_Vertices[1], Texture_Vertices[0], Texture_Vertices[3], texture), // 3
                new Face(Model_Vertices[4], Model_Vertices[7], Model_Vertices[5], Texture_Vertices[0], Texture_Vertices[3], Texture_Vertices[1], texture), // 4
                new Face(Model_Vertices[5], Model_Vertices[7], Model_Vertices[6], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture), // 5
                new Face(Model_Vertices[0], Model_Vertices[3], Model_Vertices[4], Texture_Vertices[0], Texture_Vertices[3], Texture_Vertices[1], texture), // 6
                new Face(Model_Vertices[4], Model_Vertices[3], Model_Vertices[7], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture), // 7
                new Face(Model_Vertices[7], Model_Vertices[3], Model_Vertices[6], Texture_Vertices[0], Texture_Vertices[3], Texture_Vertices[1], texture), // 8
                new Face(Model_Vertices[6], Model_Vertices[3], Model_Vertices[2], Texture_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], texture), // 9
                new Face(Model_Vertices[4], Model_Vertices[5], Model_Vertices[1], Texture_Vertices[3], Texture_Vertices[2], Texture_Vertices[1], texture), // 10
                new Face(Model_Vertices[4], Model_Vertices[1], Model_Vertices[0], Texture_Vertices[3], Texture_Vertices[1], Texture_Vertices[0], texture) // 11
            };

            Textures = new Bitmap[1]
            {
                texture // 0
            };

            Spot_Colour = Color.Blue;
            Edge_Colour = Color.Black;

            Debug.WriteLine($"Cube created at {origin}");
        }
    }
}