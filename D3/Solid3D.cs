using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using D3.Matrix3DLib;
using D3.Vector3DLib;
using D3.Plane3DLib;
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
		public Vector3D _velocity { get; set; }
		
		private Brush fillColor;
		public Face3D[] GetFaces()
		{
			return faces;
		}
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
			this._velocity = new Vector3D(0, 0, 0);
		}

		public static Solid3D GetSphere(float r, int pPBase, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			int nParallels = (pPBase - 2) / 2;
			int nVerts = 2 + pPBase * nParallels;
			Point3D[] vertices = new Point3D[nVerts];
			int nFaces = 2 * pPBase + (nParallels - 1) * pPBase;
			Face3D[] faces = new Face3D[nFaces];
			Matrix3D mZ = Matrix3D.ZRotate((float)(2 * Math.PI / pPBase));
			Matrix3D mY = Matrix3D.YRotate((float)(2 * Math.PI / pPBase));
			int i, j, ind;
			Point3D p = new Point3D(0, r, 0);
			vertices[0] = p;
			for (i = 0; i < nParallels; i++)
			{
				p = p * mZ;
				for (j = 0; j < pPBase; j++)
				{
					vertices[1 + i * pPBase + j] = p;
					p = p * mY;
				}
			}
			vertices[nVerts - 1] = new Point3D(0, -r, 0);
			////////////////////////////////////////////////////////////////////////////////////////////////////////////
			for (i = 0; i < pPBase; i++)
			{
				//*************** Up Faces ******************************
				faces[i] = new Face3D(new int[3] { 0, i + 1, (i + 1) % pPBase + 1 });
				//*************** Down Faces ****************************
				faces[i + pPBase] = new Face3D(new int[3]
				{ nVerts - 1 - pPBase + i, nVerts - 1, nVerts - 1 - pPBase + (i + 1) % pPBase });
			}
			for (i = 0; i < nParallels - 1; i++)
			{
				ind = 2 * pPBase + i * pPBase;
				for (j = 0; j < pPBase; j++)
				{
					faces[ind + j] = new Face3D(new int[4]
					{ i * pPBase + j + 1, i * pPBase + j + 1 + pPBase, i * pPBase + (j + 1) % pPBase + 1 + pPBase, i * pPBase + (j + 1) % pPBase + 1 });
				}

			}
			for (int k = 0; i < vertices.Length; i++)
			{
				Point3D point = vertices[k];
				point.X += offsetX;
				point.Y += offsetY;
				point.Z += offsetZ;
			}




			return new Solid3D(vertices, faces);
		}
		public static Solid3D GetPyramid(float r, float h, int pPBase, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			int nVerts = pPBase + 1;
			Point3D[] vertices = new Point3D[nVerts];
			int nFaces = 1 + pPBase;
			Face3D[] faces = new Face3D[nFaces];
			Matrix3D mY = Matrix3D.YRotate((float)(2 * Math.PI / pPBase));
			Point3D p = new Point3D(r + offsetX, offsetY, offsetZ);
			int[] baseLinks = new int[pPBase];
			for (int i = 0; i < pPBase; i++)
			{
				vertices[i] = p;
				p = p * mY;
				baseLinks[i] = pPBase - 1 - i;
				faces[i + 1] = new Face3D(new int[3]
				{ i, (i + 1) % pPBase, pPBase });
			}
			vertices[nVerts - 1] = new Point3D(0 + offsetX, h + offsetY, offsetZ);
			faces[0] = new Face3D(baseLinks);
			return new Solid3D(vertices, faces);
		}
		public static Solid3D GetCylinder(float r, float h, int pPBase, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			int nVerts = 2 * pPBase;
			Point3D[] vertices = new Point3D[nVerts];
			int nFaces = 2 + pPBase;
			Face3D[] faces = new Face3D[nFaces];
			Matrix3D mY = Matrix3D.YRotate((float)(2 * Math.PI / pPBase));
			Point3D p1 = new Point3D(r + offsetX, offsetY, offsetZ);
			Point3D p2 = new Point3D(r + offsetX, h + offsetY, offsetZ);
			int[] downBaseLinks = new int[pPBase];
			int[] upBaseLinks = new int[pPBase];
			for (int i = 0; i < pPBase; i++)
			{
				vertices[i] = p1;
				vertices[i + pPBase] = p2;
				p1 = p1 * mY;
				p2 = p2 * mY;
				downBaseLinks[i] = pPBase - 1 - i;
				upBaseLinks[i] = pPBase + i;
				faces[i + 2] = new Face3D(new int[4]
				{ i, (i + 1) % pPBase, (i + 1) % pPBase + pPBase, i + pPBase });
			}
			faces[0] = new Face3D(downBaseLinks);
			faces[1] = new Face3D(upBaseLinks);
			return new Solid3D(vertices, faces);
		}
		public static Solid3D GetHollowCylinder(float rBig, float rSmall, float h, int pPBase, float offsetX = 0, float offsetY = 0, float offsetZ = 0)
		{
			int nVerts = 4 * pPBase;
			Point3D[] vertices = new Point3D[nVerts];
			int nFaces = 4 * pPBase;
			Face3D[] faces = new Face3D[nFaces];
			Matrix3D mY = Matrix3D.YRotate((float)(2 * Math.PI / pPBase));
			Point3D p1 = new Point3D(rBig + offsetX, offsetY, offsetZ);
			Point3D p2 = new Point3D(rSmall + offsetX, offsetY, offsetZ);
			for (int i = 0; i < pPBase; i++)
			{
				// Small Sylinder;
				vertices[i] = p2;
				vertices[i + pPBase] = new Point3D(p2.X + offsetX, h + offsetY, p2.Z + offsetZ);
				// Big Sylinder;
				vertices[i + 2 * pPBase] = p1;
				vertices[i + 3 * pPBase] = new Point3D(p1.Y + offsetX, h + offsetY, p1.Y + offsetZ);
				p1 = p1 * mY;
				p2 = p2 * mY;
				// Inner Faces in small radius of Small Sylinder
				faces[i] = new Face3D(new int[4]
				{ (i + 1) % pPBase, i, i + pPBase, (i + 1) % pPBase + pPBase });
				//  Back Faces in big radius Of Big Cylinder
				faces[i + pPBase] = new Face3D(new int[4]
				{ i + 2 * pPBase, (i + 1) % pPBase + 2 * pPBase, (i + 1) % pPBase + pPBase + 2 * pPBase, i + pPBase + 2 * pPBase });
				// Down base Faces
				faces[i + 2 * pPBase] = new Face3D(new int[4]
                // Up base Faces
                { i, (i + 1) % pPBase, (i + 1) % pPBase + 2 * pPBase, i + 2 * pPBase });
				faces[i + 3 * pPBase] = new Face3D(new int[4]
				{ (i + 1) % pPBase + pPBase, i + pPBase, i + 3 * pPBase, (i + 1) % pPBase + 3 * pPBase });
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
		public void SetFaceVertices(int index, params Point3D[] vertices)
		{
			Face3D face = GetFace(index);
			if(face.links.Length != vertices.Length)
			{
				Console.Error.WriteLine("Invalid points!!");
				return;
			}
			
			for(int i = 0; i<face.links.Length; i++)
			{
				this.vertices[i] = vertices[i];	
			}
			
			
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
		public Point3D GetFaceCenter(Face3D face)
		{
			int index = -1;
			for (int i = 0; i < faces.Length; i++)
			{
				if (faces[i] == face)
				{
					index = i;
					break;
				}
			}
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
		public int GetIndex(Face3D face)
		{
			int index = -1;
			for (int i = 0; i < faces.Length; i++)
			{
				if (faces[i] == face)
				{
					index = i;
					break;
				}
			}
			return index;
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

				Vector3D zeroToOne = new Vector3D(vertices[faces[i].links[0]], vertices[faces[i].links[1]]);
				Vector3D oneToTwo = new Vector3D(vertices[faces[i].links[1]], vertices[faces[i].links[2]]);
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
					gr.FillPolygon(faces[i].fillColor, points);
					gr.DrawPolygon(faces[i].borderColor, points);
				}
			}
		}
		public void SetFillColor(Brush b)
		{
			foreach (Face3D face in faces)
			{
				face.SetBrush(b);
			}

		}
		
		public void UpdateVelocity(Vector3D vect)
		{
			Vector3D velTemp = vect + this._velocity;
			
			Matrix3D mat2 = Matrix3D.Translate(velTemp.X, velTemp.Y, velTemp.Z);
			
			Point3D[] temp = new Point3D[this.vertices.Length];
			Array.Copy(this.vertices, temp,this.vertices.Length);
            foreach (Point3D point in temp)
			{
				if (mat2.TransformPoint(point).Y <= -50)
				{
					return;
				}
			}
			this._velocity = velTemp;
			mat2.TransformPoints(this.vertices);

		}

		public void Apply(Matrix3D m)
		{
			Point3D[] temp = new Point3D[this.vertices.Length];
			Array.Copy(this.vertices, temp, this.vertices.Length);
			foreach (Point3D point in temp)
			{
				if (m.TransformPoint(point).Y <= -50)
				{
					return;
				}
			}
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
				faces[i].SetVertices(GetFaceVertices(i));
				
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
		public Point3D GetFaceIntersection(PointF mouse)
		{
			Point3D res = new Point3D(-1, -1, -1);
			Point3D mouse3D = new Point3D(mouse.X, mouse.Y, 0);
			Point3D camera = Camera3DLib.Camera3D.GetCameraPoint();
			Line3D line = new Line3D(camera, mouse3D);

			List<Face3D> faceCenters = new List<Face3D>();
			for (int i = 0; i < faces.Length; i++)
			{
				faceCenters.Add(faces[i]);
			}
			faceCenters.OrderBy(x => GetFaceCenter(x).GetDistance(camera));
			Face3D selected = faceCenters.ElementAt(0);
			Point3D[] verts = GetRealPoints();

			Plane3D face = new Plane3D(verts[selected.links[0]], verts[selected.links[1]], verts[selected.links[2]]);
			res = face.GetLineIntersection(line);



			return res;
		}
		public Point3D GetSelectedPoint(PointF mouse)
		{
			Point3D mouse3D = GetFaceIntersection(mouse);
			Point3D res = new Point3D(-1, -1, -1);
			float distance = Camera3DLib.Camera3D.GetDistance();
			Point3D[] verts = GetRealPoints();
			for (int i = 0; i < verts.Length; i++)
			{
				float dist = verts[i].GetDistance(mouse3D);

				if (dist < 3000.0f)
				{
					res = verts[i];
					break;
				}
			}

			return res;
		}
		
		public override string ToString()
		{
			string res = "";
			foreach(Point3D point in vertices)
			{
				res += point.ToString() + Environment.NewLine;
			}
			return res;
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("vertices", vertices, vertices.GetType());
			info.AddValue("faces", faces, faces.GetType());

		}
	}
}
