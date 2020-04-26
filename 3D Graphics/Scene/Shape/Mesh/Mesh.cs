using System;
using System.Drawing;

namespace _3D_Graphics
{
    public abstract partial class Mesh
    {
        #region Structure
        public Vector4D Model_Origin { get; } = Vector4D.Zero;
        public Vector4D World_Origin { get; set; }
        public Vector4D Camera_Origin { get; protected set; }

        private Vertex[] model_vertices;
        public Vertex[] Model_Vertices
        {
            get { return model_vertices; }
            protected set
            {
                model_vertices = value;
                World_Vertices = new Vertex[value.Length];
                Camera_Vertices = new Vertex[value.Length];
            }
        }
        public Vertex[] World_Vertices { get; private set; }
        public Vertex[] Camera_Vertices { get; protected set; }

        public Texture_Vertex[] Texture_Vertices { get; protected set; }
        public Bitmap[] Textures { get; protected set; }

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

        public void Set_Shape_Direction_1(Vector3D new_world_direction, Vector3D new_world_direction_up)
        {
            if (new_world_direction * new_world_direction_up != 0) throw new Exception("Shape direction vectors are not orthogonal.");
            new_world_direction = new_world_direction.Normalise(); new_world_direction_up = new_world_direction_up.Normalise();
            World_Direction = new_world_direction;
            World_Direction_Up = new_world_direction_up;
            World_Direction_Right = new_world_direction.Cross_Product(new_world_direction_up);
        }
        public void Set_Shape_Direction_2(Vector3D new_world_direction_up, Vector3D new_world_direction_right)
        {
            if (new_world_direction_up * new_world_direction_right != 0) throw new Exception("Shape direction vectors are not orthogonal.");
            new_world_direction_up = new_world_direction_up.Normalise(); new_world_direction_right = new_world_direction_right.Normalise();
            World_Direction = new_world_direction_up.Cross_Product(new_world_direction_right); ;
            World_Direction_Up = new_world_direction_up;
            World_Direction_Right = new_world_direction_right;
        }
        public void Set_Shape_Direction_3(Vector3D new_world_direction_right, Vector3D new_world_direction)
        {
            if (new_world_direction_right * new_world_direction != 0) throw new Exception("Shape direction vectors are not orthogonal.");
            new_world_direction_right = new_world_direction_right.Normalise(); new_world_direction = new_world_direction.Normalise();
            World_Direction = new_world_direction;
            World_Direction_Up = new_world_direction_right.Cross_Product(new_world_direction);
            World_Direction_Right = new_world_direction_right;
        }
        #endregion

        #region Draw settings
        public bool Draw_Vertices { get; set; } = true;
        public bool Draw_Edges { get; set; } = true;
        public bool Draw_Faces { get; set; } = true;
        #endregion

        #region Colours
        public Color Vertex_Colour { get; set; }
        public Color Edge_Colour { get; set; }
        public Color Face_Colour { get; set; }
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
        public Matrix4x4 Model_to_world { get; private set; }
        public Vector3D Scaling { get; protected set; } = Vector3D.One;

        // Scale, then rotate, then translate
        public void Calculate_Model_to_World_Matrix()
        {
            Matrix4x4 direction_rotation = Transform.Quaternion_Rotation_Matrix(Model_Direction, World_Direction);
            Matrix4x4 direction_up_rotation = Transform.Quaternion_Rotation_Matrix(new Vector3D(direction_rotation * new Vector4D(Model_Direction_Up)), World_Direction_Up);
            Matrix4x4 scale = Transform.Scale(Scaling.X, Scaling.Y, Scaling.Z);
            Matrix4x4 translation = Transform.Translate(new Vector3D(World_Origin));

            Model_to_world = translation * direction_up_rotation * direction_rotation * scale;
        }

        public void Apply_World_Matrices()
        {
            World_Origin = Model_to_world * Model_Origin;
            for (int i = 0; i < Model_Vertices.Length; i++) World_Vertices[i] = Model_to_world * Model_Vertices[i];
        }
    }
}