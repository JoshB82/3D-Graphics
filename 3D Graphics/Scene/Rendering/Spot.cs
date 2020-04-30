namespace _3D_Graphics
{
    public sealed partial class Scene
    {
        public void Draw_Spot(Spot spot)
        {
            Sphere sphere_spot = new Sphere(new Vector3D(spot.World_Point), Vector3D.Unit_X, Vector3D.Unit_Y, 0, 0);
        }
    }
}