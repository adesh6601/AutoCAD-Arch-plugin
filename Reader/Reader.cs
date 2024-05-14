using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.Project;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Component;

public class Reader
{
	public static Dictionary<string, List<Dictionary<string, object>>> allDataDictionary = new Dictionary<string, List<Dictionary<string, object>>>();
	public static Dictionary<string, Dictionary<string, List<Dictionary<string, object>>>> layerDictionary = new Dictionary<string, Dictionary<string, List<Dictionary<string, object>>>>();
	public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Wall> wallsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Wall>();
	public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Window> windowsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Window>();
	public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Door> doorsDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Door>();
	public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Space> spacesDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Space>();
	public static Dictionary<int, Autodesk.Aec.DatabaseServices.MultiViewBlockReference> multiViewBlockReferenceDictionary = new Dictionary<int, Autodesk.Aec.DatabaseServices.MultiViewBlockReference>();
	public static Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Zone> zonesDictionary = new Dictionary<int, Autodesk.Aec.Arch.DatabaseServices.Zone>();

	public static Dictionary<String, Collection.DataFormatter.Layer> floorsJson = new Dictionary<String, Collection.DataFormatter.Layer>();
	public static Dictionary<String, Dictionary<String, Collection.DataFormatter.Layer>> floors = new Dictionary<String, Dictionary<String, Collection.DataFormatter.Layer>>();
	public static List<Collection.DataFormatter.Layer> floorsList = new List<Collection.DataFormatter.Layer>();


	public static Dictionary<string, Collection.BuildingComponents> Elements = new Dictionary<string, Collection.BuildingComponents>();
	public static Dictionary<string, Collection.BuildingComponents> ElementsInProjectNevigator = new Dictionary<string, Collection.BuildingComponents>();
	public static Dictionary<string, Collection.BuildingComponents> ConstructsInProjectNevigator = new Dictionary<string, Collection.BuildingComponents>();

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
	public static int count = 0;

	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle> wallStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle>();
	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle> curtainWallStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle>();
	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle> windowStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle>();
	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle> windowAssemblyStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle>();
	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle> doorStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle>();
	public static Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.OpeningEndcapStyle> openingStylesDictionary = new Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.OpeningEndcapStyle>();

	public static Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialsDictionary = new Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material>();

	public static string projectFilePath = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2024\\Sample Project.apj";
	private ProjectBaseManager mgr = ProjectBaseServices.Service.ProjectManager;
	private static Project proj = null;
	private static Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

}