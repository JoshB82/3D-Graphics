﻿using System.Drawing;
using System.Linq.Expressions;

namespace _3D_Graphics
{
    public struct Spot
    {
        public Vector4D Model_Point { get; set; }
        public Vector4D World_Point { get; set; }

        #region Appearance
        public Color Colour { get; set; }
        public int Diameter { get; set; }
        public bool Visible { get; set; }
        #endregion

        public Spot(Vector4D origin, Color? colour = null) : this()
        {
            Model_Point = origin;
            Colour = colour ?? Color.BlueViolet;
            Diameter = 10;
            Visible = true;
        }

        public Spot(Vector3D origin, Color? colour = null) : this(new Vector4D(origin), colour) { }
    }

    public struct Edge
    {
        public Vector4D Model_P1 { get; set; }
        public Vector4D Model_P2 { get; set; }
        public Vector4D World_P1 { get; set; }
        public Vector4D World_P2 { get; set; }
        public Color Colour { get; set; }
        public bool Visible { get; set; }

        public Edge(Vector4D p1, Vector4D p2, Color? colour = null)
        {
            Model_P1 = p1;
            Model_P2 = p2;
            World_P1 = p1; // ?
            World_P2 = p2; // ?
            Colour = colour ?? Color.Black;
            Visible = true;
        }
    }

    public struct Face
    {
        public Vector4D Model_P1 { get; set; }
        public Vector4D Model_P2 { get; set; }
        public Vector4D Model_P3 { get; set; }
        public Vector4D World_P1 { get; set; }
        public Vector4D World_P2 { get; set; }
        public Vector4D World_P3 { get; set; }

        public Vector3D T1 { get; set; }
        public Vector3D T2 { get; set; }
        public Vector3D T3 { get; set; }

        public Color Colour { get; set; }
        public Bitmap Texture { get; set; }

        public bool Draw_Outline { get; set; }
        public bool Visible { get; set; }

        // Don't understand how this() works :(
        public Face(Vector4D p1, Vector4D p2, Vector4D p3, Color? colour = null) : this()
        {
            Model_P1 = p1;
            Model_P2 = p2;
            Model_P3 = p3;
            World_P1 = p1; // ?
            World_P2 = p2; // ?
            World_P3 = p3; // ?
            Colour = colour ?? Color.SeaGreen;
            Draw_Outline = false;
            Visible = true;
        }

        public Face(Vector4D p1, Vector4D p2, Vector4D p3, Vector3D t1, Vector3D t2, Vector3D t3, Bitmap texture) : this()
        {
            Model_P1 = p1;
            Model_P2 = p2;
            Model_P3 = p3;
            World_P1 = p1; // ?
            World_P2 = p2; // ?
            World_P3 = p3; // ?
            T1 = t1;
            T2 = t2;
            T3 = t3;
            Texture = texture;
            Draw_Outline = false;
            Visible = true;
        }
    }
}