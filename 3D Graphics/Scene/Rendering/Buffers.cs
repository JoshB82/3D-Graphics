using System.Drawing;

namespace _3D_Graphics
{
    public sealed partial class Scene
    {
        private void Check_Against_Z_Buffer(int x, int y, double z, Color new_colour)
        {
            if (z < z_buffer[x][y])
            {
                z_buffer[x][y] = z;
                colour_buffer[x][y] = new_colour;
            }
        }

        private void Check_Against_Z_Buffer_Texture(int x, int y, int tx, int ty, double z, Bitmap texture)
        {
            if (z < z_buffer[x][y])
            {
                z_buffer[x][y] = z;
                colour_buffer[x][y] = texture.GetPixel(tx, ty * -1 + texture.Height - 1);
            }
        }
    }
}
