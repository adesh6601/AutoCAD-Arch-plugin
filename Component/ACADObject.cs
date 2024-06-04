using System.Collections.Generic;

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

		public Autodesk.AutoCAD.DatabaseServices.Handle MaterialHandle { get; set; }
		public string MaterialId { get; set; }

		public Autodesk.AutoCAD.Colors.Color Color { get; set; }
	}
}
