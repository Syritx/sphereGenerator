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

        static Vector3[] directions = {
            new Vector3( 0, 1, 0),
            new Vector3( 0,-1, 0),
            new Vector3( 1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3( 0, 0, 1),
            new Vector3( 0, 0,-1),
        };

        Vector3[] axis = {
            new Vector3(directions[0].Y, directions[0].Z, directions[0].X),
            new Vector3(directions[1].Y, directions[1].Z, directions[1].X),
            new Vector3(directions[2].Y, directions[2].Z, directions[2].X),
            new Vector3(directions[3].Y, directions[3].Z, directions[3].X),
            new Vector3(directions[4].Y, directions[4].Z, directions[4].X),
            new Vector3(directions[5].Y, directions[5].Z, directions[5].X),
        };

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) {
            
            for (int x = 0; x < mapResolution; x++) {
                for (int y = 0; y < mapResolution; y++) {

                    Vector2[] percent = { 
                        new Vector2(-1.5f+x,  1.5f+y)/(mapResolution-1),
                        new Vector2( 1.5f+x,  1.5f+y)/(mapResolution-1),
                        new Vector2( 1.5f+x, -1.5f+y)/(mapResolution-1),
                        new Vector2(-1.5f+x, -1.5f+y)/(mapResolution-1),
                    };

                    List<Vector3[]> cords = new List<Vector3[]>();
                    for (int i = 0; i < 6; i++) {
                        Vector3[] coord = {
                            (directions[i] + (percent[0].X-.5f) * 2 * axis[i] + (percent[0].Y - .5f) * 2 * Vector3.Cross(directions[i], axis[i])).Normalized()*planetSize,
                            (directions[i] + (percent[1].X-.5f) * 2 * axis[i] + (percent[1].Y - .5f) * 2 * Vector3.Cross(directions[i], axis[i])).Normalized()*planetSize,
                            (directions[i] + (percent[2].X-.5f) * 2 * axis[i] + (percent[2].Y - .5f) * 2 * Vector3.Cross(directions[i], axis[i])).Normalized()*planetSize,
                            (directions[i] + (percent[3].X-.5f) * 2 * axis[i] + (percent[3].Y - .5f) * 2 * Vector3.Cross(directions[i], axis[i])).Normalized()*planetSize
                        };
                        cords.Add(coord);
                    }

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
