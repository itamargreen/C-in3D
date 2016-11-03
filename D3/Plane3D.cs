using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3.Matrix3DLib;
using D3.Vector3DLib;
namespace D3.Plane3DLib
{
	public struct Plane3D
	{
		public float a;
		public float b;
		public float c;
		public float d;

		public Plane3D(params Point3D[] points)
		{
			Point3D p1 = points[0];
			Point3D p2 = points[1];
			Point3D p3 = points[2];

			a = (p2.Y - p1.Y) * (p3.Z - p1.Z) - (p3.Y - p2.Y) * (p2.Z - p1.Z);
			b = (p2.Z - p1.Z) * (p3.X - p1.X) - (p3.Z - p2.Z) * (p2.X - p1.X);
			c = (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p2.X) * (p2.Y - p1.Y);
			d = -(a * p1.X + b * p1.Y + c * p1.Z);
		}
		public bool IsPointOnPlane(Point3D point)
		{
			return (a * point.X + b * point.Y + c * point.Z + d == 0);
		}
		public Point3D GetLineIntersection(Line3D line)
		{
			Point3D res = new Point3D();
			Point3D start = line.start;
			Vector3D vect = line.variable;
			res.X = start.X - (-vect.X * (a * start.X + b * start.Y + c * start.Z + d) /
								 (a * vect.X + b * vect.Y + c * vect.Z));
			res.Y = start.Y - (-vect.Y * (a * start.X + b * start.Y + c * start.Z + d) /
								 (a * vect.X + b * vect.Y + c * vect.Z));
			res.Z = start.Z - (-vect.Z * (a * start.X + b * start.Y + c * start.Z + d) /
								 (a * vect.X + b * vect.Y + c * vect.Z));
			return res;
		}
	}
}
