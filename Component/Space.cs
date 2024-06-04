using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace Component
{
	public class Space : ACADObject
	{
		public Space() : base() { }

		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public List<Wall> Walls { get; set; }
		public List<List<List<double>>> Surfaces { get; set; }
		public List<List<List<double>>> TranslatedSurfaces { get; set; }

		public Handle Handle { get; set; }
		public string HandleId { get; set; }

		public Handle StyleHandle { get; set; }
		public string StyleId { get; set; }
	}
}
