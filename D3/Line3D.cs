using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D3.Matrix3DLib;
using D3.Vector3DLib;
namespace D3.Plane3DLib
{

	public class Line3D
	{


		public Vector3D variable { get; set; }
		public Point3D start { get; set; }
		public Line3D(Point3D start, Point3D onLine)
		{
			this.start = start;
			this.variable = new Vector3D(onLine, start);

		}

	}
}
