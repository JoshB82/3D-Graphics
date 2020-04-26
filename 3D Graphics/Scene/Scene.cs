using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace _3D_Graphics
{
    public abstract class Scene_Object { } // Used to group scene objects together.

    public sealed partial class Scene
    {
        private static readonly object locker = new object();

        public bool Change_scene { get; set; } = true;

        public readonly List<Camera> Camera_List = new List<Camera>();
        public readonly List<Light> Light_List = new List<Light>();
        public readonly List<Shape> Shape_List = new List<Shape>();

        public Bitmap Canvas { get; private set; }
        public Color Background_colour { get; set; }

        // Buffers
        private double[][] z_buffer;
        private Color[][] colour_buffer;

        // Scene dimensions
        private int width, height;
        public int Width
        {
            get { return width; }
            set { lock (locker) { width = value; Set_Buffer(); } }
        }
        public int Height
        {
            get { return height; }
            set { lock (locker) { height = value; Set_Buffer(); } }
        }

        public Scene(int width, int height, Color? background_colour = null)
        {
            Width = width;
            Height = height;
            Background_colour = background_colour ?? Color.White;

            Canvas = new Bitmap(width, height);
        }

        private void Set_Buffer()
        {
            z_buffer = new double[width][];
            colour_buffer = new Color[width][];
            for (int i = 0; i < width; i++) z_buffer[i] = new double[height];
            for (int i = 0; i < width; i++) colour_buffer[i] = new Color[height];
        }

        #region Add to scene methods

        /// <summary>
        /// Add an object to the scene.
        /// </summary>
        /// <param name="object">Object to add.</param>
        public void Add(Scene_Object @object)
        {
            switch (@object.GetType().Name)
            {
                case "Camera":
                    Camera_List.Add((Camera)@object);
                    break;
                case "Light":
                    Light_List.Add((Light)@object);
                    break;
                case "Shape":
                    Shape_List.Add((Shape)@object);
                    break;
            }
        }

        // Probably not working
        /// <summary>
        /// Add objects to the scene.
        /// </summary>
        /// <param name="objects">Array of objects to add.</param>
        public void Add(Scene_Object[] objects)
        {
            switch (objects.GetType().Name)
            {
                case "Camera[]":
                    foreach (object camera in objects) Camera_List.Add((Camera)camera);
                    break;
                case "Light[]":
                    foreach (object light in objects) Light_List.Add((Light)light);
                    break;
                case "Shape[]":
                    foreach (object shape in objects) Shape_List.Add((Shape)shape);
                    break;
            }
        }
        #endregion

        public bool Add_From_OBJ_File(string file_path)
        {
            if (File.Exists(file_path))
            {
                try
                {
                    List<Vertex> vertices = new List<Vertex>();
                    List<Edge> edges = new List<Edge>();
                    List<Face> faces = new List<Face>();
                    List<Texture_Vertex> texture_vertices = new List<Texture_Vertex>();

                    string[] lines = File.ReadAllLines(file_path);
                    foreach (string line in lines)
                    {
                        string[] data = line.Split();
                        int p1, p2, p3;
                        double x, y, z, u, v, w;

                        switch (data[0])
                        {
                            case "#":
                                // Comment; ignore line
                                break;
                            case "v":
                                // Vertex
                                x = Double.Parse(data[1]);
                                y = Double.Parse(data[2]);
                                z = Double.Parse(data[3]);
                                w = (data.Length == 5) ? Double.Parse(data[4]) : 1;
                                vertices.Add(new Vertex(x, y, z, w));
                                break;
                            case "vt":
                                // Texture vertex
                                u = Double.Parse(data[1]);
                                v = (data.Length > 2) ? Double.Parse(data[2]) : 0;
                                w = (data.Length == 4) ? Double.Parse(data[3]) : 0;
                                texture_vertices.Add(new Texture_Vertex(u, v, w));
                                break;
                            case "l":
                                // Line (or polyline)
                                int no_end_points = data.Length - 1;
                                do
                                {
                                    p1 = Int32.Parse(data[no_end_points]) - 1;
                                    p2 = Int32.Parse(data[no_end_points - 1]) - 1;
                                    edges.Add(new Edge(vertices[p1 - 1], vertices[p2 - 1], Color.Black));
                                    no_end_points--;
                                }
                                while (no_end_points > 1);
                                break;
                            case "f":
                                // Face
                                p1 = Int32.Parse(data[1]) - 1;
                                p2 = Int32.Parse(data[2]) - 1;
                                p3 = Int32.Parse(data[3]) - 1;
                                Random rand = new Random();
                                faces.Add(new Face(vertices[p1 - 1], vertices[p2 - 1], vertices[p3 - 1], Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                                break;
                        }
                    }

                    Add(new Shape(new Custom(new Vector3D(vertices[0]), Vector3D.Unit_X, Vector3D.Unit_Y, vertices.ToArray(), edges.ToArray(), faces.ToArray())));
                    return true;
                }
                catch (Exception error)
                {
                    Debug.WriteLine($"Error: {error.Message}");
                    return false;
                }
            }
            else
            {
                Debug.WriteLine($"{file_path} not found.");
                return false;
            }
        }

        public void Remove(int ID)
        {
            lock (locker) Shape_List.RemoveAll(x => x.ID == ID);
        }

        private static bool Clip_Line(Vector3D plane_point, Vector3D plane_normal, Edge e)
        {
            Vector3D point_1 = new Vector3D(e.P1), point_2 = new Vector3D(e.P2);
            double point_1_distance = Vector3D.Point_Distance_From_Plane(point_1, plane_point, plane_normal);
            double point_2_distance = Vector3D.Point_Distance_From_Plane(point_2, plane_point, plane_normal);

            if (point_1_distance >= 0 && point_2_distance >= 0)
            {
                // Both points are on the inside, so return line unchanged
                return true;
            }
            if (point_1_distance >= 0 && point_2_distance < 0)
            {
                // One point is on the inside, the other on the outside, so clip the line
                Vector4D intersection = new Vector4D(Vector3D.Line_Intersect_Plane(point_1, point_2, plane_point, plane_normal, out double d));
                e.P1 = e.P1;
                e.P2 = intersection;
                return true;
            }
            if (point_1_distance < 0 && point_2_distance >= 0)
            {
                // One point is on the outside, the other on the inside, so clip the line
                Vector4D intersection = new Vector4D(Vector3D.Line_Intersect_Plane(point_2, point_1, plane_point, plane_normal, out double d));
                e.P1 = e.P2;
                e.P2 = intersection;
                return true;
            }
            // Both points are on the outside, so discard the line
            return false;
        }

        private static int Clip_Face(Vector3D plane_point, Vector3D plane_normal, Face f, out Face f1, out Face f2)
        {
            f1 = null; f2 = null;
            Vector3D point_1 = new Vector3D(f.P1), point_2 = new Vector3D(f.P2), point_3 = new Vector3D(f.P3);
            int inside_point_count = 0;
            List<Vector3D> inside_points = new List<Vector3D>(3);
            List<Vector3D> outside_points = new List<Vector3D>(3);
            List<Vector3D> inside_texture_points = new List<Vector3D>(3);
            List<Vector3D> outside_texture_points = new List<Vector3D>(3);

            if (Vector3D.Point_Distance_From_Plane(point_1, plane_point, plane_normal) >= 0)
            {
                inside_point_count++;
                inside_points.Add(point_1);
                inside_texture_points.Add(f.T1);
            }
            else
            {
                outside_points.Add(point_1);
                outside_texture_points.Add(f.T1);
            }

            if (Vector3D.Point_Distance_From_Plane(point_2, plane_point, plane_normal) >= 0)
            {
                inside_point_count++;
                inside_points.Add(point_2);
                inside_texture_points.Add(f.T2);
            }
            else
            {
                outside_points.Add(point_2);
                outside_texture_points.Add(f.T2);
            }

            if (Vector3D.Point_Distance_From_Plane(point_3, plane_point, plane_normal) >= 0)
            {
                inside_point_count++;
                inside_points.Add(point_3);
                inside_texture_points.Add(f.T3);
            }
            else
            {
                outside_points.Add(point_3);
                outside_texture_points.Add(f.T3);
            }

            Vector4D first_intersection, second_intersection;
            double d;

            switch (inside_point_count)
            {
                case 0:
                    // All points are on the outside, so no valid triangles to return
                    return 0;
                case 1:
                    // One point is on the inside, so only a smaller triangle is needed
                    first_intersection = new Vector4D(Vector3D.Line_Intersect_Plane(inside_points[0], outside_points[0], plane_point, plane_normal, out d));
                    second_intersection = new Vector4D(Vector3D.Line_Intersect_Plane(inside_points[0], outside_points[1], plane_point, plane_normal, out d));
                    if (f.Texture == null)
                    {
                        f1 = new Face(new Vector4D(inside_points[0]), first_intersection, second_intersection, f.Colour);
                    }
                    else
                    {
                        Vector3D t_intersection_1 = (outside_texture_points[0] - inside_texture_points[0]) * d + inside_texture_points[0];
                        Vector3D t_intersection_2 = (outside_texture_points[1] - inside_texture_points[0]) * d + inside_texture_points[0];
                        f1 = new Face(new Vector4D(inside_points[0]), first_intersection, second_intersection, inside_texture_points[0], t_intersection_1, t_intersection_2, f.Texture);
                    }
                    return 1;
                case 2:
                    // Two points are on the inside, so a quadrilateral is formed and split into two triangles
                    first_intersection = new Vector4D(Vector3D.Line_Intersect_Plane(inside_points[0], outside_points[0], plane_point, plane_normal, out d));
                    second_intersection = new Vector4D(Vector3D.Line_Intersect_Plane(inside_points[1], outside_points[0], plane_point, plane_normal, out d));
                    if (f.Texture == null)
                    {
                        f1 = new Face(new Vector4D(inside_points[0]), new Vector4D(inside_points[1]), first_intersection, f.Colour);
                        f2 = new Face(new Vector4D(inside_points[1]), second_intersection, first_intersection, f.Colour);
                    }
                    else
                    {
                        Vector3D t_intersection_1 = (outside_texture_points[0] - inside_texture_points[0]) * d + inside_texture_points[0];
                        Vector3D t_intersection_2 = (outside_texture_points[0] - inside_texture_points[1]) * d + inside_texture_points[1];
                        f1 = new Face(new Vector4D(inside_points[0]), new Vector4D(inside_points[1]), first_intersection, inside_texture_points[0], inside_texture_points[1], t_intersection_1, f.Texture);
                        f2 = new Face(new Vector4D(inside_points[1]), second_intersection, first_intersection, inside_texture_points[1], t_intersection_2, t_intersection_1, f.Texture);
                    }
                    return 2;
                case 3:
                    // All points are on the inside, so return the triangle unchanged
                    f1 = f;
                    return 1;
            }
            return 0;
        }

        public void Render(PictureBox canvas_box, Camera camera)
        {
            // Only render if a change in scene has taken place.
            // if (!Change_scene) return;

            lock (locker)
            {
                // Create temporary canvas for this frame
                Bitmap temp_canvas = new Bitmap(Width, Height);

                // Reset buffers
                for (int i = 0; i < Width; i++) for (int j = 0; j < Height; j++) z_buffer[i][j] = 2; // 2 is always greater than anything to be rendered.
                for (int i = 0; i < Width; i++) for (int j = 0; j < Height; j++) colour_buffer[i][j] = Background_colour;

                // Calculate camera matrices
                camera.Calculate_Model_to_World_Matrix();
                camera.Apply_World_Matrix();
                camera.Calculate_World_to_Screen_Matrix();

                // Clipping planes
                Clipping_Plane[] world_clipping_planes = camera.Calculate_Clipping_Planes();
                Vector3D near_bottom_left_point = new Vector3D(-1, -1, -1), far_top_right_point = new Vector3D(1, 1, 1);
                Clipping_Plane[] projection_clipping_planes = new Clipping_Plane[]
                {
                    new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_X), // Left
                    new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_Y), // Bottom
                    new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_Z), // Near
                    new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_X), // Right
                    new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_Y), // Top
                    new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_Z) // Far
                };

                // Draw graphics
                using (Graphics g = Graphics.FromImage(temp_canvas))
                {
                    foreach (Shape shape in Shape_List)
                    {
                        // Move shapes from model space to world space
                        shape.Render_Mesh.Calculate_Model_to_World_Matrix();
                        shape.Render_Mesh.Apply_World_Matrices();

                        // Draw faces
                        if (shape.Render_Mesh.Faces != null && shape.Render_Mesh.Draw_Faces)
                        {
                            foreach (Face face in shape.Render_Mesh.Faces)
                            {
                                if (face.Visible) Draw_Face(face, camera, world_clipping_planes, projection_clipping_planes, shape.Render_Mesh.GetType().Name);
                            }
                        }

                        // Draw edges
                        if (shape.Render_Mesh.Edges != null && shape.Render_Mesh.Draw_Edges)
                        {
                            foreach (Edge edge in shape.Render_Mesh.Edges)
                            {
                                if (edge.Visible) Draw_Edge(edge, camera, world_clipping_planes, projection_clipping_planes);
                            }
                        }

                        /*
                        // Draw vertices
                        if (shape.Render_Mesh.Camera_Vertices != null && shape.Render_Mesh.Draw_Vertices)
                        {
                            foreach (Vertex vertex in shape.Render_Mesh.Camera_Vertices)
                            {
                                if (vertex.Visible)
                                {
                                    // Variable simplification
                                    int point_x = (int)vertex.X;
                                    int point_y = (int)vertex.Y;
                                    double point_z = vertex.Z;

                                    for (int x = point_x - vertex.Diameter / 2; x <= point_x + vertex.Diameter / 2; x++)
                                    {
                                        for (int y = point_y - vertex.Diameter / 2; y <= point_y + vertex.Diameter / 2; y++)
                                        {
                                            // Check against z buffer
                                            if (z_buffer[x][y] > point_z)
                                            {
                                                z_buffer[x][y] = point_z;
                                                colour_buffer[x][y] = shape.Render_Mesh.Vertex_Colour;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        */
                    }

                    // Draw each pixel from colour buffer
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            using (SolidBrush face_brush = new SolidBrush(colour_buffer[x][y])) g.FillRectangle(face_brush, x, y, 1, 1);
                        }
                    }
                }

                Canvas = temp_canvas;
                canvas_box.Invalidate();
                Change_scene = false;
            }
        }

        public Vector4D Scale_to_screen(Vector4D vertex) => Transform.Scale(0.5 * (Width - 1), 0.5 * (Height - 1), 1) * Transform.Translate(new Vector3D(1, 1, 0)) * vertex;

        // Only do at drawing stage? v
        public Vector4D Change_Y_Axis(Vector4D vertex) => Transform.Translate(new Vector3D(0, Height - 1, 0)) * Transform.Scale_Y(-1) * vertex;

        public void Draw_Vertex(Vertex vertex)
        {

        }

        public void Draw_Edge(Edge edge, Camera camera, Clipping_Plane[] world_clipping_planes, Clipping_Plane[] projection_clipping_planes)
        {
            // Create a copy of the edge
            Vector4D point_1 = new Vector4D(edge.P1.X, edge.P1.Y, edge.P1.Z);
            Vector4D point_2 = new Vector4D(edge.P2.X, edge.P2.Y, edge.P2.Z);
            Edge clip_edge = new Edge(point_1, point_2, edge.Colour) { Visible = edge.Visible };

            // Clip the edge in world space
            foreach (Clipping_Plane world_clipping_plane in world_clipping_planes)
            {
                if (!Clip_Line(world_clipping_plane.Point, world_clipping_plane.Normal, clip_edge)) return;
            }

            clip_edge.P1 = camera.Apply_Camera_Matrices(clip_edge.P1);
            clip_edge.P2 = camera.Apply_Camera_Matrices(clip_edge.P2);

            clip_edge.P1 = camera.Divide_By_W(clip_edge.P1);
            clip_edge.P2 = camera.Divide_By_W(clip_edge.P2);

            foreach (Clipping_Plane projection_clipping_plane in projection_clipping_planes)
            {
                if (!Clip_Line(projection_clipping_plane.Point, projection_clipping_plane.Normal, clip_edge)) return;
            }

            Vector4D result_point_1 = Scale_to_screen(edge.P1);
            Vector4D result_point_2 = Scale_to_screen(edge.P2);

            result_point_1 = Change_Y_Axis(result_point_1);
            result_point_2 = Change_Y_Axis(result_point_2);

            int result_point_1_x = Round_To_Int(result_point_1.X);
            int result_point_1_y = Round_To_Int(result_point_1.Y);
            double result_point_1_z = result_point_1.Z;
            int result_point_2_x = Round_To_Int(result_point_2.X);
            int result_point_2_y = Round_To_Int(result_point_2.Y);
            double result_point_2_z = result_point_2.Z;

            // Finally draw the line
            Line(result_point_1_x, result_point_1_y, result_point_1_z, result_point_2_x, result_point_2_y, result_point_2_z, edge.Colour);
        }

        public void Draw_Face(Face face, Camera camera, Clipping_Plane[] world_clipping_planes, Clipping_Plane[] projection_clipping_planes, string shape_type)
        {
            // Create a copy of the face
            Vector4D point_1 = new Vector4D(face.P1.X, face.P1.Y, face.P1.Z);
            Vector4D point_2 = new Vector4D(face.P2.X, face.P2.Y, face.P2.Z);
            Vector4D point_3 = new Vector4D(face.P3.X, face.P3.Y, face.P3.Z);

            Vector3D texture_point_1 = null, texture_point_2 = null, texture_point_3 = null;
            Face clip_face;
            if (face.Texture == null)
            {
                clip_face = new Face(point_1, point_2, point_3, face.Colour) { Draw_Outline = face.Draw_Outline, Visible = face.Visible };
            }
            else
            {
                texture_point_1 = new Vector3D(face.T1.X, face.T1.Y, face.T1.Z);
                texture_point_2 = new Vector3D(face.T2.X, face.T2.Y, face.T2.Z);
                texture_point_3 = new Vector3D(face.T3.X, face.T3.Y, face.T3.Z);

                clip_face = new Face(point_1, point_2, point_3, texture_point_1, texture_point_2, texture_point_3, face.Texture) { Draw_Outline = face.Draw_Outline, Visible = face.Visible };
            }

            Vector3D normal = Vector3D.Normal_From_Plane(new Vector3D(point_1), new Vector3D(point_2), new Vector3D(point_3));
            Vector3D camera_to_face = new Vector3D(point_1 - camera.World_Origin);

            // Discard face if its not visible
            if (camera_to_face * normal >= 0 && shape_type != "Plane") return;

            // Draw outline if needed
            if (clip_face.Draw_Outline)
            {
                Draw_Edge(new Edge(clip_face.P1, clip_face.P2), camera, world_clipping_planes, projection_clipping_planes);
                Draw_Edge(new Edge(clip_face.P1, clip_face.P3), camera, world_clipping_planes, projection_clipping_planes);
                Draw_Edge(new Edge(clip_face.P2, clip_face.P3), camera, world_clipping_planes, projection_clipping_planes);
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

            // Create a queue
            Queue<Face> world_face_clip = new Queue<Face>();

            // Add initial triangle to clipping queue
            world_face_clip.Enqueue(clip_face);

            int no_triangles = 1;

            //OUT?
            // Clip face against each world clipping plane
            foreach (Clipping_Plane world_clipping_plane in world_clipping_planes)
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

            Queue<Face> projection_face_clip = world_face_clip;

            // Move remaining faces into projection space
            foreach (Face projection_clipped_face in projection_face_clip)
            {
                projection_clipped_face.P1 = camera.Apply_Camera_Matrices(projection_clipped_face.P1);
                projection_clipped_face.P2 = camera.Apply_Camera_Matrices(projection_clipped_face.P2);
                projection_clipped_face.P3 = camera.Apply_Camera_Matrices(projection_clipped_face.P3);

                projection_clipped_face.P1 = camera.Divide_By_W(projection_clipped_face.P1);
                projection_clipped_face.P2 = camera.Divide_By_W(projection_clipped_face.P2);
                projection_clipped_face.P3 = camera.Divide_By_W(projection_clipped_face.P3);

                /*
                if (face.Texture != null)
                {
                    projection_clipped_face.T1 /= projection_clipped_face.P1.W;
                    projection_clipped_face.T2 /= projection_clipped_face.P1.W;
                    projection_clipped_face.T3 /= projection_clipped_face.P1.W;
                }*/
            }

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
                Vector4D result_point_1 = Scale_to_screen(projection_clipped_face.P1);
                Vector4D result_point_2 = Scale_to_screen(projection_clipped_face.P2);
                Vector4D result_point_3 = Scale_to_screen(projection_clipped_face.P3);

                result_point_1 = Change_Y_Axis(result_point_1);
                result_point_2 = Change_Y_Axis(result_point_2);
                result_point_3 = Change_Y_Axis(result_point_3);

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
                    int result_texture_point_1_x = Round_To_Int(texture_point_1.X * width);
                    int result_texture_point_1_y = Round_To_Int(texture_point_1.Y * height);
                    int result_texture_point_2_x = Round_To_Int(texture_point_2.X * width);
                    int result_texture_point_2_y = Round_To_Int(texture_point_2.Y * height);
                    int result_texture_point_3_x = Round_To_Int(texture_point_3.X * width);
                    int result_texture_point_3_y = Round_To_Int(texture_point_3.Y * height);

                    Textured_Triangle(result_point_1_x, result_point_1_y, result_point_1_z, result_point_2_x, result_point_2_y, result_point_2_z, result_point_3_x, result_point_3_y, result_point_3_z, result_texture_point_1_x, result_texture_point_1_y, result_texture_point_2_x, result_texture_point_2_y, result_texture_point_3_x, result_texture_point_3_y, face.Texture);
                }
            }
            /*
                // between [-1,1]
                // between [0,2] (+1)
                // => between [0,1] (/2)
                // => between [0,width-1] (*(width-1))

                // RANGE TO DRAW X: [0,WIDTH-1] Y: [0,HEIGHT-1]
            */
        }
    }
}