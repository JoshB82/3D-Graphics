using System.Drawing;

namespace _3D_Graphics
{
    public partial class Vertex : Vector4D
    {
        #region Appearance
        public Color Colour { get; set; }
        public int Diameter { get; set; }
        public bool Visible { get; set; }
        #endregion

        public Vertex(double x, double y, double z, double w = 1, Color? colour = null) : base(x, y, z, w)
        {
            Colour = colour ?? Color.BlueViolet;
            Diameter = 10;
            Visible = true;
        }

        /*
        public Vertex(double x, double y, double z, Color? colour = null, bool visibility = true, int diameter = 10) : this(x, y, z, 1, colour, visibility, diameter) {}

        public Vertex(Vector3D position, double w = 1, Color? colour = null, bool visibility = true, int diameter = 10) : this(position.X, position.Y, position.Z, w, colour, visibility, diameter) {}

        public Vertex(double x, double y, double z, double w, Color? colour = null, bool visibility = true, int diameter = 10) : base(x, y, z, w)
        {
            Colour = colour ?? Color.Black;
            Diameter = diameter;
            Visible = visibility;
        }
        */
        /*
        #region Vertex Operations (Operator Overloading)
        public static Vertex operator +(Vertex v1, Vector4D v2) => new Vertex(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.Colour, v1.Visible, v1.Diameter);
        public static Vertex operator -(Vertex v1, Vector4D v2) => new Vertex(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.Colour, v1.Visible, v1.Diameter);
        public static Vertex operator *(Vertex v, double scalar) => new Vertex(v.X * scalar, v.Y * scalar, v.Z * scalar, v.Colour, v.Visible, v.Diameter);
        public static Vertex operator /(Vertex v, double scalar) => new Vertex(v.X / scalar, v.Y / scalar, v.Z / scalar, v.Colour, v.Visible, v.Diameter);
        #endregion
        */
    }

    public class Texture_Vertex : Vector3D
    {
        public Texture_Vertex(double x, double y, double z) : base(x, y, z) { }
    }

    public class Edge
    {
        public Vector4D P1 { get; set; }
        public Vector4D P2 { get; set; }
        public Color Colour { get; set; }
        public bool Visible { get; set; }

        public Edge(Vector4D p1, Vector4D p2, Color? colour = null)
        {
            P1 = p1;
            P2 = p2;
            Colour = colour ?? Color.Black;
            Visible = true;
        }
    }

    public class Face
    {
        public Vector4D P1 { get; set; }
        public Vector4D P2 { get; set; }
        public Vector4D P3 { get; set; }

        public Vector3D T1 { get; set; }
        public Vector3D T2 { get; set; }
        public Vector3D T3 { get; set; }

        public Color Colour { get; set; }
        public Bitmap Texture { get; set; }

        public bool Draw_Outline { get; set; }
        public bool Visible { get; set; }

        public Face(Vector4D p1, Vector4D p2, Vector4D p3, Color? colour = null)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            Colour = colour ?? Color.SeaGreen;
            Draw_Outline = false;
            Visible = true;
        }

        public Face(Vector4D p1, Vector4D p2, Vector4D p3, Vector3D t1, Vector3D t2, Vector3D t3, Bitmap texture)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            T1 = t1;
            T2 = t2;
            T3 = t3;
            Texture = texture;
            Draw_Outline = false;
            Visible = true;
        }
    }
}