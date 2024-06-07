using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.Aec.Project;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Collection;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

public class Reader
{
	public List<CurtainWallLayout> CurtainWalls = new List<CurtainWallLayout>();
	public List<Door> Doors = new List<Door>();
	public List<Opening> Openings = new List<Opening>();
	public List<Wall> Walls = new List<Wall>();
	public List<Window> Windows = new List<Window>();
	public List<WindowAssembly> WindowAssemblies = new List<WindowAssembly>();

	public List<Autodesk.AutoCAD.DatabaseServices.BlockReference> BlockReferences = new List<Autodesk.AutoCAD.DatabaseServices.BlockReference>();
	public List<MultiViewBlockReference> MultiViewBlockReferences = new List<MultiViewBlockReference>();

	public List<Space> Spaces = new List<Space>();
	public List<Zone> Zones = new List<Zone>();

	public Dictionary<string, List<string>> Positions = new Dictionary<string, List<string>>();

	public Dictionary<string, Material> Materials = new Dictionary<string, Material>();

	public Dictionary<string, CurtainWallLayoutStyle> CurtainWallLayoutStyles = new Dictionary<string, CurtainWallLayoutStyle>();
	public Dictionary<string, DoorStyle> DoorStyles = new Dictionary<string, DoorStyle>();
	public Dictionary<string, WallStyle> WallStyles = new Dictionary<string, WallStyle>();
	public Dictionary<string, WindowStyle> WindowStyles = new Dictionary<string, WindowStyle>();
	public Dictionary<string, WindowAssemblyStyle> WindowAssemblyStyles = new Dictionary<string, WindowAssemblyStyle>();

	public Project Project;
	public ProjectFile[] ProjectFiles;

	public Document Document;
	public dynamic OpenedDoc;
	public Database Database;

	public Transaction Txn;
	public BlockTableRecord BlockTableRecord;
	public StringCollection xRefs = new StringCollection();
	public Dictionary<string, HashSet<string>> DivisionsAndLevels = new Dictionary<string, HashSet<string>>();

	[CommandMethod("Read Entities")]
	public void ReadEntities()
	{
		OpenProject("C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2024\\Sample Project.apj");
		SetProjectFiles();

		foreach (ProjectFile projectFile in ProjectFiles)
		{
			if (projectFile.Name == "")
			{
				continue;
			}

			DivisionsAndLevels.Clear();
			SetDivisionsAndLevels(projectFile.FileFullPath);
			CheckDivisionsAndLevels();

			xRefs.Clear();
			SetXRefs(projectFile);

			OpenFileInApp(projectFile);

			SetDatabase();
			SetTransaction();
			SetBlockTableRecorde();

			foreach(Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in BlockTableRecord)
			{
				object entity = GetEntity(objectId);
				if (entity == null)
				{
					continue;
				}

				string entityType = GetEntityType(entity);
				if (entityType == null || entityType == "")
				{
					continue;
				}

				AddEntityToList(entity, entityType);
				AddEntityPositionToDict(entity, entityType);
				AddEntityMaterialToDict(entityType);
				AddEntityStyleToDict(entityType);
			}
			ResetTransaction();
			CloseFileInApp();
		}
		CheckPositions();
		CheckCounts();
	}

	public Entities GetEntities()
	{
        Entities entities = new Entities();

		entities.CurtainWalls = CurtainWalls;
		entities.Doors = Doors; ;
		entities.Openings = Openings;
		entities.Walls = Walls;
		entities.Windows = Windows;
		entities.WindowAssemblies = WindowAssemblies;

		entities.BlockReferences = BlockReferences;
		entities.MultiViewBlockReferences = MultiViewBlockReferences;

		entities.Zones = Zones;
		entities.Spaces = Spaces;

		entities.Positions = Positions;

		entities.Materials = Materials;

		entities.CurtainWallLayoutStyles = CurtainWallLayoutStyles;
		entities.DoorStyles = DoorStyles;
		entities.WallStyles = WallStyles;
		entities.WindowStyles = WindowStyles;
		entities.WindowAssemblyStyles = WindowAssemblyStyles;

		return entities;
}

