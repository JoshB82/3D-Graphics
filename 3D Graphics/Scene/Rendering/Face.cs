using System.Collections.Generic;

namespace _3D_Graphics
{
    public sealed partial class Scene
    {
        public void Draw_Face(Face face, string shape_type)
        {
            Vector3D camera_to_face = new Vector3D(face.World_P1 - Render_Camera.World_Origin);
            Vector3D normal = Vector3D.Normal_From_Plane(new Vector3D(face.World_P1), new Vector3D(face.World_P2), new Vector3D(face.World_P3));

            // Discard face if its not visible
            if (camera_to_face * normal >= 0 && shape_type != "Plane") return;

            // Draw outline if needed
            if (face.Draw_Outline)
            {
                Draw_Edge(new Edge(face.World_P1, face.World_P2));
                Draw_Edge(new Edge(face.World_P1, face.World_P3));
                Draw_Edge(new Edge(face.World_P2, face.World_P3));
            }

            /*
            // Adjust colour based on lighting
            Color face_colour = face.Colour;
            if (Light_List.Count > 0)
            {
                double max_intensity = 0, true_intensity = 0;
                foreach (Light light in Light_List) max_intensity += light.Intensity;
                foreach (Light light in Light_List)
                {
                    switch (light.GetType().Name)
                    {
                        case "Distant_Light":
                            true_intensity = Math.Max(0, -light.World_Direction * normal) * light.Intensity;
                            break;
                        case "Point_Light":
                            true_intensity = Math.Max(0, -new Vector3D(point_1 - light.World_Origin).Normalise() * normal) * light.Intensity;
                            break;
                        case "Spot_Light":
                            Vector3D light_to_shape = new Vector3D(point_1 - light.World_Origin);
                            if (light_to_shape.Angle(light.World_Direction) > ((Spotlight)light).Angle || light_to_shape * light.World_Direction > ((Spotlight)light).Distance) continue;
                            true_intensity = Math.Max(0, -light.World_Direction * normal) * light.Intensity;
                            break;
                        case "Ambient_Light":
                            break;
                    }
                    double scaled_intensity = true_intensity / max_intensity;

                    if (face.Texture == null)
                    {

                    }

                    byte new_red = (byte)Round_To_Int((face.Colour.R + light.Colour.R) * 255 / 510 * scaled_intensity);
                    byte new_green = (byte)Round_To_Int((face.Colour.G + light.Colour.G) * 255 / 510 * scaled_intensity);
                    byte new_blue = (byte)Round_To_Int((face.Colour.B + light.Colour.B) * 255 / 510 * scaled_intensity);

                    face_colour = Color.FromArgb(face.Colour.A, new_red, new_green, new_blue);
                }
            }
            */

            // Create a clipping queue and add the first triangle
            Queue<Face> world_face_clip = new Queue<Face>();
            world_face_clip.Enqueue(face);
            int no_triangles = 1;

            //OUT?
            // Clip face against each world clipping plane
            foreach (Clipping_Plane world_clipping_plane in Render_Camera.World_Clipping_Planes)
            {
                while (no_triangles > 0)
                {
                    Face triangle = world_face_clip.Dequeue();
                    Face[] triangles = new Face[2];
                    int num_intersection_points = Clip_Face(world_clipping_plane.Point, world_clipping_plane.Normal, triangle, out triangles[0], out triangles[1]);
                    for (int i = 0; i < num_intersection_points; i++) world_face_clip.Enqueue(triangles[i]);
                    no_triangles--;
                }
                no_triangles = world_face_clip.Count;
            }

            // Discard face if its been fully clipped
            if (no_triangles == 0) return;

            Face[] projection_face_clip_array = world_face_clip.ToArray();

            // Move remaining faces into projection space
            // Not entirely sure why can't use foreach loop :/
            for (int i = 0; i < projection_face_clip_array.Length; i++)
            {
                projection_face_clip_array[i].World_P1 = Render_Camera.World_to_screen * projection_face_clip_array[i].World_P1;
                projection_face_clip_array[i].World_P2 = Render_Camera.World_to_screen * projection_face_clip_array[i].World_P2;
                projection_face_clip_array[i].World_P3 = Render_Camera.World_to_screen * projection_face_clip_array[i].World_P3;

                projection_face_clip_array[i].World_P1 /= projection_face_clip_array[i].World_P1.W;
                projection_face_clip_array[i].World_P2 /= projection_face_clip_array[i].World_P2.W;
                projection_face_clip_array[i].World_P3 /= projection_face_clip_array[i].World_P3.W;

                /*
                if (face.Texture != null)
                {
                    projection_clipped_face.T1 /= projection_clipped_face.World_P1.W;
                    projection_clipped_face.T2 /= projection_clipped_face.World_P1.W;
                    projection_clipped_face.T3 /= projection_clipped_face.World_P1.W;
                }*/
            }

            // Create another clipping queue and add the first triangle
            Queue<Face> projection_face_clip = new Queue<Face>(projection_face_clip_array);

            // Clip face against each projection clipping plane
            foreach (Clipping_Plane projection_clipping_plane in projection_clipping_planes)
            {
                while (no_triangles > 0)
                {
                    Face triangle = projection_face_clip.Dequeue();
                    Face[] triangles = new Face[2];
                    int num_intersection_points = Clip_Face(projection_clipping_plane.Point, projection_clipping_plane.Normal, triangle, out triangles[0], out triangles[1]);
                    for (int i = 0; i < num_intersection_points; i++) projection_face_clip.Enqueue(triangles[i]);
                    no_triangles--;
                }
                no_triangles = projection_face_clip.Count;
            }

            foreach (Face projection_clipped_face in projection_face_clip)
            {
                Vector4D result_point_1 = Scale_to_screen(projection_clipped_face.World_P1);
                Vector4D result_point_2 = Scale_to_screen(projection_clipped_face.World_P2);
                Vector4D result_point_3 = Scale_to_screen(projection_clipped_face.World_P3);

                // More variable simplification
                int result_point_1_x = Round_To_Int(result_point_1.X);
                int result_point_1_y = Round_To_Int(result_point_1.Y);
                double result_point_1_z = result_point_1.Z;
                int result_point_2_x = Round_To_Int(result_point_2.X);
                int result_point_2_y = Round_To_Int(result_point_2.Y);
                double result_point_2_z = result_point_2.Z;
                int result_point_3_x = Round_To_Int(result_point_3.X);
                int result_point_3_y = Round_To_Int(result_point_3.Y);
                double result_point_3_z = result_point_3.Z;

                // Finally draw the triangle
                if (face.Texture == null)
                {
                    Triangle(result_point_1_x, result_point_1_y, result_point_1_z, result_point_2_x, result_point_2_y, result_point_2_z, result_point_3_x, result_point_3_y, result_point_3_z, projection_clipped_face.Colour);
                }
                else
                {
                    // Scale the texture co-ordinates
                    int width = face.Texture.Width - 1;
                    int height = face.Texture.Height - 1;

                    // AFTERWARDS?
                    int result_texture_point_1_x = Round_To_Int(face.T1.X * width);
                    int result_texture_point_1_y = Round_To_Int(face.T1.Y * height);
                    int result_texture_point_2_x = Round_To_Int(face.T2.X * width);
                    int result_texture_point_2_y = Round_To_Int(face.T2.Y * height);
                    int result_texture_point_3_x = Round_To_Int(face.T3.X * width);
                    int result_texture_point_3_y = Round_To_Int(face.T3.Y * height);

                    Textured_Triangle(result_point_1_x, result_point_1_y, result_point_1_z, result_point_2_x, result_point_2_y, result_point_2_z, result_point_3_x, result_point_3_y, result_point_3_z, result_texture_point_1_x, result_texture_point_1_y, result_texture_point_2_x, result_texture_point_2_y, result_texture_point_3_x, result_texture_point_3_y, face.Texture);
                }
            }
            // RANGE TO DRAW X: [0,WIDTH-1] Y: [0,HEIGHT-1]
        }
    }
}