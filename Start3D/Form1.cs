using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Data;
using System.Drawing;
using System.Linq;
using D3.Matrix3DLib;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using D3.Camera3DLib;

namespace Start3D
{
	public partial class Form1 : Form
	{
		
		private float xStart, yStart;
		public Form1()
		{
			InitializeComponent();
			Scene.Init();
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
		
			xStart = e.X;
			
			yStart = e.Y;
			if (e.Button == MouseButtons.Left && creatingSolid)
			{
				Scene.AddSolid(new PointF(e.Location.X - xOffset, -(e.Location.Y - yOffset)));
				creatingSolid = false;
			}
			else if (e.Button == MouseButtons.Left && creatingCylinder)
			{
				Scene.AddSolid(new PointF(e.Location.X - xOffset, -(e.Location.Y - yOffset)), true);
				creatingCylinder = false;
			}
			else if (e.Button == MouseButtons.Right && (ModifierKeys == Keys.None) && !qDown)
			{

				if (Scene.MouseAboveSolid(new PointF(e.Location.X - xOffset, -(e.Location.Y - yOffset))))
				{

				}
				else
				{
					Scene.SetSelection(-1);
				}

			}
			else if (e.Button == MouseButtons.Right && qDown)
			{
				Scene.MouseAboveFace(new PointF(e.Location.X - xOffset, -(e.Location.Y - yOffset)));
			}
			if (e.Button == MouseButtons.Right && (ModifierKeys == Keys.Shift))
			{
				Scene.MouseAboveSolid(new PointF(e.Location.X - xOffset, -(e.Location.Y - yOffset)), true);
			}

			Invalidate();
		}
		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			float dx = e.X - xStart;
			float dy = e.Y - yStart;
			this.xStart = e.X;
			this.yStart = e.Y;


			if (e.Button == MouseButtons.Left && (ModifierKeys == Keys.Control) && !creatingSolid)
			{
				if (Scene.IsNothingSelected() && !qDown)
				{
					Point3D c = Scene.GetCenter();
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);

					m.Multiply(Matrix3D.XRotate(dy / 100));
					m.Multiply(Matrix3D.YRotate(dx / 100));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));

					Scene.Apply(m);
				}
				else
				{
					Point3D c = Scene.GetSelectionCenter();
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);

					m.Multiply(Matrix3D.XRotate(dy / 100));
					m.Multiply(Matrix3D.YRotate(dx / 100));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));

					Scene.ApplySelection(m);
				}

			}
			else if (e.Button == MouseButtons.Middle && (ModifierKeys == Keys.Control) && !creatingSolid)
			{

				if (Scene.IsNothingSelected() && !qDown)
				{
					Point3D c = Scene.GetCenter();
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);

					m.Multiply(Matrix3D.Translate(dx, -dy, 0));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
					Scene.Apply(m);
				}
				else
				{
					Point3D c = Scene.GetSelectionCenter();
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);

					m.Multiply(Matrix3D.Translate(dx, -dy, 0));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
					Scene.ApplySelection(m);
				}

			}
			else if (e.Button == MouseButtons.Right && (ModifierKeys == Keys.Control) && !creatingSolid)
			{
				if (Scene.IsNothingSelected() && !qDown)
				{
					Point3D c = Scene.GetCenter();
					PointF mouse = new PointF(e.X - xOffset, -yOffset + e.Y);
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);
					float val = (float)(Math.Sqrt((dx / 100.0f) * (dx / 100.0f) + (dy / 100.0f) * (dy / 100.0f)));
					if (dx > 0)
					{
						val = -val;
					}

					m.Multiply(Matrix3D.ZRotate(val));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
					Scene.Apply(m);
				}
				else
				{
					Point3D c = Scene.GetSelectionCenter();
					PointF mouse = new PointF(e.X - xOffset, -yOffset + e.Y);
					Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);
					float val = (float)(Math.Sqrt((dx / 100.0f) * (dx / 100.0f) + (dy / 100.0f) * (dy / 100.0f)));
					if (dx > 0)
					{
						val = -val;
					}

					m.Multiply(Matrix3D.ZRotate(val));
					m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
					Scene.ApplySelection(m);
				}

			}
			//m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
			//Matrix3D m2 = Matrix3D.Translate(dx, -dy, 0);
			////Scene.Apply(m);
			//Scene.ApplySelection(m2);
			Invalidate();
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Graphics gr = e.Graphics;
			textBox1.Text = Scene.Dump();
			gr.SmoothingMode = SmoothingMode.AntiAlias;
			int numOfCells = 500;
			float cellSize = 10;
			for (int y = 0; y < numOfCells; ++y)
			{
				gr.DrawLine(Pens.LightGray, 0, y * cellSize, numOfCells * cellSize, y * cellSize);
			}

			for (int x = 0; x < numOfCells; ++x)
			{
				gr.DrawLine(Pens.LightGray, x * cellSize, 0, x * cellSize, numOfCells * cellSize);
			}



			Matrix m = new Matrix();
			m.Scale(1, -1);
			m.Translate(xOffset, yOffset, MatrixOrder.Append); ;
			gr.Transform = m;
			Scene.Draw(gr);
		}
		private float xOffset = 300;
		private float yOffset = 300;
		private bool qDown = false;
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == Keys.Right)
			{
				xOffset += 10.0f;

			}
			else if (e.KeyCode == Keys.Left)
			{
				xOffset -= 10.0f;
			}
			else if (e.KeyCode == Keys.Up)
			{
				yOffset -= 10.0f;
			}
			else if (e.KeyCode == Keys.Down)
			{
				yOffset += 10.0f;
			}
			else if (e.KeyCode == Keys.L)
			{
				Scene.RotateAboutVector(0.5f);
			}
			else if (e.KeyCode == Keys.Delete)
			{
				Scene.DeleteSelection();
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Q)
			{
				this.qDown = !this.qDown;
			}
			Invalidate();
		}
		private bool creatingSolid = false;
		private bool creatingCylinder = false;
		private void button1_Click(object sender, EventArgs e)
		{
			creatingSolid = true;

		}

		private void button2_Click(object sender, EventArgs e)
		{
			creatingCylinder = true;
		}

		private void Form1_DragDrop(object sender, DragEventArgs e)
		{

		}



		private void Form1_MouseWheel(object sender, MouseEventArgs e)
		{

			double r = (e.Delta / 120.0);
			r *= (int)(Math.PI / 3.0);
			//Point3D c = Scene.GetCenter();
			//Matrix3D m = Matrix3D.Translate(-c.X, -c.Y, -c.Z);

			float theta = (float)r;
			Camera3D.ChangeDistance(e.Delta);
			//m.Multiply(Matrix3D.ZRotate(theta));
			//m.Multiply(Matrix3D.Translate(c.X, c.Y, c.Z));
			//Scene.Apply(m);
			Invalidate();


		}
	}
}
