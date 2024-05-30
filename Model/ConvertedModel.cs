using System.Collections.Generic;
using Component;

namespace Model
{
	public class ConvertedModel
	{
		public List<CurtainWall> CurtainWalls { get; set; } = new List<CurtainWall>();
		public List<Door> Doors { get; set; } = new List<Door>();
		public List<Opening> Openings { get; set; } = new List<Opening>();
		public List<Wall> Walls { get; set; } = new List<Wall>();
		public List<Window> Windows { get; set; } = new List<Window>();
		public List<DoorWindowAssembly> WindowAssembly { get; set; } = new List<DoorWindowAssembly>();

		public List<BlockReference> BlockReferences { get; set; } = new List<BlockReference>();
		public List<MultiViewBlockReference> MultiViewBlockReferences { get; set; } = new List<MultiViewBlockReference>();

		public List<Zone> Zones { get; set; } = new List<Zone>();
		public List<Space> Spaces { get; set; } = new List<Space>();
	}
}
