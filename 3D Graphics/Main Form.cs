using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace _3D_Graphics
{
    public partial class MainForm : Form
    {
        private const double grav_acc = -9.81;
        private const double camera_pan = 0.0001;
        private const double camera_tilt = 0.000001;

        private const int max_frames_per_second = 60;
        private const int max_updates_per_second = 60;

        private Scene scene;

        private int camera_selected = 0;

        private List<Light> lights = new List<Light>();
        private int selected_light = 0;

        private long update_time;

        public MainForm()
        {
            // Create form
            InitializeComponent();

            // Create origin
            World_Point origin_mesh = new World_Point(Vector3D.Zero);
            Shape origin = new Shape(origin_mesh);

            // Create default camera
            Perspective_Camera default_camera = new Perspective_Camera(new Vector3D(0, 0, 100), origin_mesh, Vector3D.Unit_Y, Canvas_Box.Width / 10, Canvas_Box.Height / 10, 10, 750);

            // Create scene
            scene = new Scene(Canvas_Box, Canvas_Box.Width, Canvas_Box.Height, default_camera);

            scene.Add(origin);

            // Create textures
            Bitmap brick = new Bitmap(Properties.Resources.brick);
            Bitmap smiley = new Bitmap(Properties.Resources.smiley);

            // Create default meshes
            /*
            Cube cube_mesh = new Cube(new Vector3D(0, 0, 0), Vector3D.Unit_X, Vector3D.Unit_Y, 50) { Face_Colour = Color.Green };
            Shape cube = new Shape(cube_mesh) { Selected = true };
            scene.Add(cube);
            */

            Cuboid cuboid_mesh = new Cuboid(new Vector3D(100, 0, 100), Vector3D.Unit_X, Vector3D.Unit_Y, 30, 40, 90, smiley);
            Shape cuboid = new Shape(cuboid_mesh);
            scene.Add(cuboid);
            
            /*
            Plane plane_mesh = new Plane(new Vector3D(0, 0, -30), Vector3D.Unit_X, Vector3D.Unit_Y, 100, 100) { Face_Colour = Color.Aqua };
            Shape plane = new Shape(plane_mesh);
            scene.Add(plane);
            */

            // Create axes
            Line x_axis_mesh = new Line(new Vector3D(0, 0, 0), new Vector3D(250, 0, 0)) { Edge_Colour = Color.Red };
            Line y_axis_mesh = new Line(new Vector3D(0, 0, 0), new Vector3D(0, 250, 0)) { Edge_Colour = Color.Green };
            Line z_axis_mesh = new Line(new Vector3D(0, 0, 0), new Vector3D(0, 0, 250)) { Edge_Colour = Color.Blue };

            Shape x_axis = new Shape(x_axis_mesh);
            Shape y_axis = new Shape(y_axis_mesh);
            Shape z_axis = new Shape(z_axis_mesh);

            scene.Add(x_axis);
            scene.Add(y_axis);
            scene.Add(z_axis);

            Plane test_plane = new Plane(Vector3D.Zero, Vector3D.Unit_X, Vector3D.Unit_Y, 50, 50, smiley);
            Shape test_plane_shape = new Shape(test_plane);
            scene.Add(test_plane_shape);

            Perspective_Camera alternate_camera = new Perspective_Camera(new Vector3D(0, 0, -10), test_plane, Vector3D.Unit_Y, Canvas_Box.Width / 10, Canvas_Box.Height / 10, 10, 20) { Draw_Entire_View = false };
            //scene.Render_Camera = camera_1;
            scene.Add(alternate_camera);

            // Create lights
            //lights.Add(new Distant_Light(new Vector3D(300, 400, 500), cube_mesh, Color.Red, 1));
            //scene.Add(lights[0]);

            // Add object from file
            scene.Add("C:\\Users\\jbrya\\source\\repos\\3D Graphics\\3D Graphics\\Models\\teapot.obj");

            Thread graphics_thread = new Thread(Game_Loop);
            graphics_thread.Start();
            graphics_thread.IsBackground = true;
        }

        #region Graphics Thread
        private void Game_Loop()
        {
            bool game_running = true;

            long start_time = Get_UNIX_Time_Milliseconds();
            long timer = Get_UNIX_Time_Milliseconds();
            long frame_delta_time = 0, update_delta_time = 0, now_time;

            const long frame_optimal_time = 1000 / max_frames_per_second;
            const long update_optimal_time = 1000 / max_updates_per_second;

            int no_frames = 0, no_updates = 0;

            // Possible error with this code
            while (game_running)
            {
                now_time = Get_UNIX_Time_Milliseconds();
                frame_delta_time += (now_time - start_time);
                update_delta_time += (now_time - start_time);
                update_time = update_delta_time;
                start_time = now_time;

                if (frame_delta_time >= frame_optimal_time)
                {
                    // Update objects????vv
                    scene.Render();
                    no_frames++;
                }

                if (update_delta_time >= update_optimal_time)
                {
                    // Render
                    // ApplyImpulse(update_delta_time);
                    no_updates++;
                }

                if (now_time - timer >= 1000)
                {
                    // ??
                    this.Invoke((MethodInvoker)delegate { this.Text = $"3D Graphics - FPS: {no_frames}, UPS: {no_updates}"; });
                    no_frames = 0; no_updates = 0;
                    timer += 1000;
                }

                // User input
            }
        }

        private static long Get_UNIX_Time_Milliseconds() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        #endregion

        private void Canvas_Panel_Paint(object sender, PaintEventArgs e) => e.Graphics.DrawImageUnscaled(scene.Canvas, Point.Empty);

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    // Pan forward
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction * camera_pan * update_time);
                    break;
                case Keys.A:
                    // Pan left
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction_Right * -camera_pan * update_time);
                    break;
                case Keys.D:
                    // Pan right
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction_Right * camera_pan * update_time);
                    break;
                case Keys.S:
                    // Pan back
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction * -camera_pan * update_time);
                    break;
                case Keys.Q:
                    // Pan up
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction_Up * camera_pan * update_time);
                    break;
                case Keys.E:
                    // Pan down
                    scene.Render_Camera.Translate(scene.Render_Camera.World_Direction_Up * -camera_pan * update_time);
                    break;
                case Keys.I:
                    // Rotate up
                    Matrix4x4 transformation_up = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction_Right, camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_1(new Vector3D(transformation_up * new Vector4D(scene.Render_Camera.World_Direction)), new Vector3D(transformation_up * new Vector4D(scene.Render_Camera.World_Direction_Up)));
                    break;
                case Keys.J:
                    // Rotate left
                    Matrix4x4 transformation_left = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction_Up, camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_3(new Vector3D(transformation_left * new Vector4D(scene.Render_Camera.World_Direction_Right)), new Vector3D(transformation_left * new Vector4D(scene.Render_Camera.World_Direction)));
                    break;
                case Keys.L:
                    // Rotate right
                    Matrix4x4 transformation_right = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction_Up, -camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_3(new Vector3D(transformation_right * new Vector4D(scene.Render_Camera.World_Direction_Right)), new Vector3D(transformation_right * new Vector4D(scene.Render_Camera.World_Direction)));
                    break;
                case Keys.K:
                    // Rotate down
                    Matrix4x4 transformation_down = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction_Right, -camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_1(new Vector3D(transformation_down * new Vector4D(scene.Render_Camera.World_Direction)), new Vector3D(transformation_down * new Vector4D(scene.Render_Camera.World_Direction_Up)));
                    break;
                case Keys.U:
                    // Roll left
                    Matrix4x4 transformation_roll_left = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction, -camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_2(new Vector3D(transformation_roll_left * new Vector4D(scene.Render_Camera.World_Direction_Up)), new Vector3D(transformation_roll_left * new Vector4D(scene.Render_Camera.World_Direction_Right)));
                    break;
                case Keys.O:
                    // Roll right
                    Matrix4x4 transformation_roll_right = Transform.Quaternion_Rotation_Axis_Matrix(scene.Render_Camera.World_Direction, camera_tilt * update_time);
                    scene.Render_Camera.Set_Camera_Direction_2(new Vector3D(transformation_roll_right * new Vector4D(scene.Render_Camera.World_Direction_Up)), new Vector3D(transformation_roll_right * new Vector4D(scene.Render_Camera.World_Direction_Right)));
                    break;
            }
        }

        private void switchCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            camera_selected++;
            if (camera_selected > scene.Camera_List.Count - 1) camera_selected = 0;
            scene.Render_Camera = scene.Camera_List[camera_selected];
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (scene != null)
            {
                scene.Render_Camera.Width = Canvas_Box.Width / 10;
                scene.Render_Camera.Height = Canvas_Box.Height / 10;
                scene.Width = Canvas_Box.Width;
                scene.Height = Canvas_Box.Height;
            }
        }

        private void changeLightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            lights[selected_light].Colour = Color.FromArgb(255, random.Next(256), random.Next(256), random.Next(256));

            lights[selected_light].Colour = Color.White;
        }
    }
}