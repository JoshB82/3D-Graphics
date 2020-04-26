using System.Diagnostics;
using System.Drawing;

namespace _3D_Graphics
{
    public sealed class Line : Mesh
    {
        public Line(Vector3D start_position, Vector3D end_position)
        {
            Vertex_Colour = Color.Blue;
            Edge_Colour = Color.Black;

            World_Origin = new Vector4D(start_position);
            Set_Shape_Direction_1(Vector3D.Unit_X, Vector3D.Unit_Y);

            Model_Vertices = new Vertex[2]
            {
                new Vertex(0, 0, 0, Vertex_Colour),
                new Vertex(1, 1, 1, Vertex_Colour)
            };

            Edges = new Edge[1]
            {
                new Edge(Model_Vertices[0], Model_Vertices[1], Edge_Colour)
            };

            Draw_Faces = false;

            Vector3D line_vector = end_position - start_position;
            Scaling = new Vector3D(line_vector.X, line_vector.Y, line_vector.Z);

            Debug.WriteLine($"Line created at {start_position}");
        }
    }
}