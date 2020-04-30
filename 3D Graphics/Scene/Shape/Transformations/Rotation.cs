using System;

namespace _3D_Graphics
{
    public static partial class Transform
    {
        /// <summary>
        /// Creates a matrix for rotation about the x-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        /// <returns>Rotation matrix.</returns>
        public static Matrix4x4 Rotate_X(double angle)
        {
            double sin_angle = Math.Sin(angle);
            double cos_angle = Math.Cos(angle);
            Matrix4x4 rotation = Matrix4x4.Identity_Matrix();
            rotation.Data[1][1] = cos_angle;
            rotation.Data[1][2] = -sin_angle;
            rotation.Data[2][1] = sin_angle;
            rotation.Data[2][2] = cos_angle;
            return rotation;
        }

        /// <summary>
        /// Creates a matrix for rotation about the y-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        /// <returns>Rotation matrix.</returns>
        public static Matrix4x4 Rotate_Y(double angle)
        {
            double sin_angle = Math.Sin(angle);
            double cos_angle = Math.Cos(angle);
            Matrix4x4 rotation = Matrix4x4.Identity_Matrix();
            rotation.Data[0][0] = cos_angle;
            rotation.Data[0][2] = sin_angle;
            rotation.Data[2][0] = -sin_angle;
            rotation.Data[2][2] = cos_angle;
            return rotation;
        }

        /// <summary>
        /// Creates a matrix for rotation about the z-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate.</param>
        /// <returns>Rotation matrix.</returns>
        public static Matrix4x4 Rotate_Z(double angle)
        {
            double sin_angle = Math.Sin(angle);
            double cos_angle = Math.Cos(angle);
            Matrix4x4 rotation = Matrix4x4.Identity_Matrix();
            rotation.Data[0][0] = cos_angle;
            rotation.Data[0][1] = -sin_angle;
            rotation.Data[1][0] = sin_angle;
            rotation.Data[1][1] = cos_angle;
            return rotation;
        }

        /// <summary>
        /// Creates a rotation matrix that would rotate v1 onto v2.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>Rotation matrix</returns>
        public static Matrix4x4 Rotation_Between_Vectors(Vector3D v1, Vector3D v2)
        {
            // Create a normalised rotation axis
            Vector3D rotation_axis = v1.Cross_Product(v2).Normalise();
            // Find the angle to rotate and its sine and cosine
            double angle = v1.Angle(v2);
            double sin_angle = Math.Sin(angle);
            double cos_angle = Math.Cos(angle);
            // Construct the rotation matrix
            if (angle == 0)
            {
                return Matrix4x4.Identity_Matrix();
            }
            else
            {
                double[][] data = new double[4][]
                    {
                    new double[] {
                        cos_angle + Math.Pow(rotation_axis.X, 2) * (1 - cos_angle),
                        rotation_axis.X * rotation_axis.Y * (1 - cos_angle) - rotation_axis.Z * sin_angle,
                        rotation_axis.X * rotation_axis.Z * (1 - cos_angle) + rotation_axis.Y * sin_angle,
                        0
                    },
                    new double[] {
                        rotation_axis.Y * rotation_axis.X * (1 - cos_angle) + rotation_axis.Z * sin_angle,
                        cos_angle + Math.Pow(rotation_axis.Y, 2) * (1 - cos_angle),
                        rotation_axis.Y * rotation_axis.Z * (1 - cos_angle) - rotation_axis.X * sin_angle,
                        0
                    },
                    new double[] {
                        rotation_axis.Z * rotation_axis.X * (1 - cos_angle) - rotation_axis.Y * sin_angle,
                        rotation_axis.Z * rotation_axis.Y * (1 - cos_angle) + rotation_axis.X * sin_angle,
                        cos_angle + Math.Pow(rotation_axis.Z, 2) * (1 - cos_angle),
                        0
                    },
                    new double[] {
                        0,0,0,1
                    }
                    };
                return new Matrix4x4(data);
            }
        }

        public static Quaternion Quaternion_Rotation(Vector3D axis, double angle) => new Quaternion(Math.Cos(angle / 2), axis.Normalise() * Math.Sin(angle / 2)).Normalise();

        // Must supply rotation axis if vectors are antiparallel.
        public static Quaternion Quaternion_Rotation_Between_Vectors(Vector3D v1, Vector3D v2, Vector3D? rotation_axis = null)
        {
            v1 = v1.Normalise(); v2 = v2.Normalise();
            Vector3D axis = rotation_axis ?? v1.Cross_Product(v2);
            double angle = v1.Angle(v2);
            return (angle == 0) ? new Quaternion(1, 0, 0, 0) : Quaternion_Rotation(axis, angle);
        }

        public static Matrix4x4 Quaternion_to_Matrix(Quaternion q) =>
            // RIGHT HANDED ROTATION
            // (ANTI CLOCKWISE WHEN LOOKING AT ORIGIN FROM ARROW TIP TO BEGINNING)

            new Matrix4x4(
                1 - 2 * (Math.Pow(q.Q3, 2) + Math.Pow(q.Q4, 2)),
                2 * (q.Q2 * q.Q3 - q.Q4 * q.Q1),
                2 * (q.Q2 * q.Q4 + q.Q3 * q.Q1),
                0,
                2 * (q.Q2 * q.Q3 + q.Q4 * q.Q1),
                1 - 2 * (Math.Pow(q.Q2, 2) + Math.Pow(q.Q4, 2)),
                2 * (q.Q3 * q.Q4 - q.Q2 * q.Q1),
                0,
                2 * (q.Q2 * q.Q4 - q.Q3 * q.Q1),
                2 * (q.Q3 * q.Q4 + q.Q2 * q.Q1),
                1 - 2 * (Math.Pow(q.Q2, 2) + Math.Pow(q.Q3, 2)),
                0,
                0,
                0,
                0,
                1
            );

        public static Matrix4x4 Quaternion_Rotation_Matrix(Vector3D v1, Vector3D v2, Vector3D? rotation_axis = null) => Quaternion_to_Matrix(Quaternion_Rotation_Between_Vectors(v1, v2, rotation_axis));
        public static Matrix4x4 Quaternion_Rotation_Axis_Matrix(Vector3D axis, double angle) => Transform.Quaternion_to_Matrix(Transform.Quaternion_Rotation(axis, angle));
    }
}