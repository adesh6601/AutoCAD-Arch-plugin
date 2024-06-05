using Autodesk.AutoCAD.DatabaseServices;

namespace Component
{
    public class MultiViewBlockReference : ACADObject
    {
		public double Length { get; set; }
		public double Width { get; set; }
		public double BaseHeight { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public double Rotation { get; set; }

		public Handle Handle { get; set; }
		public string HandleId { get; set; }

		public Handle StyleHandle { get; set; }
		public string StyleId { get; set; }


		public MultiViewBlockReference() : base() { }
	}
}
