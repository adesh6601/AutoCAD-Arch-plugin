using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Aec.Project;
using Autodesk.AutoCAD.EditorInput;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Component;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Specialized;
using System.IO;
using Collection;
using System.Xml;
using Autodesk.AutoCAD.ApplicationServices;
using System.Runtime.InteropServices;
using Autodesk.Aec.Arch.DatabaseServices;

public class Reader
{
	public Reader() { }

	public List<Autodesk.Aec.DatabaseServices.BlockReference> BlockReferences = new List<Autodesk.Aec.DatabaseServices.BlockReference>();
	public List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout> CurtainWalls = new List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Door> Doors = new List<Autodesk.Aec.Arch.DatabaseServices.Door>();
	public List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference> MultiViewBlockReferences = new List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Opening> Openings = new List<Autodesk.Aec.Arch.DatabaseServices.Opening>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Space> Spaces = new List<Autodesk.Aec.Arch.DatabaseServices.Space>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Wall> Walls = new List<Autodesk.Aec.Arch.DatabaseServices.Wall>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Window> Windows = new List<Autodesk.Aec.Arch.DatabaseServices.Window>();
	public List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly> WindowAssembly = new List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly>();
	public List<Autodesk.Aec.Arch.DatabaseServices.Zone> Zones = new List<Autodesk.Aec.Arch.DatabaseServices.Zone>();

	public Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialsDictionary = new Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material>();



	public Project Project;
	public ProjectFile[] ProjectFiles;

	public Document Document;
	dynamic OpenedDoc;
	public Database Database;

	public Transaction Txn;
	public BlockTableRecord BlockTableRecord;
	public StringCollection xRefs;
	public Dictionary<string, List<string>> LevelsAndDivisions;



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
		xRefs.Clear();

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

	public void CloseFileInApp()
	{
		OpenedDoc.Close();
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

	public string GetEntityType(dynamic entity)
	{
		if (entity is Autodesk.Aec.Arch.DatabaseServices.Wall) return "wall";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout)	return "curtainWallLayout";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.Window) return "window";
		
		if (entity is Autodesk.Aec.Arch.DatabaseServices.WindowAssembly) return "windowAssembly";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.Door) return "door";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.Opening) return "opening";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.Space) return "space";

		if (entity is Autodesk.Aec.DatabaseServices.MultiViewBlockReference) return "multiViewBlockReference";

		if (entity is Autodesk.AutoCAD.DatabaseServices.BlockReference) return "blockReference";

		if (entity is Autodesk.Aec.Arch.DatabaseServices.Zone) return "zone";

		return null;
	}

	public void AddEntityToList(object entity, string entityType)
	{
		if(entityType == "wall")
		{
			Walls.Add((Autodesk.Aec.Arch.DatabaseServices.Wall)entity);
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			CurtainWalls.Add((Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout)entity);
			return;
		}

		if (entityType == "window")
		{
			Windows.Add((Autodesk.Aec.Arch.DatabaseServices.Window)entity);
			return;
		}

		if (entityType == "windowAssembly")
		{
			WindowAssembly.Add((Autodesk.Aec.Arch.DatabaseServices.WindowAssembly)entity);
			return;
		}

		if (entityType == "door")
		{
			Doors.Add((Autodesk.Aec.Arch.DatabaseServices.Door)entity);
			return;
		}

		if (entityType == "opening")
		{
			Openings.Add((Autodesk.Aec.Arch.DatabaseServices.Opening)entity);
			return;
		}

		if (entityType == "space")
		{
			Spaces.Add((Autodesk.Aec.Arch.DatabaseServices.Space)entity);
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			MultiViewBlockReferences.Add((Autodesk.Aec.DatabaseServices.MultiViewBlockReference)entity);
			return;
		}

		if (entityType == "blockReference")
		{
			BlockReferences.Add((Autodesk.Aec.DatabaseServices.BlockReference)entity);
			return;
		}

		if (entityType == "zone")
		{
			Zones.Add((Autodesk.Aec.Arch.DatabaseServices.Zone)entity);
			return;
		}
	}

	public void AddEntityMaterialToDict(string entityType)
	{
		if (entityType == "wall")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Walls.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Walls.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "curtainWallLayout")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(CurtainWalls.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = CurtainWalls.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "window")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Windows.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Windows.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "windowAssembly")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(WindowAssembly.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = WindowAssembly.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "door")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Doors.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Doors.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "opening")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Openings.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Openings.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "space")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Spaces.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Spaces.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "multiViewBlockReference")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(MultiViewBlockReferences.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = MultiViewBlockReferences.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "blockReference")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(BlockReferences.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = BlockReferences.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
		}

		if (entityType == "zone")
		{
			Autodesk.AutoCAD.DatabaseServices.Material material = Txn.GetObject(Zones.Last().MaterialId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Material;
			string materialId = Zones.Last().MaterialId.Handle.ToString();

			if (!materialsDictionary.ContainsKey(materialId))
			{
				materialsDictionary[materialId] = material;
			}
			return;
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


}