﻿namespace _3D_Graphics
{
    public sealed partial class Scene
    {
        public void Draw_Camera(Camera camera, string camera_type,
            Matrix4x4 model_to_world,
            Matrix4x4 world_to_view,
            Matrix4x4 view_to_screen)
        {
            double semi_width = camera.Width / 2, semi_height = camera.Height / 2;
            double ratio = camera.Z_Far / camera.Z_Near;

            Vector4D origin = camera.World_Origin;
            Vector4D near_top_left_point = origin + new Vector4D(-semi_width, semi_height, camera.Z_Near);
            Vector4D near_top_right_point = origin + new Vector4D(semi_width, semi_height, camera.Z_Near);
            Vector4D near_bottom_left_point = origin + new Vector4D(-semi_width, -semi_height, camera.Z_Near);
            Vector4D near_bottom_right_point = origin + new Vector4D(semi_width, -semi_height, camera.Z_Near);

            if (camera.Draw_Near_View || camera.Draw_Entire_View)
            {
                Edge near_top_left_edge = new Edge(origin, near_top_left_point);
                Edge near_top_right_edge = new Edge(origin, near_top_right_point);
                Edge near_bottom_left_edge = new Edge(origin, near_bottom_left_point);
                Edge near_bottom_right_edge = new Edge(origin, near_bottom_right_point);
                Edge near_top_edge = new Edge(near_top_left_point, near_top_right_point);
                Edge near_bottom_edge = new Edge(near_bottom_left_point, near_bottom_right_point);
                Edge near_left_edge = new Edge(near_top_left_point, near_bottom_left_point);
                Edge near_right_edge = new Edge(near_top_right_point, near_bottom_right_point);

                /*
                Draw_Edge(near_top_left_edge, camera_type);
                Draw_Edge(near_top_right_edge, camera_type);
                Draw_Edge(near_bottom_left_edge, camera_type);
                Draw_Edge(near_bottom_right_edge, camera_type);
                Draw_Edge(near_top_edge, camera_type);
                Draw_Edge(near_bottom_edge, camera_type);
                Draw_Edge(near_left_edge, camera_type);
                Draw_Edge(near_right_edge, camera_type);
                */
            }
            if (camera.Draw_Entire_View)
            {
                double semi_width_ratio = semi_width * ratio, semi_height_ratio = semi_height * ratio;

                Vector4D far_top_left_point = origin + new Vector4D(-semi_width_ratio, semi_height_ratio, camera.Z_Far);
                Vector4D far_top_right_point = origin + new Vector4D(semi_width_ratio, semi_height_ratio, camera.Z_Far);
                Vector4D far_bottom_left_point = origin + new Vector4D(-semi_width_ratio, -semi_height_ratio, camera.Z_Far);
                Vector4D far_bottom_right_point = origin + new Vector4D(semi_width_ratio, -semi_height_ratio, camera.Z_Far);

                Edge far_top_left_edge = new Edge(near_top_left_point, far_top_left_point);
                Edge far_top_right_edge = new Edge(near_top_right_point, far_top_right_point);
                Edge far_bottom_left_edge = new Edge(near_bottom_left_point, far_bottom_left_point);
                Edge far_bottom_right_edge = new Edge(near_bottom_right_point, far_bottom_right_point);
                Edge far_top_edge = new Edge(far_top_left_point, far_top_right_point);
                Edge far_bottom_edge = new Edge(far_bottom_left_point, far_bottom_right_point);
                Edge far_left_edge = new Edge(far_top_left_point, far_bottom_left_point);
                Edge far_right_edge = new Edge(far_top_right_point, far_bottom_right_point);
                /*
                Draw_Edge(far_top_left_edge, camera_type);
                Draw_Edge(far_top_right_edge, camera_type);
                Draw_Edge(far_bottom_left_edge, camera_type);
                Draw_Edge(far_bottom_right_edge, camera_type);
                Draw_Edge(far_top_edge, camera_type);
                Draw_Edge(far_bottom_edge, camera_type);
                Draw_Edge(far_left_edge, camera_type);
                Draw_Edge(far_right_edge, camera_type);*/
            }
        }
    }
}