using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3.Matrix3DLib;
namespace D3.Vector3DLib
{
    public struct Vector3D
    {
        public float X;
        public float Y;
        public float Z;
        //public float w = 0;
        public Vector3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vector3D(Point3D a, Point3D b)
        {
            this.X = b.X - a.X;
            this.Y = b.Y - a.Y;
            this.Z = b.Z - a.Z;
        }
        public static Vector3D operator *(Vector3D v, float f)
        {
            return new Vector3D(f * v.X, f * v.Y, f * v.Z);
        }
        public static Vector3D operator +(Vector3D v, Vector3D b)
        {
            return new Vector3D(v.X+b.X, v.Y + b.Y, v.Z + b.Z);
        }
        public static float operator ^(Vector3D v, Vector3D b)
        {
            return (v.X * b.X+ v.Y * b.Y+ v.Z * b.Z);
        }
        public static Vector3D operator *(Vector3D v, Vector3D b)
        {
            float xComp = v.Y * b.Z - v.Z * b.Y;
            float yComp =-( v.X * b.Z - v.Z * b.X);
            float zComp = v.X * b.Y - v.Y * b.X;
            return new Vector3D(xComp,yComp,zComp);
        }
        public static Point3D operator +(Vector3D v, Point3D b)
        {
            return new Point3D(v.X + b.X, v.Y + b.Y, v.Z + b.Z);
        }
        public float GetLength()
        {
            return (float)Math.Sqrt((this.X) * (this.X) + (this.Y) * (this.Y) + (this.Z) * (this.Z));
        }
        public void SetLength(float l)
        {
            float t = l / GetLength();
            X *= t;
            Y *= t;
            Z *= t;

        }
        public string ToString()
        {
            return "" + X + "   " + Y + "   " + Z;
        }
    }
}
