using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace D3.Matrix3DLib
{
	public struct Point3D
	{
		public float X;
		public float Y;
		public float Z;
		private Pen color;
		public void SetColor(Pen p)
		{
			this.color = (Pen)p.Clone();
		}

		public Point3D(float X, float Y, float Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
			color = Pens.Black;
		}

		public double Size()
		{
			return Math.Sqrt(X * X + Y * Y + Z * Z);
		}
		public float GetDistance(Point3D other)
		{
			return (float)Math.Sqrt(((this.X - other.X) * (this.X - other.X) + (this.Y - other.Y) * (this.Y - other.Y) + (this.Z - other.Z) * (this.Z - other.Z)));
		}
		public PointF ToPointF(float d)
		{

			float f = d / (d - Z);
			return new PointF(X * f, Y * f);
		}
		public static bool operator !=(Point3D a, Point3D b)
		{
			return (a.X != b.X || a.Y != b.Y || a.Z != b.Z);
		}
		public static bool operator ==(Point3D a, Point3D b)
		{
			return (a.X == b.X && a.Y == b.Y && a.Z == b.Z);
		}
		public static Point3D GetCenter(params Point3D[] vertices)
		{
			float x = 0, y = 0, z = 0;
			for (int i = 0; i < vertices.Length; i++)
			{
				x += vertices[i].X;
				y += vertices[i].Y;
				z += vertices[i].Z;
			}
			return new Point3D(x / vertices.Length, y / vertices.Length, z / vertices.Length);
		}
	}

}
