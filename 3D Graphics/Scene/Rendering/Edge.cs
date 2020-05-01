namespace _3D_Graphics
{
    public sealed partial class Scene
    {
        public void Draw_Edge(Edge edge)
        {
            // Clip the edge in world space
            foreach (Clipping_Plane world_clipping_plane in Render_Camera.world_clipping_planes)
            {
                if (!Clip_Edge(world_clipping_plane.Point, world_clipping_plane.Normal, ref edge)) return;
            }

            // Transform the edge into screen space and correct for perspective
            edge.World_P1 = Render_Camera.World_to_screen * edge.World_P1;
            edge.World_P2 = Render_Camera.World_to_screen * edge.World_P2;

            edge.World_P1 /= edge.World_P1.W;
            edge.World_P2 /= edge.World_P2.W;

            // Clip the edge in screen space
            foreach (Clipping_Plane projection_clipping_plane in projection_clipping_planes)
            {
                if (!Clip_Edge(projection_clipping_plane.Point, projection_clipping_plane.Normal, ref edge)) return;
            }

            // Scale to the full screen size
            Vector4D result_point_1 = Scale_to_screen(edge.World_P1);
            Vector4D result_point_2 = Scale_to_screen(edge.World_P2);

            // Variable simplification
            int result_point_1_x = Round_To_Int(result_point_1.X);
            int result_point_1_y = Round_To_Int(result_point_1.Y);
            double result_point_1_z = result_point_1.Z;
            int result_point_2_x = Round_To_Int(result_point_2.X);
            int result_point_2_y = Round_To_Int(result_point_2.Y);
            double result_point_2_z = result_point_2.Z;

            // Finally draw the line
            Line(result_point_1_x, result_point_1_y, result_point_1_z, result_point_2_x, result_point_2_y, result_point_2_z, edge.Colour);
        }
    }
}