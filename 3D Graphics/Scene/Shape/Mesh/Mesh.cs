﻿using System;
using System.Drawing;

namespace _3D_Graphics
{
    public abstract partial class Mesh
    {
        // SORT OUT!
        #region Structure
        public Vector4D Model_Origin { get; } = Vector4D.Zero;
        public Vector4D World_Origin { get; set; }

        private Vector4D[] model_vertices;
        public Vector4D[] Model_Vertices
        {
            get => model_vertices;
            protected set
            {
                model_vertices = value;
                World_Vertices = new Vector4D[value.Length];
            }
        }
        public Vector4D[] World_Vertices { get; protected set; }

        public Vector3D[] Texture_Vertices { get; protected set; }
        public Bitmap[] Textures { get; protected set; }

        public Spot[] Spots { get; protected set; }
        public Edge[] Edges { get; protected set; }
        public Face[] Faces { get; protected set; }
        #endregion

        #region Directions
        public Vector3D Model_Direction { get; } = Vector3D.Unit_X;
        public Vector3D Model_Direction_Up { get; } = Vector3D.Unit_Y;
        public Vector3D Model_Direction_Right { get; } = Vector3D.Unit_Z;

        public Vector3D World_Direction { get; private set; }
        public Vector3D World_Direction_Up { get; private set; }
        public Vector3D World_Direction_Right { get; private set; }
        #endregion

        #region Draw settings
        public bool Draw_Spots { get; set; } = true;
        public bool Draw_Edges { get; set; } = true;
        public bool Draw_Faces { get; set; } = true;
        #endregion

        #region Colours
        private Color spot_colour, edge_colour, face_colour;
        public Color Spot_Colour
        {
            get => spot_colour;
            set
            {
                spot_colour = value;
                // Not entirely sure why can't use foreach loop :/
                for (int i = 0; i < Spots.Length; i++) Spots[i].Colour = value;
            }
        }
        public Color Edge_Colour
        {
            get => edge_colour;
            set
            {
                edge_colour = value;
                // Not entirely sure why can't use foreach loop :/
                for (int i = 0; i < Edges.Length; i++) Edges[i].Colour = value;
            }
        }
        public Color Face_Colour
        {
            get => face_colour;
            set
            {
                face_colour = value;
                // Not entirely sure why can't use foreach loop :/
                for (int i = 0; i < Faces.Length; i++) Faces[i].Colour = value;
            }
        }
        #endregion

        // Miscellaneous
        /// <summary>
        /// Determines if an outline is drawn with the mesh.
        /// </summary>
        public bool Draw_Outline { get; set; } = false;
        /// <summary>
        /// Determines if the mesh is visible or not.
        /// </summary>
        public bool Visible { get; set; } = true;

        // Object transformations
        public Matrix4x4 Model_To_World { get; private set; }
        public Vector3D Scaling { get; protected set; } = Vector3D.One;

        public void Calculate_Model_to_World_Matrix()
        {
            // Scale, then rotate, then translate
            Matrix4x4 direction_rotation = Transform.Rotate_Between_Vectors(Model_Direction, World_Direction);
            Matrix4x4 direction_up_rotation = Transform.Rotate_Between_Vectors(new Vector3D(direction_rotation * new Vector4D(Model_Direction_Up)), World_Direction_Up);
            Matrix4x4 scale = Transform.Scale(Scaling.X, Scaling.Y, Scaling.Z);
            Matrix4x4 translation = Transform.Translate(new Vector3D(World_Origin));

            Model_To_World = translation * direction_up_rotation * direction_rotation * scale;
        }

        // Could world and model points be put into a single struct? With overloading possibly so the computer knows how to handle them?
        public void Apply_World_Matrices()
        {
            World_Origin = Model_To_World * Model_Origin;
            for (int i = 0; i < Model_Vertices.Length; i++) World_Vertices[i] = Model_To_World * Model_Vertices[i];
            if (Draw_Spots) for (int i = 0; i < Spots.Length; i++) Spots[i].World_Point = Model_To_World * Spots[i].Model_Point;
            if (Draw_Edges)
            {
                for (int i = 0; i < Edges.Length; i++)
                {
                    Edges[i].World_P1 = Model_To_World * Edges[i].Model_P1;
                    Edges[i].World_P2 = Model_To_World * Edges[i].Model_P2;
                }
            }
            if (Draw_Faces)
            {
                for (int i = 0; i < Faces.Length; i++)
                {
                    Faces[i].World_P1 = Model_To_World * Faces[i].Model_P1;
                    Faces[i].World_P2 = Model_To_World * Faces[i].Model_P2;
                    Faces[i].World_P3 = Model_To_World * Faces[i].Model_P3;
                }
            }
        }
    }
}