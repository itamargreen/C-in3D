using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using D3.Vector3DLib;
using D3.Matrix3DLib;
using D3.Camera3DLib;
using D3.Polygon3DLib;
using Polygon2DLib;
using D3.Solid3D;
namespace Start3D
{
	static class Scene
	{
		private static Point3D[] points;
		private static Polygon3D[] polygons;
		private static Solid3D[] solids;
		private static int selection = -1;
		private static List<Solid3D> selected;
		private static List<Point3D> selectedPoints;
		private static IEnumerable<Solid3D> solidsEn;
		public static void Init()
		{
			float size = 70;
			solids = new Solid3D[1];
			solids[0] = Solid3D.GetCube(size, -size / 2, -size / 2, 0);
			solidsEn = solids.AsEnumerable();
			selected = new List<Solid3D>();
			selectedPoints = new List<Point3D>();

		}
		public static void AddSolid(PointF mouse, bool cylinder = false)
		{
			Solid3D[] solidsTemp = new Solid3D[solids.Length + 1];
			Array.Copy(solids, solidsTemp, solids.Length);
			if (!cylinder)
			{
				mouse.X = 10.0f * ((int)mouse.X / 10);
				mouse.Y = 10.0f * ((int)mouse.Y / 10);
				solidsTemp[solids.Length] = Solid3D.GetCube(70, mouse.X, mouse.Y);
			}
			else
			{
				mouse.X = 10.0f * ((int)mouse.X / 10);
				mouse.Y = 10.0f * ((int)mouse.Y / 10);
				solidsTemp[solids.Length] = Solid3D.GetCylinder(65, 100, mouse.X, mouse.Y);
			}

			solids = new Solid3D[solids.Length + 1];
			Array.Copy(solidsTemp, solids, solidsTemp.Length);
			solidsEn = solids.AsEnumerable();
		}
		private static string dump = "";
		public static string GetDump()
		{
			return dump;
		}
		public static void Draw(Graphics gr)
		{
			float d = Camera3D.GetDistance();
			IDictionary<Solid3D, float> rankedSolids = new Dictionary<Solid3D, float>();
			dump = "";
			//middle = -1;
			for (int i = 0; i < solids.Length; i++)
			{
				if (selected.Contains(solids[i]))
				{
					solids[i].SetFillColor(Brushes.Red);
				}
				else
				{
					solids[i].SetFillColor(Brushes.White);
				}
				rankedSolids.Add(solids[i], solids[i].CameraDistance());
			}
			foreach(Point3D point in selectedPoints)
			{
				PointF p = point.ToPointF(d);
				gr.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 5, 5);
			}
			IOrderedEnumerable<KeyValuePair<Solid3D, float>> ordered = rankedSolids.OrderBy(distance => distance.Value);
			KeyValuePair<Solid3D, float>[] arr = ordered.ToArray();
			Array.Reverse(arr);
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i].Key.Draw(gr);
			}
		}


		public static void RotateAboutVector(float theta)
		{

		}
		public static void Apply(Matrix3D m)
		{
			for (int i = 0; i < solids.Length; i++)
			{
				if (i != selection)
				{
					solids[i].SetFillColor(Brushes.White);
					solids[i].Apply(m);
				}

			}
		}
		public static void ApplySelection(Matrix3D m)
		{
			foreach (Solid3D solid in selected)
			{
				solid.Apply(m);
			}
			if (selectedPoints.Count != 0)
			{
				Point3D[] selectP = selectedPoints.ToArray();
				m.TransformPoints(selectP);
			}

		}

		public static bool IsNothingSelected()
		{
			return (selected.Count == 0 && selectedPoints.Count==0);
		}
		public static Point3D GetCenter()
		{
			Point3D[] centers = new Point3D[solids.Length];
			for (int i = 0; i < solids.Length; i++)
			{
				centers[i] = solids[i].GetCenter();
			}
			return Point3D.GetCenter(centers);

		}
		public static Point3D GetSelectionCenter()
		{
			Point3D[] centers = new Point3D[selected.Count];
			for (int i = 0; i < selected.Count; i++)
			{
				centers[i] = selected.ElementAt(i).GetCenter();
			}
			if (selected.Count == 0)
			{
				return new Point3D();
			}
			else
			{
				return Point3D.GetCenter(centers);
			}

		}
		public static bool MouseAboveSolid(PointF mouse, bool changeSelected = false)
		{
			bool flag = false;
			solidsEn.OrderBy(solid => solid.CameraDistance());
			for (int i = 0; i < solids.Length; i++)
			{
				if (null != solidsEn.ElementAt(i).GetSelectedFace(mouse))
				{
					if (!changeSelected)
					{
						selected.Add(solidsEn.ElementAt(i));
					}
					else
					{
						selected.Clear();
						selected.Add(solidsEn.ElementAt(i));
					}

					flag = true;
					break;
				}
				else
				{
				}

			}
			return flag;
		}
		public static bool MouseAbovePoint(PointF mouse, bool changeSelected = false)
		{
			bool flag = false;
			for (int i = 0; i < solids.Length; i++)
			{
				if (solids[i].GetSelectedPoint(mouse) != new Point3D(-1, -1, -1))
				{
					if (!changeSelected)
					{
						selectedPoints.Add(solids[i].GetSelectedPoint(mouse));
					}
					else
					{
						selectedPoints.Clear();
						selectedPoints.Add(solids[i].GetSelectedPoint(mouse));
					}

					flag = true;
					break;
				}
				else
				{
				}

			}
			return flag;
		}
		public static void SetSelection(int val)
		{
			selected.Clear();
			selection = val;
		}
		public static void DeleteSelection()
		{
			IEnumerable<Solid3D> en = selected.AsEnumerable();
			using (var sequenceNum = en.GetEnumerator())
			{
				while (sequenceNum.MoveNext())
				{
					solids = solids.Where(val => val != sequenceNum.Current).ToArray();

				}
			}
			SetSelection(-1);
		}
	}
}
