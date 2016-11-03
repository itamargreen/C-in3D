using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using D3.Matrix3DLib;
using D3.Vector3DLib;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Net;

namespace D3.Solid3D
{
    public class Solid3D : ISerializable
    {
        public enum Direction { XAxis, YAxis, ZAxis };
        private Point3D[] vertices;
        private Face3D[] faces;

        private Brush fillColor;
        public float CameraDistance()
        {
            Vector3D camera = new Vector3D(Camera3DLib.Camera3D.GetCameraPoint(), GetCenter());
            return camera.GetLength();
        }
        public void ClampVertices()
        {
            Point3D[] points = GetRealPoints();
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = 10.0f * ((int)points[i].X / 10);
                points[i].Y = 10.0f * ((int)points[i].Y / 10);
                points[i].Z = 10.0f * ((int)points[i].Z / 10);
            }
        }
        public Solid3D(Point3D[] vertices, Face3D[] faces)
        {
            this.vertices = new Point3D[vertices.Length];
            Array.Copy(vertices, this.vertices, vertices.Length);
            this.faces = new Face3D[faces.Length];
            Array.Copy(faces, this.faces, faces.Length);
            fillColor = Brushes.White;
        }
        public static Solid3D GetCylinder(float r, float h, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
        {
            Point3D[] vertices = new Point3D[12];
            Face3D[] faces = new Face3D[8];
            for (int i = 0; i < 6; i++)
            {
                double temp = 2 * Math.PI / 6 * i;
                float x = r * (float)Math.Sin((temp));
                float y = r * (float)Math.Cos((temp));
                vertices[i] = new Point3D(x + offsetX, y + offsetY, offsetZ);
            }
            for (int i = 6; i < 12; i++)
            {
                double temp = 2 * Math.PI / 6 * (i - 6);
                float x = r * (float)Math.Sin((temp));
                float y = r * (float)Math.Cos((temp));
                vertices[i] = new Point3D(x + offsetX, y + offsetY, h + offsetZ);
            }
            faces[1] = new Face3D(7, 8, 9, 10, 11, 6);
            faces[0] = new Face3D(1, 0, 5, 4, 3, 2);
            for (int j = 2; j < faces.Length; j++)
            {
                int i = j - 2;
                if (j == faces.Length - 1)
                {
                    faces[j] = new Face3D(i % (vertices.Length / 2), (i + 1) % (vertices.Length / 2), vertices.Length / 2, i + 6);
                    continue;
                }
                faces[j] = new Face3D(i % (vertices.Length / 2), (i + 1) % (vertices.Length / 2), (i + 7) % (vertices.Length), i + 6);
            }
            return new Solid3D(vertices, faces);

        }
        public Point3D GetCenter()
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

        public static Solid3D GetCube(float w, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
        {
            Point3D[] vertices = new Point3D[8];
            Face3D[] faces = new Face3D[6];

            vertices[0] = new Point3D(offsetX, offsetY, w + offsetZ);
            vertices[1] = new Point3D(w + offsetX, offsetY, w + offsetZ);
            vertices[2] = new Point3D(w + offsetX, w + offsetY, w + offsetZ);
            vertices[3] = new Point3D(offsetX, w + offsetY, w + offsetZ);
            vertices[4] = new Point3D(offsetX, offsetY, offsetZ);
            vertices[5] = new Point3D(w + offsetX, offsetY, offsetZ);
            vertices[6] = new Point3D(w + offsetX, w + offsetY, offsetZ);
            vertices[7] = new Point3D(offsetX, w + offsetY, offsetZ);

            faces[0] = new Face3D(0, 1, 2, 3);
            faces[1] = new Face3D(1, 5, 6, 2);
            faces[2] = new Face3D(1, 0, 4, 5);
            faces[3] = new Face3D(4, 0, 3, 7);
            faces[4] = new Face3D(5, 4, 7, 6);
            faces[5] = new Face3D(2, 6, 7, 3);
            return new Solid3D(vertices, faces);
        }
        public Point3D[] GetVertices()
        {
            return this.vertices;
        }


        public float GetDot(int index)
        {
            Point3D[] verts = GetFaceVertices(index);
            Vector3D zeroToOne = new Vector3D(verts[0], verts[1]);
            Vector3D oneToTwo = new Vector3D(verts[1], verts[2]);


            Vector3D perp = zeroToOne * oneToTwo;
            Vector3D camera = Camera3DLib.Camera3D.GetCameraVector();

            double value = (double)(perp ^ camera);
            return (float)value;
        }
        public Point3D[] GetFaceVertices(int index)
        {
            Point3D[] points = new Point3D[4];
            for (int i = 0; i < 4; i++)
            {
                points[i] = GetRealPoints()[faces[index].links[i]];
            }
            return points;
        }
        public Point3D GetFaceCenter(int index)
        {
            Point3D[] points = new Point3D[4];
            for (int i = 0; i < 4; i++)
            {
                points[i] = GetRealPoints()[faces[index].links[i]];
            }
            return Point3D.GetCenter(points);
        }
        public Face3D GetFace(int i)
        {
            return faces[i];
        }
        public Point3D[] GetRealPoints()
        {
            float distance = Camera3DLib.Camera3D.GetDistance();

            Point3D[] verts = new Point3D[this.vertices.Length];
            float f;
            for (int i = 0; i < verts.Length; i++)
            {
                f = distance / (distance - this.vertices[i].Z);
                verts[i].X = f * this.vertices[i].X;
                verts[i].Y = f * this.vertices[i].Y;
                verts[i].Z = this.vertices[i].Z;
            }
            return verts;
        }
        public void Draw(Graphics gr)
        {
            float distance = Camera3DLib.Camera3D.GetDistance();

            Point3D[] verts = new Point3D[this.vertices.Length];
            float f;
            for (int i = 0; i < verts.Length; i++)
            {
                f = distance / (distance - this.vertices[i].Z);
                verts[i].X = f * this.vertices[i].X;
                verts[i].Y = f * this.vertices[i].Y;
                verts[i].Z = this.vertices[i].Z;
            }

            for (int i = 0; i < this.faces.Length; i++)
            {

                Vector3D zeroToOne = new Vector3D(verts[faces[i].links[0]], verts[faces[i].links[1]]);
                Vector3D oneToTwo = new Vector3D(verts[faces[i].links[1]], verts[faces[i].links[2]]);
                Vector3D perp = zeroToOne * oneToTwo;
                float z = (perp).Z;
                if (z > 0)
                {
                    PointF[] points = new PointF[faces[i].links.Length];
                    for (int j = 0; j < points.Length; j++)
                    {
                        points[j].X = verts[faces[i].links[j]].X;
                        points[j].Y = verts[faces[i].links[j]].Y;
                    }
                    //gr.FillRegion(Brushes.Cyan, faces[i].GetRegion(verts));
                    gr.FillPolygon(fillColor, points);
                    gr.DrawPolygon(faces[i].borderColor, points);
                }
            }
        }
        public void SetFillColor(Brush b)
        {
            this.fillColor = (Brush)b.Clone();
        }
        public void Apply(Matrix3D m)
        {

            m.TransformPoints(this.vertices);
            ClampVertices();
        }
        public Face3D GetSelectedFace(PointF mouse)
        {
            Face3D res = null;
            float distance = Camera3DLib.Camera3D.GetDistance();
            Point3D[] verts = new Point3D[this.vertices.Length];
            float f;
            for (int i = 0; i < verts.Length; i++)
            {
                f = distance / (distance - this.vertices[i].Z);
                verts[i].X = f * this.vertices[i].X;
                verts[i].Y = f * this.vertices[i].Y;
                verts[i].Z = this.vertices[i].Z;
            }
            for (int i = 0; i < faces.Length; i++)
            {

                Vector3D zeroToOne = new Vector3D(verts[faces[i].links[0]], verts[faces[i].links[1]]);
                Vector3D oneToTwo = new Vector3D(verts[faces[i].links[1]], verts[faces[i].links[2]]);
                Vector3D perp = zeroToOne * oneToTwo;
                float z = (perp).Z;
                if (z > 0)
                {
                    if (faces[i].PointInFace(mouse, verts))
                    {
                        res = faces[i];
                        break;
                    }
                }
                else
                {
                    continue;
                }

            }
            return res;
        }
        public Point3D GetSelectedPoint(PointF mouse)
        {
            Point3D res = new Point3D(-1,-1,-1);
            float distance = Camera3DLib.Camera3D.GetDistance();
            Point3D[] verts = new Point3D[this.vertices.Length];
            float f;
            for (int i = 0; i < verts.Length; i++)
            {
                f = distance / (distance - this.vertices[i].Z);
                verts[i].X = f * this.vertices[i].X;
                verts[i].Y = f * this.vertices[i].Y;
                verts[i].Z = this.vertices[i].Z;
            }
            for (int i = 0; i < verts.Length; i++)
            {
                float dist = (float)Math.Sqrt((mouse.X - verts[i].ToPointF(distance).X) * (mouse.X - verts[i].ToPointF(distance).X) + (mouse.Y - verts[i].ToPointF(distance).Y) * (mouse.Y - verts[i].ToPointF(distance).Y));
                if (dist < 150.0f)
                {
                    res = verts[i];
                }
            }
            //for (int i = 0; i < faces.Length; i++)
            //{

            //    Vector3D zeroToOne = new Vector3D(verts[faces[i].links[0]], verts[faces[i].links[1]]);
            //    Vector3D oneToTwo = new Vector3D(verts[faces[i].links[1]], verts[faces[i].links[2]]);
            //    Vector3D perp = zeroToOne * oneToTwo;
            //    float z = (perp).Z;
            //    if (z > 0)
            //    {
            //        if (faces[i].PointInFace(mouse, verts))
            //        {
            //            res = faces[i];
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        continue;
            //    }

            //}
            return res;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("vertices",vertices,vertices.GetType());
            info.AddValue("faces", faces, faces.GetType());
            
        }
    }
}
