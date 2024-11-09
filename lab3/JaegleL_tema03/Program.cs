using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTK_immediate_mode
{
    class ImmediateMode : GameWindow
    {
        private float red = 1.0f, green = 1.0f, blue = 1.0f;
        private float alpha = 1.0f;
        private float rotationX = 0f, rotationY = 0f; 
        private bool menuVisible = false;  
        private Point previousMousePosition;
        private float cameraDistance = 60.0f; 
        private Vector3[] triangleVertices; 
        private float targetRed = 1.0f, targetGreen = 1.0f, targetBlue = 1.0f;  
        private float colorSpeed = 0.05f;

        public ImmediateMode() : base(800, 600, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.On;

            Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
            Title = "OpenGL Version: " + GL.GetString(StringName.Version) + " (Immediate Mode)";
            CenterWindow();

            triangleVertices = LoadTriangleVertices("triangle.txt");
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.Lavender);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            previousMousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);

            DisplayMenu(); 
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\n   ALEGETI OPTIUNEA");
            Console.WriteLine(" R - Rosu");
            Console.WriteLine(" G - Verde");
            Console.WriteLine(" B - Albastru");
            Console.WriteLine(" D - Mareste transparenta");
            Console.WriteLine(" A - Micsoreaza transparenta ");
            Console.WriteLine(" X - Resetează RGBA");
            Console.WriteLine(" V - Valorile RGBA");
            Console.WriteLine(" M - Afisare meniu");
            Console.WriteLine(" ESC - Iesire");
            Console.WriteLine(" CLICK(stanga) - Rotire");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            double aspectRatio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspectRatio, 1.0f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            Matrix4 lookat = Matrix4.LookAt(0, 0, cameraDistance, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard[Key.Escape]) Exit();
            MouseState mouse = Mouse.GetState();

            int deltaX = mouse.X - previousMousePosition.X;
            int deltaY = mouse.Y - previousMousePosition.Y;

            if (keyboard[Key.S]) cameraDistance -= 5.0f;  
            if (keyboard[Key.W]) cameraDistance += 5.0f;  

            if (mouse.IsButtonDown(MouseButton.Left))
            {
                rotationY += deltaX * 0.2f; 
                rotationX += deltaY * 0.2f; 
            }

            previousMousePosition = new Point(mouse.X, mouse.Y);

            if (keyboard[Key.A] && alpha < 1.0f) alpha += 0.05f;
            if (keyboard[Key.D] && alpha > 0.0f) alpha -= 0.05f; 
            
            if (keyboard[Key.R])
            {
                targetRed = 1.0f;  
                targetGreen = 0.0f;
                targetBlue = 0.0f;
            }
            if (keyboard[Key.G])
            {
                targetRed = 0.0f;
                targetGreen = 1.0f;  
                targetBlue = 0.0f;
            }
            if (keyboard[Key.B])
            {
                targetRed = 0.0f;
                targetGreen = 0.0f;
                targetBlue = 1.0f;  
            }

            red = Lerp(red, targetRed, colorSpeed);
            green = Lerp(green, targetGreen, colorSpeed);
            blue = Lerp(blue, targetBlue, colorSpeed);


            if (keyboard[Key.X])
            {
                red = 1.0f;
                green = 1.0f;
                blue = 1.0f;
                Console.WriteLine("RGBA values reset to default (1.0, 1.0, 1.0, 1.0)");
            }

            if (keyboard[Key.V])
            {
                Console.WriteLine($"Current RGBA values: Red={red}, Green={green}, Blue={blue}");
            }

            if (keyboard[Key.M] && !menuVisible)
            {
                DisplayMenu(); 
                menuVisible = true; 
            }

            if (!keyboard[Key.M] && menuVisible)  
            {
                menuVisible = false;
            }
        }
        private float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0, 0, -30);
            UpdateCamera();

            DrawObjects();
            SwapBuffers();
        }

        private void DrawTriangle()
        {
            GL.PushMatrix();  
            GL.Translate(0, 0, 0);  
            GL.Rotate(rotationY, 0.0f, 1.0f, 0.0f);  
            GL.Rotate(rotationX, 1.0f, 0.0f, 0.0f);  

            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(red, green, blue, alpha);
            GL.Vertex3(triangleVertices[0]);

            GL.Color4(red, green, blue, alpha);
            GL.Vertex3(triangleVertices[1]);

            GL.Color4(red, green, blue, alpha);
            GL.Vertex3(triangleVertices[2]);

            GL.End();
            GL.PopMatrix();  
        }

        private void DrawObjects()
        {
            DrawTriangle();
        }

        private Vector3[] LoadTriangleVertices(string fileName)
        {
            Vector3[] vertices = new Vector3[3];

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] values = line.Split(',');
                            float x = float.Parse(values[0]);
                            float y = float.Parse(values[1]);
                            float z = float.Parse(values[2]);
                            vertices[i] = new Vector3(x, y, z);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
                return null;
            }

            return vertices;
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (ImmediateMode example = new ImmediateMode())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
