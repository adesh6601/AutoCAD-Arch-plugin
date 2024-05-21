using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.Arch.DatabaseServices;

namespace Collection
{
	public class File
	{
		public File() { }

		public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Wall> WallsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Wall>();
		public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Window> WindowsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Window>();
		public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Door> DoorsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Door>();
		public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Space> SpacesDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Space>();
		public static Dictionary<int, Autodesk.Aec.DatabaseServices.MultiViewBlockReference> MultiViewBlockReferenceDictionary = new Dictionary<int, Autodesk.Aec.DatabaseServices.MultiViewBlockReference>();
		public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Zone> ZonesDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Zone>();

		public static Dictionary<string, BuildingComponents> ElementsInProjectNevigator = new Dictionary<string, BuildingComponents>();
		public static Dictionary<string, BuildingComponents> ConstructsInProjectNevigator = new Dictionary<string, BuildingComponents>();

		public static List<Autodesk.Aec.Arch.DatabaseServices.Wall> walls = new List<Autodesk.Aec.Arch.DatabaseServices.Wall>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout> curtainWallLayout = new List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.Window> windows = new List<Autodesk.Aec.Arch.DatabaseServices.Window>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly> windowAssembly = new List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.Door> doors = new List<Autodesk.Aec.Arch.DatabaseServices.Door>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.Opening> openings = new List<Autodesk.Aec.Arch.DatabaseServices.Opening>();

		public static List<Autodesk.Aec.Arch.DatabaseServices.Space> spaces = new List<Autodesk.Aec.Arch.DatabaseServices.Space>();
		public static List<Autodesk.AutoCAD.DatabaseServices.BlockReference> BlockReferences = new List<Autodesk.AutoCAD.DatabaseServices.BlockReference>();
		public static List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference> multiViewBlockReferences = new List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference>();
		public static List<Autodesk.Aec.Arch.DatabaseServices.Zone> zones = new List<Autodesk.Aec.Arch.DatabaseServices.Zone>();

		public static int wallCount = 0;
		public static int windowCount = 0;
		public static int spaceCount = 0;
		public static int doorCount = 0;
		public static int multiViewBlockReferenceCount = 0;
		public static int zoneCount = 0;

		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle> wallStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle>();
		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle> curtainWallStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle>();
		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle> windowStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle>();
		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle> windowAssemblyStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle>();
		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle> doorStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle>();
		public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.OpeningEndcapStyle> openingStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.OpeningEndcapStyle>();

		public static Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialsDictionary = new Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material>();

	}
}
