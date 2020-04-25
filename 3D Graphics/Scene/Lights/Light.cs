using System.Drawing;

namespace _3D_Graphics
{
    public abstract class Light : Scene_Object
    {
        public Vector3D Model_Origin { get; } = Vector3D.Zero;
        public Vector4D World_Origin { get; set; }
        public Vector4D Camera_Origin { get; protected set; } //?

        public Vector3D Model_Direction { get; } = Vector3D.Unit_X;
        public Vector3D Model_Direction_Up { get; } = Vector3D.Unit_Y;
        public Vector3D Model_Direction_Right { get; } = Vector3D.Unit_Z;

        private Vector3D world_direction;
        public Vector3D World_Direction
        {
            get { return world_direction; }
            set { world_direction = value.Normalise(); }
        }
        public Vector3D World_Direction_Up { get; set; }
        public Vector3D World_Direction_Right { get; set; }

        // Transformations
        public Vector3D Translation { get; protected set; }

        public Color Colour { get; set; }
        public double Intensity { get; set; }
        public string Icon { get; } = "";
    }
}