using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using D3.Matrix3DLib;
using Polygon2DLib;
namespace D3.Polygon3DLib
{
    public class Polygon3D
    {
        public enum Direction { XAxis, YAxis, ZAxis };
        public int VerticesNum()
        {
            return vertices.Length;
        }
        public Point3D[] GetPoints()
        {
            return vertices;
        }
        private PointF[] plane;
        private Point3D[] vertices;
        public Polygon3D(Polygon2D polygonBase, float height = 0)
        {
            
            PointF[] points = polygonBase.GetPoints();
            this.plane = new PointF[points.Length];
            vertices = new Point3D[points.Length];
            Array.Copy(points, plane, points.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new Point3D(points[i].X, points[i].Y, height);
            }
        }
        
        public void Draw(Graphics gr)
        {
            
            for (int i = 0; i < vertices.Length; i++)
            {
                plane[i] = vertices[i].ToPointF(Camera3DLib.Camera3D.GetDistance());
            }
            Polygon2D polygon = new Polygon2D(plane);
            polygon.Draw(gr);

        }
        public void Apply(Matrix3D m)
        {
            m.TransformPoints(this.vertices);
            //for (int i = 0; i < this.vertices.Length; i++)
            //{
                
            //    this.vertices[i].Apply(m);
            //}

        }
        public Point3D GetCenter()
        {
            Point3D[] points = this.vertices;
            float x = 0, y = 0, z = 0;
            for (int i = 0; i < points.Length; i++)
            {
                x += points[i].X;
                y += points[i].Y;
                z += points[i].Z;
            }
            x /= points.Length;
            y /= points.Length;
            z /= points.Length;
            return new Point3D(x, y, z);
        }

        
    }
}
