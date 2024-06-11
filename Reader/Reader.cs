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
using Collection;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

public class Reader
{
	public ACADEntities ACADEntities = new ACADEntities();
	public EntitiesConvertor Convertor = new EntitiesConvertor();

	public Project Project;
	public ProjectFile[] ProjectFiles;

	public Document Document;
	public dynamic OpenedDoc;
	public Database Database;

	public Transaction Txn;
	public BlockTableRecord BlockTableRecord;
	public StringCollection xRefs = new StringCollection();
	public Dictionary<string, HashSet<string>> DivisionsAndLevels = new Dictionary<string, HashSet<string>>();

	public void ReadEntities(string projectPath, Entities entities)
	{
		OpenProject(projectPath);
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
				AddEntityPositionToDict(entity, entityType, entities);
				AddEntityMaterialToDict(entityType);
				AddEntityStyleToDict(entityType);
			}

			Convertor.Convert(ACADEntities, entities);
			ACADEntities.Clear();

			ResetTransaction();
			CloseFileInApp();
		}
		CheckPositions(entities);
		CheckCounts(entities);
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
			ACADEntities.Walls.Add((Wall)entity);
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			ACADEntities.CurtainWalls.Add((CurtainWallLayout)entity);
			return;
		}

		if (entityType == "window")
		{
			ACADEntities.Windows.Add((Window)entity);
			return;
		}

		if (entityType == "windowAssembly")
		{
			ACADEntities.WindowAssemblies.Add((WindowAssembly)entity);
			return;
		}

		if (entityType == "door")
		{
			ACADEntities.Doors.Add((Door)entity);
			return;
		}

		if (entityType == "opening")
		{
			ACADEntities.Openings.Add((Opening)entity);
			return;
		}

		if (entityType == "space")
		{
			ACADEntities.Spaces.Add((Space)entity);
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			ACADEntities.MultiViewBlockReferences.Add((MultiViewBlockReference)entity);
			return;
		}

		if (entityType == "blockReference")
		{
			ACADEntities.BlockReferences.Add((Autodesk.AutoCAD.DatabaseServices.BlockReference)entity);
			return;
		}

		if (entityType == "zone")
		{
			ACADEntities.Zones.Add((Zone)entity);
			return;
		}
	}

	public void AddEntityPositionToDict(object entity, string entityType, Entities entities)
	{
		if (entityType == "wall")
		{
			Wall wall = (Wall)entity;
			string handleId = wall.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWallLayout curtainWall = (CurtainWallLayout)entity;
			string handleId = curtainWall.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "window")
		{
			Window window = (Window)entity;
			string handleId = window.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssembly windowAssembly = (WindowAssembly)entity;
			string handleId = windowAssembly.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "door")
		{
			Door door = (Door)entity;
			string handleId = door.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "opening")
		{
			Opening opening = (Opening)entity;
			string handleId = opening.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "space")
		{
			Space space = (Space)entity;
			string handleId = space.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			MultiViewBlockReference multiViewBlockReference = (MultiViewBlockReference)entity;
			string handleId = multiViewBlockReference.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "blockReference")
		{
			Autodesk.AutoCAD.DatabaseServices.BlockReference blockReference = (Autodesk.AutoCAD.DatabaseServices.BlockReference)entity;
			string handleId = blockReference.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}

		if (entityType == "zone")
		{
			Zone zone = (Zone)entity;
			string handleId = zone.Handle.ToString();

			AddDivisionAndLevelToDict(handleId, entities);
			return;
		}
	}

	public void AddDivisionAndLevelToDict(string handleId, Entities entities)
	{
		if (!entities.Positions.ContainsKey(handleId))
		{
			entities.Positions[handleId] = new List<string>();
		}

		foreach (string div in DivisionsAndLevels.Keys)
		{
			foreach (string lvl in DivisionsAndLevels[div])
			{
				entities.Positions[handleId].Add(div + "." + lvl);
			}
		}
	}

	public void AddEntityMaterialToDict(string entityType)
	{
		if (entityType == "wall")
		{
			Material material = Txn.GetObject(ACADEntities.Walls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Walls.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			Material material = Txn.GetObject(ACADEntities.CurtainWalls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.CurtainWalls.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "window")
		{
			Material material = Txn.GetObject(ACADEntities.Windows.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Windows.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "windowAssembly")
		{
			Material material = Txn.GetObject(ACADEntities.WindowAssemblies.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.WindowAssemblies.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "door")
		{
			Material material = Txn.GetObject(ACADEntities.Doors.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Doors.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "opening")
		{
			Material material = Txn.GetObject(ACADEntities.Openings.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Openings.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "space")
		{
			Material material = Txn.GetObject(ACADEntities.Spaces.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Spaces.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			Material material = Txn.GetObject(ACADEntities.MultiViewBlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.MultiViewBlockReferences.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "blockReference")
		{
			Material material = Txn.GetObject(ACADEntities.BlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.BlockReferences.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}

		if (entityType == "zone")
		{
			Material material = Txn.GetObject(ACADEntities.Zones.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = ACADEntities.Zones.Last().MaterialId.Handle.ToString();

			ACADEntities.Materials[materialId] = material;
			return;
		}
	}

	public void AddEntityStyleToDict(string entityType)
	{
		if (entityType == "wall")
		{
			WallStyle wallStyle = Txn.GetObject(ACADEntities.Walls.Last().StyleId, OpenMode.ForRead) as WallStyle;
			string wallStyleId = ACADEntities.Walls.Last().StyleId.Handle.ToString();

			ACADEntities.WallStyles[wallStyleId] = wallStyle;
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWallLayoutStyle curtainWallLayoutStyle = Txn.GetObject(ACADEntities.CurtainWalls.Last().StyleId, OpenMode.ForRead) as CurtainWallLayoutStyle;
			string CurtainWallLayoutStyleId = ACADEntities.CurtainWalls.Last().StyleId.Handle.ToString();

			ACADEntities.CurtainWallLayoutStyles[CurtainWallLayoutStyleId] = curtainWallLayoutStyle;
			return;
		}

		if (entityType == "window")
		{
			WindowStyle windowStyle = Txn.GetObject(ACADEntities.Windows.Last().StyleId, OpenMode.ForRead) as WindowStyle;
			string windowStyleId = ACADEntities.Windows.Last().StyleId.Handle.ToString();

			ACADEntities.WindowStyles[windowStyleId] = windowStyle;
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssemblyStyle windowAssemblyStyle = Txn.GetObject(ACADEntities.WindowAssemblies.Last().StyleId, OpenMode.ForRead) as WindowAssemblyStyle;
			string windowAssemblyStyleId = ACADEntities.WindowAssemblies.Last().StyleId.Handle.ToString();

			ACADEntities.WindowAssemblyStyles[windowAssemblyStyleId] = windowAssemblyStyle;
			return;
		}

		if (entityType == "door")
		{
			DoorStyle doorStyle = Txn.GetObject(ACADEntities.Doors.Last().StyleId, OpenMode.ForRead) as DoorStyle;
			string doorStyleId = ACADEntities.Doors.Last().StyleId.Handle.ToString();

			ACADEntities.DoorStyles[doorStyleId] = doorStyle;
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

	public void CheckCounts(Entities entities)
	{
		string filePath = "counts.txt";

		string text = "CurtainWalls - " + entities.CurtainWalls.Count() +
						"\nDoors - " + entities.Doors.Count() +
						"\nOpenings - " + entities.Openings.Count() +
						"\nWalls - " + entities.Walls.Count() +
						"\nWindows - " + entities.Windows.Count() +
						"\nWindowAssembly - " + entities.WindowAssemblies.Count() +
						"\n\nBlockReferences - " + entities.BlockReferences.Count() +
						"\nMultiViewBlockReferences - " + entities.MultiViewBlockReferences.Count() +
						"\n\nSpaces - " + entities.Spaces.Count() +
						"\nZones - " + entities.Zones.Count();

		WriteTextToFile(filePath, text);
	}

	public void CheckPositions(Entities entities)
	{
		string filePath = "positions.txt";

		using (StreamWriter sw = File.AppendText(filePath))
		{
			foreach (string handleId in entities.Positions.Keys)
			{
				foreach (string divlvl in entities.Positions[handleId])
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