using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Component
{
    public class BlockReference
    {
        public BlockReference() { }

		public string BlockId { get; set; }
		public string ObjectId { get; set; }
		public string BlockName { get; set; }

		public string BlockUnit { get; set; }

		public List<Component.Point> Bounds { get; set; }

		public Component.Point Position { get; set; }

		public Component.Point ScaleFactor { get; set; }

		public double Rotation { get; set; }

		public double Area { get; set; }
		public double BaseHeight { get; set; }
		public double Width { get; set; }
		public double Length { get; set; }
		public string DisplayName { get; set; }

		public string Name { get; set; }
		public string Layer { get; set; }

		public string HandleId { get; set; }
		public Autodesk.AutoCAD.DatabaseServices.Handle Handle { get; set; }
		public string LayerId { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle MaterialHandle { get; set; }
		public string MaterialId { get; set; }


		public Autodesk.AutoCAD.Colors.Color Color { get; set; }
	}
}
