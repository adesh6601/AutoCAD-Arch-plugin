using System.Collections.Generic;
using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Collection
{
	public class Entities
	{
		public List<CurtainWallLayout> CurtainWalls { get; set; } = new List<CurtainWallLayout>();
		public List<Door> Doors { get; set; } = new List<Door>();
		public List<Opening> Openings { get; set; } = new List<Opening>();
		public List<Wall> Walls { get; set; } = new List<Wall>();
		public List<Window> Windows { get; set; } = new List<Window>();
		public List<WindowAssembly> WindowAssembly { get; set; } = new List<WindowAssembly>();

		public List<Autodesk.AutoCAD.DatabaseServices.BlockReference> BlockReferences { get; set; } = new List<Autodesk.AutoCAD.DatabaseServices.BlockReference>();
		public List<MultiViewBlockReference> MultiViewBlockReferences { get; set; } = new List<MultiViewBlockReference>();

		public List<Zone> Zones { get; set; } = new List<Zone>();
		public List<Space> Spaces { get; set; } = new List<Space>();

		public Dictionary<string, Material> Materials { get; set; } = new Dictionary<string, Material>();

		public Dictionary<string, CurtainWallLayoutStyle> CurtainWallLayoutStyles { get; set; } = new Dictionary<string, CurtainWallLayoutStyle>();
		public Dictionary<string, DoorStyle> DoorStyles { get; set; } = new Dictionary<string, DoorStyle>();
		public Dictionary<string, OpeningEndcapStyle> OpeningStyles { get; set; } = new Dictionary<string, OpeningEndcapStyle>();
		public Dictionary<string, WallStyle> WallStyles { get; set; } = new Dictionary<string, WallStyle>();
		public Dictionary<string, WindowStyle> WindowStyles { get; set; } = new Dictionary<string, WindowStyle>();
		public Dictionary<string, WindowAssemblyStyle> WindowAssemblyStyles { get; set; } = new Dictionary<string, WindowAssemblyStyle>();
	}
}
