using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    public class GameCamera
    {
        public Matrix camViewMatrix; //Cameras view
        public Matrix camProjectionMatrix; //
        public Matrix camRotationMatrix; //Rotation Matrix for camera to reflect movement around Y Axis
        public Vector3 camPosition; //Position of Camera in world
        public Vector3 camLookat; //Where the camera is looking or pointing at
        public Vector3 camTransform; //Used for repositioning the camer after it has been rotated
        public float camRotationSpeed; //Defines the amount of rotation
        public float camYaw; //Cumulative rotation on Y
        public float camPitch; //Cumulative rotation on X
        public bool firstPerson = false;

        public GameCamera(Vector3 mdlPosition)
        {
            camPosition = mdlPosition + new Vector3(0.0f, 3.0f, 15.0f);
            camLookat = mdlPosition;
            camViewMatrix = Matrix.CreateLookAt(camPosition, camLookat, Vector3.Up);
        }

        public void camUpdate(GameCamera cam, Vector3 mdlPosition)
        {
            camPosition = mdlPosition + new Vector3(0.0f, 3.0f, 15.0f);
            //cam.camLookat = mdlPosition - ((Vector3.UnitX) * -(float)Math.Sin(mdlRotation) * 0.05f * 15.0F) + ((Vector3.UnitZ) * (float)Math.Cos(mdlRotation) * 0.05f * 15.0F);
            //cam.camRotationMatrix = -Matrix.CreateRotationY(mdlRotation);
            //cam.camTransform = Vector3.Transform(Vector3.Forward, cam.camRotationMatrix);
            cam.camViewMatrix = Matrix.CreateLookAt(cam.camPosition, cam.camLookat, Vector3.Up);
        }

        public void camUpdate(GameCamera cam, Vector3 mdlPosition, float mdlRotation)
        {
            cam.camPosition = mdlPosition + new Vector3(0.0f, 0.0f, 0.0f);
        //    if (mdlRotation != 0f)
        //    {
        //        cam.camRotationMatrix = Matrix.CreateRotationY(mdlRotation);
        //    }
        //    if (cam.camYaw != 0f)
        //    {
        //        cam.camRotationMatrix = - Matrix.CreateRotationZ(cam.camYaw);
        //        Console.WriteLine("True");
        //    }
        //    cam.camPosition = mdlPosition + ((Vector3.UnitX) * -(float)Math.Sin(mdlRotation) * 25.0f) + ((Vector3.UnitZ) * (15 - (float)Math.Sin(mdlRotation) * 25.0f)) + Vector3.UnitY*10;
        //    //cam.camPosition = mdlPosition + new Vector3(0.0f, 3.0f, 15.0f);
        //    cam.camLookat = mdlPosition - ((Vector3.UnitX) * -(float)Math.Sin(mdlRotation) * 0.05f) + ((Vector3.UnitZ) * (float)Math.Cos(mdlRotation) * 0.05f);
            cam.camLookat = camPosition;
            cam.camViewMatrix = Matrix.CreateLookAt(camPosition, camLookat, Vector3.Up);
        }
    }
}
