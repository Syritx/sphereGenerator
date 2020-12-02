using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace _3d
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeWindowSettings nws = new NativeWindowSettings() {
                Title = "Sphere Generator",
                Size = new Vector2i(1000,1000),
                StartFocused = true,
                StartVisible = true,
                APIVersion = new Version(3,2),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core,
            };
            GameWindowSettings gws = new GameWindowSettings();
            new Game(gws,nws);
        }
    }

    class Game : GameWindow {

        Tile tile;
        Camera camera;

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        int mapResolution = 300, id = 0;
        float[] colors = {1,1,1};
        float planetSize = 2f;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) {
            
            for (int x = 0; x < mapResolution; x++) {
                for (int y = 0; y < mapResolution; y++) {

                    Vector2[] percent = { 
                        new Vector2(-1.5f+x,  1.5f+y)/(mapResolution-1),
                        new Vector2( 1.5f+x,  1.5f+y)/(mapResolution-1),
                        new Vector2( 1.5f+x, -1.5f+y)/(mapResolution-1),
                        new Vector2(-1.5f+x, -1.5f+y)/(mapResolution-1),
                    };

                    Vector3[] topFace = {
                        (new Vector3(0,1,0) + (percent[0].X-.5f) * 2 * new Vector3(1,0,0) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,1,0), new Vector3(1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,1,0) + (percent[1].X-.5f) * 2 * new Vector3(1,0,0) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,1,0), new Vector3(1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,1,0) + (percent[2].X-.5f) * 2 * new Vector3(1,0,0) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,1,0), new Vector3(1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,1,0) + (percent[3].X-.5f) * 2 * new Vector3(1,0,0) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,1,0), new Vector3(1,0,0))).Normalized()*planetSize
                    };
                    Vector3[] bottomFace = {
                        (new Vector3(0,-1,0) + (percent[0].X-.5f) * 2 * new Vector3(-1,0,0) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,-1,0), new Vector3(-1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,-1,0) + (percent[1].X-.5f) * 2 * new Vector3(-1,0,0) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,-1,0), new Vector3(-1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,-1,0) + (percent[2].X-.5f) * 2 * new Vector3(-1,0,0) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,-1,0), new Vector3(-1,0,0))).Normalized()*planetSize,
                        (new Vector3(0,-1,0) + (percent[3].X-.5f) * 2 * new Vector3(-1,0,0) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,-1,0), new Vector3(-1,0,0))).Normalized()*planetSize
                    };
                    Vector3[] leftFace = {
                        (new Vector3(1,0,0) + (percent[0].X-.5f) * 2 * new Vector3(0,0,1) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(1,0,0), new Vector3(0,0,1))).Normalized()*planetSize,
                        (new Vector3(1,0,0) + (percent[1].X-.5f) * 2 * new Vector3(0,0,1) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(1,0,0), new Vector3(0,0,1))).Normalized()*planetSize,
                        (new Vector3(1,0,0) + (percent[2].X-.5f) * 2 * new Vector3(0,0,1) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(1,0,0), new Vector3(0,0,1))).Normalized()*planetSize,
                        (new Vector3(1,0,0) + (percent[3].X-.5f) * 2 * new Vector3(0,0,1) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(1,0,0), new Vector3(0,0,1))).Normalized()*planetSize
                    };
                    Vector3[] rightFace = {
                        (new Vector3(-1,0,0) + (percent[0].X-.5f) * 2 * new Vector3(0,0,-1) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(-1,0,0), new Vector3(0,0,-1))).Normalized()*planetSize,
                        (new Vector3(-1,0,0) + (percent[1].X-.5f) * 2 * new Vector3(0,0,-1) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(-1,0,0), new Vector3(0,0,-1))).Normalized()*planetSize,
                        (new Vector3(-1,0,0) + (percent[2].X-.5f) * 2 * new Vector3(0,0,-1) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(-1,0,0), new Vector3(0,0,-1))).Normalized()*planetSize,
                        (new Vector3(-1,0,0) + (percent[3].X-.5f) * 2 * new Vector3(0,0,-1) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(-1,0,0), new Vector3(0,0,-1))).Normalized()*planetSize
                    };
                    Vector3[] forwardFace = {
                        (new Vector3(0,0,1) + (percent[0].X-.5f) * 2 * new Vector3(0,1,0) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,1), new Vector3(0,1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,1) + (percent[1].X-.5f) * 2 * new Vector3(0,1,0) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,1), new Vector3(0,1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,1) + (percent[2].X-.5f) * 2 * new Vector3(0,1,0) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,1), new Vector3(0,1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,1) + (percent[3].X-.5f) * 2 * new Vector3(0,1,0) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,1), new Vector3(0,1,0))).Normalized()*planetSize
                    };
                    Vector3[] backwardFace = {
                        (new Vector3(0,0,-1) + (percent[0].X-.5f) * 2 * new Vector3(0,-1,0) + (percent[0].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,-1), new Vector3(0,-1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,-1) + (percent[1].X-.5f) * 2 * new Vector3(0,-1,0) + (percent[1].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,-1), new Vector3(0,-1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,-1) + (percent[2].X-.5f) * 2 * new Vector3(0,-1,0) + (percent[2].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,-1), new Vector3(0,-1,0))).Normalized()*planetSize,
                        (new Vector3(0,0,-1) + (percent[3].X-.5f) * 2 * new Vector3(0,-1,0) + (percent[3].Y - .5f) * 2 * Vector3.Cross(new Vector3(0,0,-1), new Vector3(0,-1,0))).Normalized()*planetSize
                    };

                    List<Vector3[]> cords = new List<Vector3[]>();

                    cords.Add(topFace);
                    cords.Add(bottomFace);
                    cords.Add(leftFace);
                    cords.Add(rightFace);
                    cords.Add(forwardFace);
                    cords.Add(backwardFace);

                    foreach (Vector3[] coords in cords) {
                        for (int i = 0; i < 4; i++) {
                            vertices.Add(coords[i].X*100);
                            vertices.Add(coords[i].Y*100);
                            vertices.Add(coords[i].Z*100);

                            vertices.Add(colors[0]);
                            vertices.Add(colors[1]);
                            vertices.Add(colors[2]);

                            indices.Add((uint)id*4);
                            indices.Add((uint)id*4+1);
                            indices.Add((uint)id*4+2);
                            indices.Add((uint)id*4);
                            indices.Add((uint)id*4+2);
                            indices.Add((uint)id*4+3);
                            id++;
                        }
                    }
                }
            }

            Run();
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            tile.Render();
            //tile.ChangeTerrain(vertices);
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
        }

        protected override void OnLoad() {
            camera = new Camera(this);
            tile = new Tile(camera, vertices, indices);

            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0,0,0,1.0f);
        }
    }
}
