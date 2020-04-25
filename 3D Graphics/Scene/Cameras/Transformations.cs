using System.Diagnostics;

namespace _3D_Graphics
{
    public abstract partial class Camera : Scene_Object
    {
        #region Rotations (Directions)
        public void Set_Camera_Direction_1(Vector3D new_world_direction, Vector3D new_world_direction_up)
        {
            //if (new_world_direction * new_world_direction_up != 0) throw new Exception("Camera direction vectors are not orthogonal.");
            new_world_direction = new_world_direction.Normalise(); new_world_direction_up = new_world_direction_up.Normalise();
            World_Direction = new_world_direction;
            World_Direction_Up = new_world_direction_up;
            World_Direction_Right = new_world_direction.Cross_Product(new_world_direction_up);
            Debug.WriteLine("Camera direction changed to:\n" +
                $"Forward: ({World_Direction.X}, {World_Direction.Y}, {World_Direction.Z})\n" +
                $"Up: ({World_Direction_Up.X}, {World_Direction_Up.Y}, {World_Direction_Up.Z})\n" +
                $"Right: ({World_Direction_Right.X}, {World_Direction_Right.Y}, {World_Direction_Right.Z})"
            );
        }
        public void Set_Camera_Direction_2(Vector3D new_world_direction_up, Vector3D new_world_direction_right)
        {
            //if (new_world_direction_up * new_world_direction_right != 0) throw new Exception("Camera direction vectors are not orthogonal.");
            new_world_direction_up = new_world_direction_up.Normalise(); new_world_direction_right = new_world_direction_right.Normalise();
            World_Direction = new_world_direction_up.Cross_Product(new_world_direction_right);
            World_Direction_Up = new_world_direction_up;
            World_Direction_Right = new_world_direction_right;
            Debug.WriteLine("Camera direction changed to:\n" +
                $"Forward: ({World_Direction.X}, {World_Direction.Y}, {World_Direction.Z})\n" +
                $"Up: ({World_Direction_Up.X}, {World_Direction_Up.Y}, {World_Direction_Up.Z})\n" +
                $"Right: ({World_Direction_Right.X}, {World_Direction_Right.Y}, {World_Direction_Right.Z})"
            );
        }
        public void Set_Camera_Direction_3(Vector3D new_world_direction_right, Vector3D new_world_direction)
        {
            //if (new_world_direction_right * new_world_direction != 0) throw new Exception("Camera direction vectors are not orthogonal.");
            new_world_direction_right = new_world_direction_right.Normalise(); new_world_direction = new_world_direction.Normalise();
            World_Direction = new_world_direction;
            World_Direction_Up = new_world_direction_right.Cross_Product(new_world_direction);
            World_Direction_Right = new_world_direction_right;
            Debug.WriteLine("Camera direction changed to:\n" +
                $"Forward: ({World_Direction.X}, {World_Direction.Y}, {World_Direction.Z})\n" +
                $"Up: ({World_Direction_Up.X}, {World_Direction_Up.Y}, {World_Direction_Up.Z})\n" +
                $"Right: ({World_Direction_Right.X}, {World_Direction_Right.Y}, {World_Direction_Right.Z})"
            );
        }
        #endregion

        #region Translations (Position)
        public Vector3D Translation { get; private set; }

        public void Translate_X(double distance) => Translation += new Vector3D(distance, 0, 0);
        public void Translate_Y(double distance) => Translation += new Vector3D(0, distance, 0);
        public void Translate_Z(double distance) => Translation += new Vector3D(0, 0, distance);
        public void Translate(Vector3D distance) => Translation += distance;
        #endregion
    }
}