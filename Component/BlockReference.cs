namespace Component
{
    public class BlockReference : ACADObject
    {
        public BlockReference() : base() { }

		public double Length { get; set; }
		public double Width { get; set; }
		public double BaseHeight { get; set; }
		public double Area { get; set; }

		public Point Position { get; set; }
		public double Rotation { get; set; }

		public Point ScaleFactor { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle Handle { get; set; }
		public string HandleId { get; set; }
	}
}