	public void OpenProject(string projectPath)
	{
		ProjectBaseServices projectBaseServices = ProjectBaseServices.Service;
		ProjectBaseManager projectManager = projectBaseServices.ProjectManager;
		Project = projectManager.OpenProject(OpenMode.ForRead, projectPath);
	}

	public void SetProjectFiles()
	{
		ProjectFiles = Project.GetConstructs();
	}

	public void SetDivisionsAndLevels(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return;
		}

		using (XmlReader reader = XmlReader.Create(filePath))
		{
			while (reader.Read())
			{
				if (reader.NodeType != XmlNodeType.Element || reader.Name != "Cell")
				{
					continue;
				}
				
				string division = reader.GetAttribute("Division");
				string level = reader.GetAttribute("Level");

				if (!DivisionsAndLevels.ContainsKey(division))
				{
					DivisionsAndLevels[division] = new HashSet<string>();
				}
				DivisionsAndLevels[division].Add(level);
			}
		}
	}

	public void SetXRefs(ProjectFile file)
	{
		if (file == null || !file.DwgExists)
		{
			return;
		}

		string dwgFullPath = file.DrawingFullPath;
		Database dwgDatabase = GetDbForFile(dwgFullPath);

		if (dwgDatabase == null)
		{
			return;
		}

		Autodesk.AutoCAD.DatabaseServices.ObjectId symTbId = dwgDatabase.BlockTableId;
		using (Transaction trans = dwgDatabase.TransactionManager.StartTransaction())
		{
			BlockTable bt = trans.GetObject(symTbId, OpenMode.ForRead, false) as BlockTable;
			foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId recId in bt)
			{
				BlockTableRecord btr = trans.GetObject(recId, OpenMode.ForRead) as BlockTableRecord;
				if (!btr.IsFromExternalReference)
					continue;

				xRefs.Add(btr.PathName);
			}
		}
	}

	public Database GetDbForFile(string dwgFullPath)
	{
		DocumentCollection docs = Application.DocumentManager;
		Document doc = null;

		foreach (Document elem in docs)
		{
			if (IsSamePath(elem.Database.Filename, dwgFullPath))
			{
				doc = elem;
				break;
			}
		}

		if (doc != null)
		{
			return doc.Database;
		}

		Database db = new Database(false, true);
		db.ReadDwgFile(dwgFullPath, FileShare.Read, false, null);
		db.ResolveXrefs(false, true);
		return db;
	}

	public bool IsSamePath(string path1, string path2)
	{
		return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
	}

	public void OpenFileInApp(ProjectFile file)
	{
		string dwgFullPath = file.DrawingFullPath;

		try
		{
			dynamic acadApp = Marshal.GetActiveObject("AutoCAD.Application");
			if (System.IO.File.Exists(dwgFullPath))
			{
				OpenedDoc = acadApp.Documents.Open(dwgFullPath);
				SetDocument(file);
			}
		}
		catch (Autodesk.AutoCAD.Runtime.Exception ex)
		{
			Console.WriteLine($"Error opening DWG file: {ex.Message}");
		}
	}

	public void SetDocument(ProjectFile file)
	{
		string dwgFullPath = file.DrawingFullPath;

		DocumentCollection documentManager = Application.DocumentManager;
		foreach (Document document in documentManager)
		{
			if (document.Name != dwgFullPath)
			{
				continue;
			}

			Document = document;
			return;
		}
	}

	public void SetDatabase()
	{
		Database = Document.Database;
	}

	public void SetTransaction()
	{
		Txn = Database.TransactionManager.StartTransaction();
	}

	public void SetBlockTableRecorde()
	{
		BlockTableRecord = Txn.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(Database), OpenMode.ForRead) as BlockTableRecord;
	}

	public dynamic GetEntity(Autodesk.AutoCAD.DatabaseServices.ObjectId objectId)
	{
		return Txn.GetObject(objectId, OpenMode.ForRead);
	}

	public string GetEntityType(object entity)
	{
		if (entity is Wall) return "wall";

		if (entity is CurtainWallLayout)	return "curtainWallLayout";

		if (entity is Window) return "window";
		
		if (entity is WindowAssembly) return "windowAssembly";

		if (entity is Door) return "door";

		if (entity is Opening) return "opening";

		if (entity is Space) return "space";

		if (entity is MultiViewBlockReference) return "multiViewBlockReference";

		if (entity is Autodesk.AutoCAD.DatabaseServices.BlockReference) return "blockReference";

		if (entity is Zone) return "zone";

		return null;
	}

	public void AddEntityToList(object entity, string entityType)
	{
		if(entityType == "wall")
		{
			Walls.Add((Wall)entity);
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWalls.Add((CurtainWallLayout)entity);
			return;
		}

		if (entityType == "window")
		{
			Windows.Add((Window)entity);
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssemblies.Add((WindowAssembly)entity);
			return;
		}

		if (entityType == "door")
		{
			Doors.Add((Door)entity);
			return;
		}

		if (entityType == "opening")
		{
			Openings.Add((Opening)entity);
			return;
		}

		if (entityType == "space")
		{
			Spaces.Add((Space)entity);
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			MultiViewBlockReferences.Add((MultiViewBlockReference)entity);
			return;
		}

		if (entityType == "blockReference")
		{
			BlockReferences.Add((Autodesk.AutoCAD.DatabaseServices.BlockReference)entity);
			return;
		}

		if (entityType == "zone")
		{
			Zones.Add((Zone)entity);
			return;
		}
	}

	public void AddEntityPositionToDict(object entity, string entityType)
	{
		if (entityType == "wall")
		{
			Wall wall = (Wall)entity;
			string handleId = wall.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWallLayout curtainWall = (CurtainWallLayout)entity;
			string handleId = curtainWall.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "window")
		{
			Window window = (Window)entity;
			string handleId = window.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssembly windowAssembly = (WindowAssembly)entity;
			string handleId = windowAssembly.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "door")
		{
			Door door = (Door)entity;
			string handleId = door.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "opening")
		{
			Opening opening = (Opening)entity;
			string handleId = opening.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "space")
		{
			Space space = (Space)entity;
			string handleId = space.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			MultiViewBlockReference multiViewBlockReference = (MultiViewBlockReference)entity;
			string handleId = multiViewBlockReference.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "blockReference")
		{
			Autodesk.AutoCAD.DatabaseServices.BlockReference blockReference = (Autodesk.AutoCAD.DatabaseServices.BlockReference)entity;
			string handleId = blockReference.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}

		if (entityType == "zone")
		{
			Zone zone = (Zone)entity;
			string handleId = zone.Handle.ToString();

			AddDivisionAndLevelToDict(handleId);
			return;
		}
	}

	public void AddDivisionAndLevelToDict(string handleId)
	{
		if (!Positions.ContainsKey(handleId))
		{
			Positions[handleId] = new List<string>();
		}

		foreach (string div in DivisionsAndLevels.Keys)
		{
			foreach (string lvl in DivisionsAndLevels[div])
			{
				Positions[handleId].Add(div + "." + lvl);
			}
		}
	}

	public void AddEntityMaterialToDict(string entityType)
	{
		if (entityType == "wall")
		{
			Material material = Txn.GetObject(Walls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Walls.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			Material material = Txn.GetObject(CurtainWalls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = CurtainWalls.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "window")
		{
			Material material = Txn.GetObject(Windows.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Windows.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "windowAssembly")
		{
			Material material = Txn.GetObject(WindowAssemblies.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = WindowAssemblies.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "door")
		{
			Material material = Txn.GetObject(Doors.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Doors.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "opening")
		{
			Material material = Txn.GetObject(Openings.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Openings.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "space")
		{
			Material material = Txn.GetObject(Spaces.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Spaces.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			Material material = Txn.GetObject(MultiViewBlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = MultiViewBlockReferences.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "blockReference")
		{
			Material material = Txn.GetObject(BlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = BlockReferences.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}

		if (entityType == "zone")
		{
			Material material = Txn.GetObject(Zones.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Zones.Last().MaterialId.Handle.ToString();

			Materials[materialId] = material;
			return;
		}
	}

	public void AddEntityStyleToDict(string entityType)
	{
		if (entityType == "wall")
		{
			WallStyle wallStyle = Txn.GetObject(Walls.Last().StyleId, OpenMode.ForRead) as WallStyle;
			string wallStyleId = Walls.Last().StyleId.Handle.ToString();

			WallStyles[wallStyleId] = wallStyle;
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWallLayoutStyle curtainWallLayoutStyle = Txn.GetObject(CurtainWalls.Last().StyleId, OpenMode.ForRead) as CurtainWallLayoutStyle;
			string CurtainWallLayoutStyleId = CurtainWalls.Last().StyleId.Handle.ToString();

			CurtainWallLayoutStyles[CurtainWallLayoutStyleId] = curtainWallLayoutStyle;
			return;
		}

		if (entityType == "window")
		{
			WindowStyle windowStyle = Txn.GetObject(Windows.Last().StyleId, OpenMode.ForRead) as WindowStyle;
			string windowStyleId = Windows.Last().StyleId.Handle.ToString();

			WindowStyles[windowStyleId] = windowStyle;
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssemblyStyle windowAssemblyStyle = Txn.GetObject(WindowAssemblies.Last().StyleId, OpenMode.ForRead) as WindowAssemblyStyle;
			string windowAssemblyStyleId = WindowAssemblies.Last().StyleId.Handle.ToString();

			WindowAssemblyStyles[windowAssemblyStyleId] = windowAssemblyStyle;
			return;
		}

		if (entityType == "door")
		{
			DoorStyle doorStyle = Txn.GetObject(Doors.Last().StyleId, OpenMode.ForRead) as DoorStyle;
			string doorStyleId = Doors.Last().StyleId.Handle.ToString();

			DoorStyles[doorStyleId] = doorStyle;
			return;
		}
	}

	public void ResetTransaction()
	{
		Txn.Commit();
	}

	public void CloseFileInApp()
	{
		OpenedDoc.Close();
	}

	public void CheckCounts()
	{
		string filePath = "counts.txt";

		string text = "BlockReferences - " + BlockReferences.Count() +
						"\nCurtainWalls - " + CurtainWalls.Count() +
						"\nDoors - " + Doors.Count() +
						"\nMultiViewBlockReferences - " + MultiViewBlockReferences.Count() +
						"\nOpenings - " + Openings.Count() +
						"\nSpaces - " + Spaces.Count() +
						"\nWalls - " + Walls.Count() +
						"\nWindows - " + Windows.Count() +
						"\nWindowAssembly - " + WindowAssemblies.Count() +
						"\nZones - " + Zones.Count() +
						"\nMaterialsDictionary - " + Materials.Count() +
						"\nCurtainWallLayoutStyles " + CurtainWallLayoutStyles.Count() +
						"\nDoorStyles " + DoorStyles.Count() +
						"\nWallStyles " + WallStyles.Count() +
						"\nWindowStyles " + WindowStyles.Count() +
						"\nWindowAssemblyStyles " + WindowAssemblyStyles.Count();
		WriteTextToFile(filePath, text);
	}

	public void CheckPositions()
	{
		string filePath = "positions.txt";

		using (StreamWriter sw = File.AppendText(filePath))
		{
			foreach (string handleId in Positions.Keys)
			{
				foreach (string divlvl in Positions[handleId])
				{
					sw.WriteLine(handleId + " - " + divlvl);
				}
			}
		}
	}

	public void CheckDivisionsAndLevels()
	{
		string filePath = "lvlsdiv.txt";

		using (StreamWriter sw = File.AppendText(filePath))
		{
			foreach (string div in DivisionsAndLevels.Keys)
			{
				foreach (string lvl in DivisionsAndLevels[div])
				{
					sw.WriteLine(div + " - " + lvl);
				}
			}
		}		
	}

	static void WriteTextToFile(string filePath, string text)
	{
		try
		{
			using (StreamWriter writer = new StreamWriter(filePath))
			{
				writer.WriteLine(text);
			}
			Console.WriteLine("Text written to file successfully.");
		}
		catch (System.Exception ex)
		{
			Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
		}
	}
}