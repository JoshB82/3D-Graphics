﻿using System.Diagnostics;

namespace _3D_Graphics
{
    public sealed class Sphere : Mesh
    {
        public Sphere(Vector3D origin, Vector3D direction, Vector3D direction_up, double radius, int resolution)
        {
            World_Origin = new Vector4D(origin);
            Set_Shape_Direction_1(direction, direction_up);

            Debug.WriteLine($"Sphere created at {origin}");
        }
    }
}