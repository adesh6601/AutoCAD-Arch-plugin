using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component
{
	public class Opening
	{
		public Opening() { }
		public string BlockId { get; set; }
		public string ObjectId { get; set; }
		public string BlockName { get; set; }
		public string Name { get; set; }
		public List<Component.Point> Bounds { get; set; }

		public Component.Point StartPoint { get; set; }
		public Component.Point EndPoint { get; set; }


		public double Area { get; set; }
		public double Height { get; set; }
		public double Width { get; set; }

		/*public double Thickness { get; set; }*/

		public Component.Point Normal { get; set; }
		public string DisplayName { get; set; }

		public string HandleId { get; set; }
		public string Layer { get; set; }

		public double Offset { get; set; }
		public string LayerId { get; set; }

		public string WallId { get; set; }

		public string CollisionType { get; set; }

		public string ShapeType { get; set; }

		public string LineTypeID { get; set; }
		public Autodesk.AutoCAD.DatabaseServices.Handle Handle { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle MaterialHandle { get; set; }
		//public Autodesk.AutoCAD.DatabaseServices.Handle StyleHandle { get; set; }
		public string MaterialId { get; set; }
		//public string StyleID { get; set; }

		public Autodesk.AutoCAD.Colors.Color Color { get; set; }


	}
}
