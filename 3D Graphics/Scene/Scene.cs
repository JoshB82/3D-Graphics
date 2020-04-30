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

        public readonly List<Camera> Camera_List = new List<Camera>();
        public readonly List<Light> Light_List = new List<Light>();
        public readonly List<Shape> Shape_List = new List<Shape>();

        private Clipping_Plane[] world_clipping_planes;
        private Clipping_Plane[] projection_clipping_planes;

        private Camera render_camera;
        public Camera Render_Camera
        {
            get => render_camera;
            set
            {
                render_camera = value;
                world_clipping_planes = render_camera.Calculate_Clipping_Planes();
            }
        }

        public PictureBox Canvas_box { get; set; }
        public Bitmap Canvas { get; private set; }
        
        public Color Background_colour { get; set; }

        // Buffers
        private double[][] z_buffer;
        private Color[][] colour_buffer;

        public bool Change_scene { get; set; } = true;

        #region Dimensions
        private int width, height;
        public int Width
        {
            get => width;
            set { lock (locker) { width = value; Set_Buffer(); } }
        }
        public int Height
        {
            get => height;
            set { lock (locker) { height = value; Set_Buffer(); } }
        }

        private void Set_Buffer()
        {
            z_buffer = new double[width][];
            colour_buffer = new Color[width][];
            for (int i = 0; i < width; i++) z_buffer[i] = new double[height];
            for (int i = 0; i < width; i++) colour_buffer[i] = new Color[height];
        }
        #endregion

        /// <summary>
        /// Create a new scene.
        /// </summary>
        /// <param name="canvas_box">The Picture box where the scene will be rendered.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="render_camera"></param>
        public Scene(PictureBox canvas_box, int width, int height, Camera render_camera)
        {
            Canvas_box = canvas_box;
            Width = width;
            Height = height;
            Render_Camera = render_camera;
            Camera_List.Add(render_camera);

            Background_colour = Color.White;
            Canvas = new Bitmap(width, height);

            Vector3D near_bottom_left_point = new Vector3D(-1, -1, -1), far_top_right_point = new Vector3D(1, 1, 1);
            projection_clipping_planes = new Clipping_Plane[]
            {
                new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_X), // Left
                new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_Y), // Bottom
                new Clipping_Plane(near_bottom_left_point, Vector3D.Unit_Z), // Near
                new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_X), // Right
                new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_Y), // Top
                new Clipping_Plane(far_top_right_point, Vector3D.Unit_Negative_Z) // Far
            };
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
                case "Orthogonal_Camera":
                case "Perspective_Camera":
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
                case "Orthogonal_Camera[]":
                case "Perspective_Camera[]":
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
        public bool Add(string file_path)
        {
            if (File.Exists(file_path))
            {
                try
                {
                    List<Vector4D> vertices = new List<Vector4D>();
                    List<Edge> edges = new List<Edge>();
                    List<Face> faces = new List<Face>();
                    List<Vector3D> texture_vertices = new List<Vector3D>();

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
                                vertices.Add(new Vector4D(x, y, z, w));
                                break;
                            case "vt":
                                // Texture vertex
                                u = Double.Parse(data[1]);
                                v = (data.Length > 2) ? Double.Parse(data[2]) : 0;
                                w = (data.Length == 4) ? Double.Parse(data[3]) : 0;
                                texture_vertices.Add(new Vector3D(u, v, w));
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

                    Add(new Shape(new Custom(new Vector3D(vertices[0]), Vector3D.Unit_X, Vector3D.Unit_Y, vertices.ToArray(), null, edges.ToArray(), faces.ToArray())));
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
        #endregion

        public void Remove(int ID)
        {
            lock (locker) Shape_List.RemoveAll(x => x.ID == ID);
        }

        public void Render()
        {
            if (Canvas_box == null) throw new Exception("No picture box has been set yet!");
            if (render_camera == null) throw new Exception("No camera has been set yet!");

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
                render_camera.Calculate_Model_to_World_Matrix();
                render_camera.Apply_World_Matrix();
                render_camera.Calculate_World_to_Screen_Matrix();
                
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
                                if (face.Visible) Draw_Face(face, shape.Render_Mesh.GetType().Name);
                            }
                        }

                        // Draw edges
                        if (shape.Render_Mesh.Edges != null && shape.Render_Mesh.Draw_Edges)
                        {
                            foreach (Edge edge in shape.Render_Mesh.Edges) if (edge.Visible) Draw_Edge(edge);
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

                    foreach (Camera camera_to_draw in Camera_List) Draw_Camera(camera_to_draw);

                    // Draw each pixel from colour buffer
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            using (SolidBrush face_brush = new SolidBrush(colour_buffer[x][y])) g.FillRectangle(face_brush, x, y * -1 + Height - 1, 1, 1);
                        }
                    }
                }

                Canvas = temp_canvas;
                Canvas_box.Invalidate();
                Change_scene = false;
            }
        }

        public Vector4D Scale_to_screen(Vector4D vertex) => Transform.Scale(0.5 * (Width - 1), 0.5 * (Height - 1), 1) * Transform.Translate(new Vector3D(1, 1, 0)) * vertex;
    }
}