﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Collection;
using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.Aec.Project;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

public class Reader
{
	public List<CurtainWallLayout> CurtainWalls = new List<CurtainWallLayout>();
	public List<Door> Doors = new List<Door>();
	public List<Opening> Openings = new List<Opening>();
	public List<Wall> Walls = new List<Wall>();
	public List<Window> Windows = new List<Window>();
	public List<WindowAssembly> WindowAssembly = new List<WindowAssembly>();

	public List<Autodesk.AutoCAD.DatabaseServices.BlockReference> BlockReferences = new List<Autodesk.AutoCAD.DatabaseServices.BlockReference>();
	public List<MultiViewBlockReference> MultiViewBlockReferences = new List<MultiViewBlockReference>();

	public List<Zone> Zones = new List<Zone>();
	public List<Space> Spaces = new List<Space>();

	public Dictionary<string, Material> MaterialsDictionary = new Dictionary<string, Material>();

	public Project Project;
	public ProjectFile[] ProjectFiles;

	public Document Document;
	public dynamic OpenedDoc;
	public Database Database;

	public Transaction Txn;
	public BlockTableRecord BlockTableRecord;
	public StringCollection xRefs = new StringCollection();
	public Dictionary<string, List<string>> LevelsAndDivisions = new Dictionary<string, List<string>>();

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

			LevelsAndDivisions.Clear();
			SetLevelsAndDivisions(projectFile.FileFullPath);

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
				AddEntityMaterialToDict(entityType);
			}

			ResetTransaction();
			CloseFileInApp();
		}

		CheckCounts();
		string stop = "stop";
	}

	public ModelData GetModelData()
	{
		ModelData modelData = new ModelData();

		modelData.CurtainWalls = CurtainWalls;
		modelData.Doors = Doors; ;
		modelData.Openings = Openings;
		modelData.Walls = Walls;
		modelData.Windows = Windows;
		modelData.WindowAssembly = WindowAssembly;

		modelData.BlockReferences = BlockReferences;
		modelData.MultiViewBlockReferences = MultiViewBlockReferences;

		modelData.Zones = Zones;
		modelData.Spaces = Spaces;

		modelData.MaterialsDictionary = MaterialsDictionary;

		return modelData;
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

	public void SetLevelsAndDivisions(string filePath)
	{
		if (!System.IO.File.Exists(filePath))
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
				
				string level = reader.GetAttribute("Level");
				string division = reader.GetAttribute("Division");

				if (!LevelsAndDivisions.ContainsKey(division))
				{
					LevelsAndDivisions.Add(division, new List<string>());
				}

				List<string> levelsList = LevelsAndDivisions[division];
				
				if (!levelsList.Contains(level))
				{
					continue;
				}
				levelsList.Add(level);
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

	public void ResetTransaction()
	{
		Txn.Commit();
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
			WindowAssembly.Add((WindowAssembly)entity);
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

	public void AddEntityMaterialToDict(string entityType)
	{
		if (entityType == "wall")
		{
			Material material = Txn.GetObject(Walls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Walls.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			Material material = Txn.GetObject(CurtainWalls.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = CurtainWalls.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "window")
		{
			Material material = Txn.GetObject(Windows.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Windows.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "windowAssembly")
		{
			Material material = Txn.GetObject(WindowAssembly.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = WindowAssembly.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "door")
		{
			Material material = Txn.GetObject(Doors.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Doors.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "opening")
		{
			Material material = Txn.GetObject(Openings.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Openings.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "space")
		{
			Material material = Txn.GetObject(Spaces.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Spaces.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			Material material = Txn.GetObject(MultiViewBlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = MultiViewBlockReferences.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "blockReference")
		{
			Material material = Txn.GetObject(BlockReferences.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = BlockReferences.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "zone")
		{
			Material material = Txn.GetObject(Zones.Last().MaterialId, OpenMode.ForRead) as Material;
			string materialId = Zones.Last().MaterialId.Handle.ToString();

			if (!MaterialsDictionary.ContainsKey(materialId))
			{
				MaterialsDictionary[materialId] = material;
			}
			return;
		}
	}

	public void CloseFileInApp()
	{
		OpenedDoc.Close();
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
						"\nWindowAssembly - " + WindowAssembly.Count() +
						"\nZones - " + Zones.Count() +
						"\nmaterialsDictionary - " + MaterialsDictionary.Count();
		WriteTextToFile(filePath, text);
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