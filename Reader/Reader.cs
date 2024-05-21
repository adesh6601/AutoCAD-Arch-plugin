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

public class Reader
{
	public Reader() { }

	public List<Component.BlockReference> BlockReferences = new List<Component.BlockReference>();
	public List<Component.CurtainWall> CurtainWalls = new List<Component.CurtainWall>();
	public List<Component.Door> Doors = new List<Component.Door>();
	public List<Component.DoorwindowAssembly> DoorwindowAssemblies = new List<Component.DoorwindowAssembly>();
	public List<Component.MultiViewBlockReference> MultiViewBlockReferences = new List<Component.MultiViewBlockReference>();
	public List<Component.Opening> Openings = new List<Component.Opening>();
	public List<Component.Space> Spaces = new List<Component.Space>();
	public List<Component.Wall> Walls = new List<Component.Wall>();
	public List<Component.Window> Windows = new List<Component.Window>();
	public List<Component.Zone> Zones = new List<Component.Zone>();

	public Project Project;
	public ProjectFile[] ProjectFiles;
	public Document Document;

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

	public void GetLevelsAndDivisions(string filePath)
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

	public void GetXRefs(ProjectFile file)
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
				dynamic openedDoc = acadApp.Documents.Open(dwgFullPath);
				SetDocument(file);
			}
		}
		catch (Autodesk.AutoCAD.Runtime.Exception ex)
		{
			Console.WriteLine($"Error opening DWG file: {ex.Message}");
		}
	}

	//public 




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