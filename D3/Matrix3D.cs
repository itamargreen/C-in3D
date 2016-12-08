using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D3.Vector3DLib;
using System.Threading.Tasks;

namespace D3.Matrix3DLib
{
    public class Matrix3D
    {
        public float m11;
        public float m12;
        public float m13;
        public float m14 = 0;
        public float m21;
        public float m22;
        public float m23;
        public float m24 = 0;
        public float m31;
        public float m32;
        public float m33;
        public float m34 = 0;
        public float m41;
        public float m42;
        public float m43;
        public float m44 = 1;


        public static Matrix3D operator *(Matrix3D a, Matrix3D b)
        {
            Matrix3D res = new Matrix3D();
            res.m11 = a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
            res.m12 = a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
            res.m13 = a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;
            res.m14 = 0;
            res.m21 = a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
            res.m22 = a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
            res.m23 = a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;
            res.m24 = 0;
            res.m31 = a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
            res.m32 = a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
            res.m33 = a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;
            res.m34 = 0;
            res.m41 = a.m41 * b.m11 + a.m42 * b.m21 + a.m43 * b.m31 + b.m41;
            res.m42 = a.m41 * b.m12 + a.m42 * b.m22 + a.m43 * b.m32 + b.m42;
            res.m43 = a.m41 * b.m13 + a.m42 * b.m23 + a.m43 * b.m33 + b.m43;
            res.m44 = 1;
            return res;
        }
        public static Matrix3D Identity()
        {
            Matrix3D res = new Matrix3D();
            res.m11 = 1;
            res.m12 = 0;
            res.m13 = 0;
            res.m14 = 0;

            res.m21 = 0;
            res.m22 = 1;
            res.m23 = 0;
            res.m24 = 0;

            res.m31 = 0;
            res.m32 = 0;
            res.m33 = 1;
            res.m34 = 0;

            res.m41 = 0;
            res.m42 = 0;
            res.m43 = 0;
            res.m44 = 1;
            return res;
        }
        public static Matrix3D Translate(float tx, float ty, float tz)
        {
            Matrix3D res = Matrix3D.Identity();
            res.m41 = tx;
            res.m42 = ty;
            res.m43 = tz;
            res.m44 = 1;
            return res;
        }
        public static Matrix3D Scale(float tx, float ty, float tz)
        {
            Matrix3D res = Matrix3D.Identity();
            res.m11 = tx;
            res.m22 = ty;
            res.m33 = tz;
            res.m44 = 1;
            return res;
        }
        public static Matrix3D XRotate(float theta)
        {
            Matrix3D res = Matrix3D.Identity();
            float sn = (float)Math.Sin((double)theta);
            float cs = (float)Math.Cos((double)theta);
            res.m22 = cs;
            res.m23 = sn;
            res.m32 = -sn;
            res.m33 = cs;
            return res;
        }
        public static Matrix3D YRotate(float theta)
        {
            Matrix3D res = Matrix3D.Identity();
            float sn = (float)Math.Sin((double)theta);
            float cs = (float)Math.Cos((double)theta);
            res.m11 = cs;
            res.m13 = -sn;
            res.m31 = sn;
            res.m33 = cs;
            return res;
        }
        public static Matrix3D ZRotate(float theta)
        {
            Matrix3D res = Matrix3D.Identity();
            float sn = (float)Math.Sin((double)theta);
            float cs = (float)Math.Cos((double)theta);
            res.m11 = cs;
            res.m12 = sn;
            res.m21 = -sn;
            res.m22 = cs;
            return res;
        }
        public static Matrix3D LineRotate(Vector3D axis, Point3D p, float theta)
        {
            axis.SetLength(1.0f);
            float l = (float)Math.Sqrt((axis.X * axis.X) + (axis.Y * axis.Y));
            float sinA = axis.X / l;
            float cosA = axis.Y / l;
            float alpha = (float)Math.Acos(cosA);
            float sinB = axis.Y;
            float cosB = axis.Z;
            float beta = (float)Math.Acos(sinB);

            float sn = (float)Math.Sin(theta);
            float cs = (float)Math.Cos(theta);
            float c = 1 - cs;
            Matrix3D m = new Matrix3D();
            m.m11 = axis.X * axis.X * c + cs;
            m.m12 = axis.X * axis.Y * c + axis.Z * sn;
            m.m13 = axis.X * axis.Z * c - axis.Y*sn;

            m.m21 = axis.Y * axis.X * c - axis.Z*sn;
            m.m22 = axis.Y * axis.Y * c + cs;
            m.m23 = axis.Y * axis.Z * c + sn*axis.X;

            m.m31 = axis.X * axis.Z * c + sn*axis.Y;
            m.m32 = axis.Z * axis.Y * c - sn*axis.X;
            m.m33 = axis.Z * axis.Z * c + cs;

            m.m41 = p.X - p.X * m.m11 - p.Y * m.m21 - p.Z * m.m31;
            m.m42 = p.Y - p.X * m.m12 - p.Y * m.m22 - p.Z * m.m32;
            m.m43 = p.Z - p.X * m.m13 - p.Y * m.m23 - p.Z * m.m33;
            return m;

        }
        public void Multiply(Matrix3D m)
        {
            Matrix3D a = (this * m);
            this.m11 = a.m11;
            this.m12 = a.m12;
            this.m13 = a.m13;
            this.m14 = a.m14;
            this.m21 = a.m21;
            this.m22 = a.m22;
            this.m23 = a.m23;
            this.m24 = a.m24;
            this.m31 = a.m31;
            this.m32 = a.m32;
            this.m33 = a.m33;
            this.m34 = a.m34;
            this.m41 = a.m41;
            this.m42 = a.m42;
            this.m43 = a.m43;
            this.m44 = a.m44;
        }
        public static Vector3D operator *(Vector3D v, Matrix3D m)
        {
            float x = v.X * m.m11 + v.Y * m.m21 + v.Z * m.m31;
            float y = v.X * m.m12 + v.Y * m.m22 + v.Z * m.m32;
            float z = v.X * m.m13 + v.Y * m.m23 + v.Z * m.m33;
            return new Vector3D(x,y,z);
        }
        public static Point3D operator *(Point3D p, Matrix3D m)
        {
            float x = p.X * m.m11 + p.Y * m.m21 + p.Z * m.m31 + m.m41;
            float y = p.X * m.m12 + p.Y * m.m22 + p.Z * m.m32 + m.m42;
            float z = p.X * m.m13 + p.Y * m.m23 + p.Z * m.m33 + m.m43;
            return new Point3D(x, y, z);
        }
        public void TransformPoints(ref Point3D p)
        {
            float x = p.X * m11 + p.Y * m21 + p.Z * m31 + m41;
            float y = p.X * m12 + p.Y * m22 + p.Z * m32 + m42;
            float z = p.X * m13 + p.Y * m23 + p.Z * m33 + m43;
            p.X = x;
            p.Y = y;
            p.Z = z;
        }
		public Point3D TransformPoint(Point3D p)
		{
			float x = p.X * m11 + p.Y * m21 + p.Z * m31 + m41;
			float y = p.X * m12 + p.Y * m22 + p.Z * m32 + m42;
			float z = p.X * m13 + p.Y * m23 + p.Z * m33 + m43;
			p.X = x;
			p.Y = y;
			p.Z = z;
			return p;
		}
        public Point3D[] TransformPoints(Point3D[] points)
        {
            float x, y, z;
            for (int i = 0; i<points.Length;i++)
            {
                
                x = points[i].X * m11 + points[i].Y * m21 + points[i].Z * m31 + m41;
                y = points[i].X * m12 + points[i].Y * m22 + points[i].Z * m32 + m42;
                z = points[i].X * m13 + points[i].Y * m23 + points[i].Z * m33 + m43;
                points[i].X = x;
                points[i].Y = y;
                points[i].Z = z;
            }
			return points;
        }
        public void TransformVector(ref Vector3D v)
        {
            v.X = v.X * m11 + v.Y * m21 + v.Z * m31;
            v.Y = v.X * m12 + v.Y * m22 + v.Z * m32;
            v.Z = v.X * m13 + v.Y * m23 + v.Z * m33;
        }
    }

}
