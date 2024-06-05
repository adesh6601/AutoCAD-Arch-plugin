using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace Component
{
	public class ACADObject
	{
		public string DisplayName { get; set; }

		public string BlockName { get; set; }
		public string BlockId { get; set; }

		public string ObjectId { get; set; }

		public List<Point> Bounds { get; set; } = new List<Point>();

		public string Layer {  get; set; }
		public string LayerId { get; set; }

		public Handle MaterialHandle { get; set; }
		public string MaterialId { get; set; }

		public Color Color { get; set; }
	}
}
