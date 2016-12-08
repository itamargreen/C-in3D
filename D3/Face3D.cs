using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using D3.Matrix3DLib;
using D3.Vector3DLib;

namespace D3.Solid3D
{
    public class Face3D
    {
        internal int[] links;
        internal Pen borderColor;
        internal Brush fillColor;
		public Point3D[] vertices { get; set; }
		public void SetBrush(Brush b)
        {
            this.fillColor = (Brush)b.Clone();
        }
		public void SetVertices(params Point3D[] verts)
		{
			this.vertices = new Point3D[verts.Length];
			Array.Copy(verts, this.vertices, verts.Length);
		}
        public Face3D(params int[] links)
        {
            this.links = new int[links.Length];
            Array.Copy(links, this.links, links.Length);
            this.borderColor = Pens.Black;
            this.fillColor = Brushes.White;
        }
        public Region GetRegion(params Point3D[] facePoints)
        {
            float distance = Camera3DLib.Camera3D.GetDistance();
            PointF[] planePoints = new PointF[links.Length];
            for (int i = 0; i < planePoints.Length; i++)
            {
                planePoints[i] = facePoints[links[i]].ToPointF(distance);
            }

            GraphicsPath pth = new GraphicsPath();

            pth.AddPolygon(planePoints);
            Region r = new Region(pth);
            return r;
        }
        public bool PointInFace(PointF point, params Point3D[] facePoints)
        {
            float distance = Camera3DLib.Camera3D.GetDistance();
            PointF[] planePoints = new PointF[links.Length];
            for (int i = 0; i < planePoints.Length; i++)
            {
                planePoints[i] = facePoints[links[i]].ToPointF(distance);
            }

            GraphicsPath pth = new GraphicsPath();
            
            pth.AddPolygon(planePoints);
            Region r = new Region(pth);
            
            return r.IsVisible(point);
            

        }
        public bool ShouldDrawFace(params Point3D[] verts)
        {
            Vector3D zeroToOne = new Vector3D(verts[links[0]], verts[links[1]]);
            Vector3D oneToTwo = new Vector3D(verts[links[1]], verts[links[2]]);

            Vector3D perp = zeroToOne * oneToTwo;
            Vector3D camera = new Vector3D(Camera3DLib.Camera3D.GetCameraPoint(), Point3D.GetCenter(verts));

            double value = Math.Acos((perp ^ camera) / (perp.GetLength() * camera.GetLength()));
            return (value < (Math.PI / 2.0)) && value > (-Math.PI / 2.0);

        }
		//public static bool operator ==(Face3D a, Face3D b)
		//{
		//	bool flag = true;
		//	if(null == a.links)
		//	{
		//		return (null == b.links);
		//	}
		//	for (int i = 0; i < a.links.Length; i++)
		//	{
		//		if (a.links[i] != b.links[i])
		//		{
		//			flag = false;
		//		}
					
		//	}
		//	return flag;
		//}
		//public static bool operator !=(Face3D a, Face3D b)
		//{
		//	bool flag = false;
		//	if (null == a.links)
		//	{
		//		return (null != b.links);
		//	}
		//	for (int i = 0; i < a.links.Length; i++)
		//	{
		//		if (a.links[i] != b.links[i])
		//		{
		//			flag = true;
		//		}

		//	}
		//	return flag;
		//}
	}
}
