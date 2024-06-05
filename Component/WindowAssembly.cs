﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;

namespace Component
{
	public class WindowAssembly : ACADObject
	{
		public int CellCount { get; set; }
		public string Description { get; set; }
		public string WallId { get; set; }

		public double Length { get; set; }
		public double Height { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public Point Normal { get; set; }
		public double Offset { get; set; }

		public string CollisionType { get; set; }

		public Handle Handle { get; set; }
		public string HandleId { get; set; }

		public Handle StyleHandle { get; set; }
		public string Style { get; set; }
		public string StyleId { get; set; }

		public string MaterialName { get; set; }

		public MaterialColor Ambient { get; set; }
		public double ColorBleedScale { get; set; }
		public double IndirectBumpScale { get; set; }
		public double Luminance { get; set; }
		public double ReflectanceScale { get; set; }
		public double Reflectivity { get; set; }
		public double SelfIllumination { get; set; }
		public double Translucence { get; set; }
		public double TransmittanceScale { get; set; }
		public bool TwoSided { get; set; }


		public WindowAssembly() : base() { }
	}
}
