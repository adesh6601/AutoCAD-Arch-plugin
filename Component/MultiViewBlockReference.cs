﻿namespace Component
{
    public class MultiViewBlockReference : ACADObject
    {
        public MultiViewBlockReference() : base() { }

		public double Length { get; set; }
		public double Width { get; set; }
		public double BaseHeight { get; set; }
		public double Area { get; set; }

		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }

		public double Rotation { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle Handle { get; set; }
		public string HandleId { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle StyleHandle { get; set; }
		public string StyleID { get; set; }
	}
}
