using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace JaegleL_tema02
{
    class SimpleWindow3D : GameWindow
    {
        private const int SIZE = 75;
        private Vector3 trianglePosition = new Vector3(0.0f, 0.0f, 0.0f);
        private float triangleRotationY = 0.0f;
        private float triangleRotationX = 0.0f;
        private Point previousMousePosition;
        private float cameraDistance = 250.0f;  // Camera is closer to the object now

        public SimpleWindow3D() : base(800, 600, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.On;
            Console.WriteLine("OpenGl versiunea: " + GL.GetString(StringName.Version));
            Title = "OpenGl versiunea: " + GL.GetString(StringName.Version) + " (mod imediat)";
            CenterWindow();
        }

        private void CenterWindow()
        {
            int screenWidth = DisplayDevice.Default.Width;
            int screenHeight = DisplayDevice.Default.Height;
            var bounds = this.Bounds;
            int centeredX = (screenWidth - Width) / 2;
            int centeredY = (screenHeight - bounds.Height) / 2 - 10;
            Location = new Point(centeredX, centeredY);
        }

        // OpenGL setup and loading resources
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.CornflowerBlue);  // Changed background color to differentiate
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            previousMousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);
        }

        // Handle window resizing
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            UpdateCamera();
        }

        // Update the camera position
        private void UpdateCamera()
        {
            // Update the camera position: move it back to make sure the object is in view
            Matrix4 lookat = Matrix4.LookAt(0, 0, cameraDistance, 0, 0, 0, 0, 1, 0);  // Camera positioned correctly
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
        }

        // Handle user input (keyboard, mouse)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            int deltaX = mouse.X - previousMousePosition.X;
            int deltaY = mouse.Y - previousMousePosition.Y;

            if (keyboard[Key.Escape])
            {
                Exit();
            }

            // Zoom functionality: W (zoom in), S (zoom out)
            if (keyboard[Key.S]) cameraDistance -= 5.0f;  // Zoom out
            if (keyboard[Key.W]) cameraDistance += 5.0f;  // Zoom in

            // Rotation using mouse when left button is pressed
            if (mouse.IsButtonDown(MouseButton.Left))
            {
                triangleRotationY += deltaX * 0.2f;
                triangleRotationX += deltaY * 0.2f;
            }

            previousMousePosition = new Point(mouse.X, mouse.Y);
        }

        // Render the scene (draw the triangle)
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Update camera position based on zoom
            UpdateCamera();

            // Draw the triangle
            DrawTriangle();

            // Swap buffers (double buffering)
            SwapBuffers();
        }

        // Drawing the triangle with faces and colors
        private void DrawTriangle()
        {
            GL.PushMatrix();  // Save the current matrix state
            GL.Translate(trianglePosition);  // Move triangle to its position
            GL.Rotate(triangleRotationY, 0.0f, 1.0f, 0.0f);
            GL.Rotate(triangleRotationX, 1.0f, 0.0f, 0.0f);

            GL.Begin(PrimitiveType.Triangles);

            // Set colors for each vertex of the triangle
            GL.Color3(Color.Red);
            GL.Vertex3(-SIZE, 0.0f, SIZE);

            GL.Color3(Color.Green);
            GL.Vertex3(SIZE, 0.0f, SIZE);

            GL.Color3(Color.Blue);
            GL.Vertex3(0.0f, SIZE, 0.0f);

            GL.End();

            GL.PopMatrix();  // Restore the original matrix state
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (SimpleWindow3D example = new SimpleWindow3D())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
