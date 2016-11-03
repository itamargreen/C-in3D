using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3.Matrix3DLib;
using D3.Vector3DLib;

namespace D3.Camera3DLib
{
    public static class Camera3D
    {
        private static float d = 1000;
        public static Point3D GetCameraPoint()
        {
            return new Point3D(0, 0, d);
        }
        public static Vector3D GetCameraVector()
        {
            Point3D camera = new Point3D(0, 0, d);
            Point3D zero = new Point3D(0, 0, 0);
            return new Vector3D(camera,zero);
        }
        public static float GetDistance()
        {
            return d;
        }
        public static void ChangeDistance(float change)
        {
            d += change;
        }
        public static void SetDistace(float d)
        {
            Camera3D.d = d;
        }
    }
}
