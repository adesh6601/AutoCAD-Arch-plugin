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

	public Project CurrentProject;
	public ProjectFile[] CurrentProjectFiles;
	public StringCollection xRefs;
	public Dictionary<string, List<string>> LevelsAndDivisions;



	public void OpenProject(string ProjectPath)
	{
		ProjectBaseServices projectBaseServices = ProjectBaseServices.Service;
		ProjectBaseManager projectManager = projectBaseServices.ProjectManager;
		CurrentProject = projectManager.OpenProject(OpenMode.ForRead, ProjectPath);
	}

	public void GetProjectFiles()
	{
		CurrentProjectFiles = CurrentProject.GetConstructs();
	}

	public void GetLevelsAndDivisions(string FilePath)
	{
		if (!File.Exists(FilePath))
		{
			return;
		}

		using (XmlReader reader = XmlReader.Create(FilePath))
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

	public void GetXRef(ProjectFile File)
	{
		string drawingFullPath = File.DrawingFullPath;


	}
}