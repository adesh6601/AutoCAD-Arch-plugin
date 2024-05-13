using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component
{
	public class Wall
	{
		public Wall() { }

		public string BlockId { get; set; }
		public string ObjectId { get; set; }
		public string BlockName { get; set; }
		public List<Component.Point> Bounds { get; set; }

		public Component.Point StartPoint { get; set; }
		public Component.Point EndPoint { get; set; }


		public double Area { get; set; }
		public double BaseHeight { get; set; }
		public double Width { get; set; }
		public double Length { get; set; }

		public double Rotation { get; set; }
		public string DisplayName { get; set; }

		public string Name { get; set; }
		public string Layer { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle ObjectHandle { get; set; }
		public string ObjectHandleId { get; set; }

		public string LayerId { get; set; }

		public string CollisionType { get; set; }

		public Autodesk.AutoCAD.DatabaseServices.Handle MaterialHandle { get; set; }
		public Autodesk.AutoCAD.DatabaseServices.Handle StyleHandle { get; set; }
		public string MaterialId { get; set; }
		public string StyleID { get; set; }

		public Autodesk.AutoCAD.Colors.Color Color { get; set; }

		//public Autodesk.AutoCAD.DatabaseServices.Material Material { get; set; }

		//public MaterialOpacityComponent Opacity { get; set; }
		public double Translucence { get; set; }
		public double SelfIllumination { get; set; }
		public double Reflectivity { get; set; }
		public double ColorBleedScale { get; set; }
		public double IndirectBumpScale { get; set; }
		public double ReflectanceScale { get; set; }
		public double TransmittanceScale { get; set; }
		public bool TwoSided { get; set; }

		public double Luminance { get; set; }
		public string Style { get; set; }

		public string Description { get; set; }

		public string MaterialName { get; set; }

		public MaterialColor Ambient { get; set; }

		// public MaterialDiffuseComponent Diffuse { get; set; }   
		//public Autodesk.Aec.Arch.DatabaseServices.WallStyle Style { get; set; }
	}
}
