using System;
using System.IO;
using System.Collections.Generic;
/*using Autodesk.GeometryPrimitives.Geometry;*/
using Newtonsoft.Json.Linq;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Aec.Arch.DatabaseServices;
/*using static Component.DataFormatter;
using Autodesk.AutoCAD.Windows;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;*/
/*using Autodesk.DataExchange.DataModels;*/



namespace Collection
{
	public class DataFormatter
	{
		public string unit { get; set; }
		public Dictionary<string, Grid> grids { get; set; }
		public string selectedLayer { get; set; }
		public string simulationType { get; set; }
		public Groups groups { get; set; }
		public int width { get; set; }
		public int height { get; set; }
		public Meta meta { get; set; }
		public Guides guides { get; set; }
		public AirsideSystem airsideSystem { get; set; }
		public BuildingDesign buildingDesign { get; set; }
		public int pixelsPerUnit { get; set; }
		public string displayUnit { get; set; }

		public string level { get; set; }

		public string division { get; set; }

		public string buildingTemplate;

		Dictionary<int, Component.Wall> linesDictionary = new Dictionary<int, Component.Wall>();
		Dictionary<int, Component.Window> windows = new Dictionary<int, Component.Window>();
		Dictionary<int, Component.Door> doors = new Dictionary<int, Component.Door>();
		Dictionary<int, Component.Space> spacesDictionary = new Dictionary<int, Component.Space>();
		Dictionary<int, Component.Zone> zonesDictionary = new Dictionary<int, Component.Zone>();

		List<List<List<double>>> listOfWalls = new List<List<List<double>>>();
		Dictionary<string, bool> uniqueIds = new Dictionary<string, bool>();
		Dictionary<string, Vertex> vertices = new Dictionary<string, Vertex>();
		Dictionary<string, Line2D> lines = new Dictionary<string, Line2D>();
		Dictionary<string, Areas> areas = new Dictionary<string, Areas>();
		Dictionary<string, Holes> holes = new Dictionary<string, Holes>();
		Dictionary<string, Component.Zone> zones = new Dictionary<string, Component.Zone>();
		public Dictionary<string, Layer> layers = new Dictionary<string, Layer>();
		Dictionary<int, Point> uniqueVertices = new Dictionary<int, Point>();

		public DataFormatter()
		{

		}

		public DataFormatter(Dictionary<int, Component.Wall> linesDictionary, Dictionary<int, Component.Window> windows, Dictionary<int, Component.Door> doors, Dictionary<int, Component.Space> spacesDictionary, Dictionary<int, Component.Zone> zonesDictionary, List<List<List<double>>> listOfWalls)
		{
			this.linesDictionary = linesDictionary;
			this.windows = windows;
			this.doors = doors;
			this.listOfWalls = listOfWalls;
			this.spacesDictionary = spacesDictionary;
			this.zonesDictionary = zonesDictionary;
			UniqueVertices();
			GetLinesDictionary();
			MakeHole();
			makeZones();
			MakeLayer();
			foreach (var hole in holes.Values)
			{
				string vertex1Id = lines[hole.line].vertices[0];
				string vertex2Id = lines[hole.line].vertices[1];
				foreach (var area in areas.Values)
				{
					int flag1 = 0;
					int flag2 = 0;
					foreach (var vertex in area.vertices)
					{
						if (vertex1Id == vertex)
						{
							flag1 = 1;
						}
						if (vertex2Id == vertex)
						{
							flag2 = 1;
						}
						if (flag1 == 1 && flag2 == 1)
						{
							hole.space = area.id;
							break;
						}
					}
				}
			}
			componenets();
		}
		public void componenets()
		{
			unit = "ft";
			grids = new Dictionary<string, Grid>();
			var h1 = new Grid();
			h1.id = "h1";
			h1.type = "horizontal-streak";
			h1.properties = new GridProperties();
			h1.properties.step = 20;
			h1.properties.colors = new List<string>() { "#000", "#ddd", "#ddd", "#ddd", "#ddd" };
			grids.Add(h1.id, h1);

			var v1 = new Grid();
			v1.id = "v1";
			v1.type = "vertical-streak";
			v1.properties = new GridProperties();
			v1.properties.step = 20;
			v1.properties.colors = new List<string>() { "#000", "#ddd", "#ddd", "#ddd", "#ddd" };
			grids.Add(v1.id, v1);

			selectedLayer = "layer-1";

			simulationType = "engineeringSimulation";
			buildingTemplate = "office";

			groups = new Groups();
			width = 6000;
			height = 6000;
			meta = new Meta();
			guides = new Guides();
			guides.horizontal = new Horizontal();
			guides.vertical = new Vertical();
			guides.circular = new Circular();
			airsideSystem = new AirsideSystem();

			buildingDesign = new BuildingDesign();
			buildingDesign.building_details = new BuildingDetails();
			buildingDesign.building_details.wall_height = 9;
			buildingDesign.building_details.wall_height_unit = "ft";
			buildingDesign.building_details.buildingLevel = "midFloorMultiStorey";
			buildingDesign.building_details.surroundingFloorAirConditions = new SurroundingFloorAirConditions();
			buildingDesign.building_details.surroundingFloorAirConditions.lowerFloor = true;
			buildingDesign.building_details.surroundingFloorAirConditions.upperFloor = true;


			buildingDesign.material_library = new MaterialLibrary();
			buildingDesign.material_library.Exposed_Wall = new ExposedWall();
			buildingDesign.material_library.Exposed_Wall.uValueUnit = "Btu/(hr-ft²-°F)";
			buildingDesign.material_library.Exposed_Wall.absorptivity = 0.9;
			buildingDesign.material_library.Exposed_Wall.materialAssembly = "Face Brick + 1\" Insulation + 4\" LW Concrete Block";
			buildingDesign.material_library.Exposed_Wall.wallGroup = "D";
			buildingDesign.material_library.Exposed_Wall.thicknessUnit = "in";
			buildingDesign.material_library.Exposed_Wall.total_Thickness = 6.142;
			buildingDesign.material_library.Exposed_Wall.uValue = 0.184;
			buildingDesign.material_library.Exposed_Wall.transmissivity = 0;
			buildingDesign.material_library.Exposed_Wall.colorAdjustmentFactor = 1;

			var PartitionWall = new PartitionWall();
			PartitionWall.materialAssembly = "4″ Brick with Plaster on both side";
			PartitionWall.infoText = "Cement plaster + Brick + Cement plaster";
			PartitionWall.total_Thickness = "5.4133858";
			PartitionWall.uValue = "1.016";
			PartitionWall.transmissivity = "0";
			PartitionWall.absorptivity = "0.9";
			PartitionWall.uValueUnit = "Btu/(hr-ft²-°F)";
			PartitionWall.thicknessUnit = "in";
			buildingDesign.material_library.Partition_Wall = PartitionWall;

			var GlassWall = new GlassWall();
			GlassWall.uValueUnit = "Btu/(hr-ft²-°F)";
			GlassWall.infiltrationThickness = 0.05;
			GlassWall.color = new List<int>() { 171, 170, 175 };
			GlassWall.absorptivity = "0.459";
			GlassWall.shadingCoefficient = 0.647;
			GlassWall.materialAssembly = "Double Glazing – 1/4” Gray Tint Glass with 1/4” Air Space";
			GlassWall.thicknessUnit = "in";
			GlassWall.total_Thickness = "0.75";
			GlassWall.infoText = "Glass + Air + Glass";
			GlassWall.uValue = "0.56";
			GlassWall.transmissivity = "0.479";
			buildingDesign.material_library.Glass_Wall = GlassWall;

			var Floor = new Floor();
			Floor.materialAssembly = "4″ RCC";
			Floor.infoText = "RCC";
			Floor.total_Thickness = "3.93";
			Floor.uValue = "1.452";
			Floor.transmissivity = "0";
			Floor.absorptivity = "0.2";
			Floor.uValueUnit = "Btu/(hr-ft²-°F)";
			Floor.thicknessUnit = "in";
			buildingDesign.material_library.Floor = Floor;

			var Roof = new Roof();
			Roof.uValueUnit = "Btu/(hr-ft²-°F)";
			Roof.f = 1;
			Roof.absorptivity = 0.9;
			Roof.uValueSolarLoad = 0.0883;
			Roof.materialAssembly = "6\" HW Concrete with 2\" Insulation";
			Roof.roofNo = 12;
			Roof.thicknessUnit = "in";
			Roof.total_Thickness = 8.88;
			Roof.infoText = "HW Concrete + Insulation + Felt Membrane + Slag";
			Roof.uValue = 0.1331;
			Roof.transmissivity = 0;
			Roof.colorAdjustmentFactor = 1;
			buildingDesign.material_library.Roof = Roof;

			var Ceiling = new Ceiling();
			Ceiling.uValueUnit = "Btu/(hr-ft²-°F)";
			Ceiling.f = 1;
			Ceiling.absorptivity = 0.9;
			Ceiling.uValueSolarLoad = 0.0883;
			Ceiling.materialAssembly = "6\" HW Concrete with 2\" Insulation";
			Ceiling.roofNo = 12;
			Ceiling.thicknessUnit = "in";
			Ceiling.total_Thickness = 8.88;
			Ceiling.infoText = "HW Concrete + Insulation + Felt Membrane + Slag";
			Ceiling.uValue = 0.1331;
			Ceiling.transmissivity = 0;
			Ceiling.colorAdjustmentFactor = 1;
			buildingDesign.material_library.Ceiling = Ceiling;

			pixelsPerUnit = 20;
			displayUnit = "ft";
		}
		public void makeZones()
		{
			foreach (var zone in zonesDictionary)
			{
				var Spaces = zone.Value.Spaces;
				var Zones = zone.Value.ZoneIds;
				List<string> ids = new List<string>();
				List<Component.Space> spacesList = new List<Component.Space>();
				foreach (var space in Spaces)
				{

					ids.Add(space.ToString().Substring(1, space.ToString().Length - 2));
					//spacesList.Add(spacesDictionary[space.ToString().Substring(1, space.ToString().Length - 2)]);
				}
				List<string> zoneIds = new List<string>();
				foreach (var z in Zones)
				{

					zoneIds.Add(z.ToString().Substring(1, z.ToString().Length - 2));

				}

				Component.Zone zoneObj = new Component.Zone();
				zoneObj.Name = zone.Value.Name;
				zoneObj.DisplayName = zone.Value.DisplayName;
				zoneObj.LayerId = zone.Value.LayerId;
				zoneObj.ObjectId = zone.Value.ObjectId;
				zoneObj.BlockId = zone.Value.BlockId;
				zoneObj.BlockName = zone.Value.BlockName;

				List<Component.Point> bounds = new List<Component.Point>();
				Component.Point maxPoint = new Component.Point(zone.Value.Bounds[1].X, zone.Value.Bounds[1].Y, zone.Value.Bounds[1].Z);
				Component.Point minPoint = new Component.Point(zone.Value.Bounds[0].X, zone.Value.Bounds[0].Y, zone.Value.Bounds[0].Z);
				bounds.Add(minPoint);
				bounds.Add(maxPoint);
				zoneObj.Bounds = bounds;

				Component.Point startPoint = new Component.Point(zone.Value.StartPoint.X, zone.Value.StartPoint.Y, zone.Value.StartPoint.Z);
				zoneObj.StartPoint = startPoint;
				Component.Point endPoint = new Component.Point(zone.Value.EndPoint.X, zone.Value.EndPoint.Y, zone.Value.EndPoint.Z);
				zoneObj.EndPoint = endPoint;
				zoneObj.SpaceIds = ids;
				zoneObj.ZoneIds = zoneIds;
				zoneObj.TotalNumberOfSpaces = zone.Value.TotalNumberOfSpaces;
				zoneObj.TotalNumberOfZones = zone.Value.TotalNumberOfZones;
				zoneObj.Area = zone.Value.Area;
				zoneObj.Layer = zone.Value.Layer;
				zoneObj.Spaces = spacesList;
				zones.Add(zoneObj.ObjectId, zoneObj);
			}


		}
		public bool IsUniquePoint(Point InPoint, Dictionary<int, Point> uniqueVertices)
		{
			foreach (var Point in uniqueVertices)
			{
				if (Math.Abs(Point.Value.Position.X - InPoint.Position.X) <= 0.001 && Math.Abs(Point.Value.Position.Y - InPoint.Position.Y) <= 0.01)
				{ return false; }
			}
			return true;
		}
		public string GenerateId()
		{
			int length = 10;
			string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
			Random random = new Random();
			char[] result = new char[length];
			int maxAttempts = 1000; // Define maximum attempts

			for (int attempt = 0; attempt < maxAttempts; attempt++)
			{
				for (int i = 0; i < length; i++)
				{
					result[i] = characters[random.Next(characters.Length)];
				}

				string randomString = new string(result);
				if (!uniqueIds.ContainsKey(randomString))
				{
					uniqueIds.Add(randomString, true);
					return randomString;
				}
			}

			throw new Exception("Unable to generate a unique ID after maximum attempts.");
		}
		public double GetLength(List<Point> inPoint)
		{
			var length = Math.Pow(Math.Pow((inPoint[1].Position.X - inPoint[0].Position.X), 2) + Math.Pow((inPoint[1].Position.Y - inPoint[0].Position.Y), 2), 0.5);
			return length;
		}
		public Dictionary<string, Vertex> UniqueVertices()
		{
			int IndexOfUniqueVertces = 1;
			int windowIndex = 1;
			int doorIndex = 1;

			using (StreamWriter writer = new StreamWriter("..\\..\\Output\\vertexdata.txt", true))
			{
				foreach (var pair in linesDictionary)
				{
					//bool addIntoUnqiueVertex = false;
					int addUniqueVertex = 1;

					Point startPoint = new Point(pair.Value.StartPoint.X, pair.Value.StartPoint.Y, pair.Value.StartPoint.Z);
					Point endPoint = new Point(pair.Value.EndPoint.X, pair.Value.EndPoint.Y, pair.Value.EndPoint.Z);
					if (IsUniquePoint(startPoint, uniqueVertices) == true && addUniqueVertex == 1)
					{
						uniqueVertices.Add(IndexOfUniqueVertces, startPoint);
						IndexOfUniqueVertces++;
						writer.WriteLine("Start point : " + startPoint.Position.X + " , " + startPoint.Position.Y);
						var startVertex = new Vertex();
						startVertex.id = GenerateId();
						startVertex.type = "";
						startVertex.prototype = "vertices";
						startVertex.name = "Vertex";
						startVertex.misc = new Misc();
						startVertex.selected = false;
						startVertex.properties = new VertexProperties();
						startVertex.visible = true;
						startVertex.tag = new List<object>();
						startVertex.x = pair.Value.StartPoint.X;
						startVertex.y = pair.Value.StartPoint.Y;
						startVertex.lines = new List<string>();
						startVertex.areas = new List<object>();

						vertices.Add(startVertex.id, startVertex);

					}
					if (IsUniquePoint(endPoint, uniqueVertices) == true && addUniqueVertex == 1)
					{

						uniqueVertices.Add(IndexOfUniqueVertces, endPoint);
						IndexOfUniqueVertces++;
						writer.WriteLine("End point : " + endPoint.Position.X + " , " + endPoint.Position.Y);
						var endVertex = new Vertex();
						endVertex.id = GenerateId();
						endVertex.type = "";
						endVertex.prototype = "vertices";
						endVertex.name = "Vertex";
						endVertex.misc = new Misc();
						endVertex.selected = false;
						endVertex.properties = new VertexProperties();
						endVertex.visible = true;
						endVertex.tag = new List<object>();
						endVertex.x = pair.Value.EndPoint.X;
						endVertex.y = pair.Value.EndPoint.Y;
						endVertex.lines = new List<string>();
						endVertex.areas = new List<object>();

						vertices.Add(endVertex.id, endVertex);

					}

				}

			}

			return vertices;
		}
		public static DataFormatter ShiftModelPosition(DataFormatter doc)
		{
			try
			{
				var vertices = doc.layers["layer-1"].vertices;
				var items = doc.layers["layer-1"].items;
				var areas = doc.layers["layer-1"].areas;

				double lowestX = 0;
				double highestY = 0;
				double offsetXAdd = 0;
				double offsetXSub = 0;
				double offsetYAdd = 0;
				double offsetYSub = 0;


				foreach (var vertex in vertices)
				{
					if (vertex.Value.x < lowestX)
					{
						lowestX = vertex.Value.x;
					}

					if (vertex.Value.y > highestY)
					{
						highestY = vertex.Value.y;
					}
				}

				if (lowestX < 0)
				{
					offsetXAdd = Math.Abs(lowestX) + 20;
				}
				else if (lowestX > 20)
				{
					offsetXSub = lowestX - 20;
				}
				else
				{
					offsetXAdd = 20 - lowestX;
				}

				if (highestY < 0)
				{
					offsetYAdd = Math.Abs(highestY) + 280;
				}
				else if (highestY > 280)
				{
					offsetYSub = highestY - 280;
				}
				else
				{
					offsetYAdd = 280 - highestY;
				}

				foreach (var vertex in vertices)
				{
					vertex.Value.x += offsetXAdd;
					vertex.Value.x -= offsetXSub;
					vertex.Value.y += offsetYAdd;
					vertex.Value.y -= offsetYSub;
				}

				foreach (var item in items)
				{
					item.Value.x += offsetXAdd;
					item.Value.x -= offsetXSub;
					item.Value.y += offsetYAdd;
					item.Value.y -= offsetYSub;
				}

				/*foreach (var area in areas)
                {
                    area.Value.fluidPoint["x"] += offsetXAdd;
                    area.Value.fluidPoint["x"] -= offsetXSub;
                    area.Value.fluidPoint["y"] += offsetYAdd;
                    area.Value.fluidPoint["y"] -= offsetYSub;
                }*/

				// doc.designShiftOffset.offsetXAdd = offsetXAdd;
				// doc.designShiftOffset.offsetYAdd = offsetYAdd;
				// doc.designShiftOffset.offsetXSub = offsetXSub;
				// doc.designShiftOffset.offsetYSub = offsetYSub;

				return doc;
			}
			catch (Exception ex)
			{
				throw new Exception("\nError encountered while shifting BIM model position. " + ex.Message);
			}
		}
		public List<string> GetVerticesOfLine(List<Point> inLine)
		{
			List<string> vertexList = new List<string>();
			foreach (var vertex in vertices)
			{
				if (Math.Abs(vertex.Value.x - inLine[0].Position.X) < 0.01 && Math.Abs(vertex.Value.y - inLine[0].Position.Y) < 0.01)
				{
					vertexList.Add(vertex.Key);
					continue;
				}
				if (Math.Abs(vertex.Value.x - inLine[1].Position.X) < 0.01 && Math.Abs(vertex.Value.y - inLine[1].Position.Y) < 0.01)
				{
					vertexList.Add(vertex.Key);
					continue;
				}

			}
			return vertexList;
		}
		public Dictionary<string, Line2D> GetLinesDictionary()
		{
			foreach (var line in linesDictionary)
			{
				var l1 = new Line2D();
				//l1.id = GenerateId();
				l1.id = line.Value.ObjectId.ToString().Substring(1, line.Value.ObjectId.ToString().Length - 2);
				l1.type = "wall";
				l1.prototype = "lines";
				l1.name = "wall";
				l1.misc = new Misc();
				l1.selected = false;
				string newJson = @"{
                        ""type"": ""Exposed_Wall"",
                        ""length"": 300,
                        ""thickness"": 10.236666666666668,
                        ""userSetPartition"": false,
                        ""materialProperties"": {
                            ""uValueUnit"": ""Btu/(hr-ft²-°F)"",
                            ""absorptivity"": 0.9,
                            ""materialAssembly"": ""Face Brick + 1\"" Insulation + 4\"" LW Concrete Block"",
                            ""wallGroup"": ""D"",
                            ""thicknessUnit"": ""in"",
                            ""total_Thickness"": 6.142,
                            ""uValue"": 0.184,
                            ""transmissivity"": 0,
                            ""colorAdjustmentFactor"": 1
                        }
                    }";
				var initialJson = JObject.Parse(newJson);
				initialJson["length"] = line.Value.Length * 20; // Set the desired length value here
				string modifiedJson = initialJson.ToString();
				l1.properties = JObject.Parse(modifiedJson);
				l1.visible = true;
				l1.tag = new List<object>();
				List<Point> points = new List<Point>();
				Point startPoint = new Point(line.Value.StartPoint.X, line.Value.StartPoint.Y, line.Value.StartPoint.Z);
				Point endPoint = new Point(line.Value.EndPoint.X, line.Value.EndPoint.Y, line.Value.EndPoint.Z);
				points.Add(startPoint);
				points.Add(endPoint);
				l1.vertices = GetVerticesOfLine(points);
				l1.holes = new List<string>();
				lines.Add(l1.id, l1);


				foreach (var vertexId in l1.vertices)
				{
					vertices[vertexId].lines.Add(l1.id);
				}
			}


			return lines;
		}
		public void MakeArea(Dictionary<string, Vertex> vertices, int count, string spaceId)
		{
			List<string> listOfVertices = new List<string>();
			Areas area = new Areas();
			//area.id = GenerateId();
			area.id = spaceId;
			area.type = "area";
			area.prototype = "areas";
			area.holes = new List<object> { };
			area.name = "Space " + count;
			area.misc = new Misc();
			string newJson = @"{
                        ""Floor"": {
                            ""name"": ""Floor"",
                            ""unit"": ""sq. ft"",
                            ""type"": ""Interior"",
                            ""materialProperties"": {
                                ""materialAssembly"": ""4″ RCC"",
                                ""infoText"": ""RCC"",
                                ""total_Thickness"": ""3.93"",
                                ""uValue"": ""1.452"",
                                ""transmissivity"": ""0"",
                                ""absorptivity"": ""0.2"",
                                ""uValueUnit"": ""Btu/(hr-ft²-°F)"",
                                ""thicknessUnit"": ""in""
                            },
                            ""area"": 150
                        },
                        ""Ceiling"": {
                            ""name"": ""Ceiling"",
                            ""unit"": ""sq. ft"",
                            ""type"": ""Exterior"",
                            ""materialProperties"": {
                                ""uValueUnit"": ""Btu/(hr-ft²-°F)"",
                                ""f"": 1,
                                ""absorptivity"": 0.9,
                                ""uValueSolarLoad"": 0.0883,
                                ""materialAssembly"": ""6\"" HW Concrete with 2\"" Insulation"",
                                ""roofNo"": 12,
                                ""thicknessUnit"": ""in"",
                                ""total_Thickness"": 8.88,
                                ""infoText"": ""HW Concrete + Insulation + Felt Membrane + Slag"",
                                ""uValue"": 0.1331,
                                ""transmissivity"": 0,
                                ""colorAdjustmentFactor"": 1
                            },
                            ""area"": 150
                        },
                        ""fidelitySimulationInputs"": {
                            ""method"": ""engineeringSimulation"",
                            ""spaceType"": ""office"",
                            ""occupancy"": {
                                ""occupantActivity"": ""Manikin Standing"",
                                ""occupantType"": ""defaultASHRAE"",
                                ""numberOfManikins"": 0,
                                ""manikinType"": {
                                    ""men"": 0,
                                    ""women"": 0,
                                    ""children"": 0
                                },
                                ""previousManikin"": 0,
                                ""occuapntDensity"": 200
                            },
                            ""lighting"": {
                                ""lightingLoadPerArea"": 1.11,
                                ""lightingLoadPerAreaUnit"": ""W/sq.ft""
                            },
                            ""appliances"": {
                                ""latentAppliancesLoad"": 0,
                                ""appliancesLoadPerAreaUnit"": ""W/sq.ft"",
                                ""sensibleAppliancesLoad"": 1
                            }
                        }
                    }";
			//area.properties = JObject.Parse(newJson);
			List<List<double>> verticeOfArea = new List<List<double>>();
			foreach (var vertex in vertices)
			{
				List<double> WallVertices = new List<double>();
				listOfVertices.Add(vertex.Key);
				WallVertices.Add(vertex.Value.x);
				WallVertices.Add(vertex.Value.y);
				verticeOfArea.Add(WallVertices);
			}
			double areaOfPolygoan = PolygonArea(verticeOfArea);
			var initialJson = JObject.Parse(newJson);
			initialJson["Floor"]["area"] = areaOfPolygoan;
			initialJson["Ceiling"]["area"] = areaOfPolygoan;// Set the desired length value here
			string modifiedJson = initialJson.ToString();
			area.properties = JObject.Parse(modifiedJson);
			area.vertices = listOfVertices;
			area.fluidPoint = new Dictionary<string, double>();
			//area.fluidPoint.Add("x", fluidPointArray[0]);
			//area.fluidPoint.Add("y", fluidPointArray[1]);
			area.visible = true;
			area.tag = new List<object>();

			areas.Add(area.id, area);
		}

		/*       public static bool IntersectBoundingBoxes(Extents3d ext1, Extents3d ext2)
			   {

				   // Check for intersection along X axis
				   if (ext1.MaxPoint.X < ext2.MinPoint.X || ext2.MaxPoint.X < ext1.MinPoint.X)
					   return false;

				   // Check for intersection along Y axis
				   if (ext1.MaxPoint.Y < ext2.MinPoint.Y || ext2.MaxPoint.Y < ext1.MinPoint.Y)
					   return false;

				   // If no axis has a gap, the bounding boxes intersect
				   return true;
			   }*/

		public static bool IntersectBoundingBoxes(List<Component.Point> ext1, List<Component.Point> ext2)
		{

			// Check for intersection along X axis
			if (ext1[1].X < ext2[0].X || ext2[1].X < ext1[1].X)
				return false;

			// Check for intersection along Y axis
			if (ext1[0].Y < ext2[0].Y || ext2[1].Y < ext1[1].Y)
				return false;

			// If no axis has a gap, the bounding boxes intersect
			return true;
		}
		public static string Orientation(List<List<double>> vertices)
		{
			int total = 0;
			int n = vertices.Count;
			for (int i = 0; i < n; i++)
			{
				double x1 = vertices[i][0];
				double y1 = vertices[i][1];
				double x2 = vertices[(i + 1) % n][0];
				double y2 = vertices[(i + 1) % n][1];
				total += (int)((x2 - x1) * (y2 + y1));
			}
			if (total > 0)
				return "Counterclockwise";
			else if (total < 0)
				return "Clockwise";
			else
				return "Collinear";
		}

		public static bool isStartingVertex(List<Point> listOfVertices)
		{
			if (listOfVertices[0].Position.X < listOfVertices[1].Position.X)
			{
				return true;
			}
			else if (listOfVertices[0].Position.X > listOfVertices[1].Position.X)
			{

				return false;
			}
			else
			{
				if (listOfVertices[0].Position.Y < listOfVertices[0].Position.Y)
				{
					return true;
				}
				return false;
			}

		}
		public void MakeHole()
		{
			foreach (var window in windows)
			{
				foreach (var wall in linesDictionary)
				{
					if (IntersectBoundingBoxes(window.Value.Bounds, wall.Value.Bounds))
					{
						Point point1 = new Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
						Point point2 = new Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);

						Holes windowHole = new Holes();
						//windowHole.id = GenerateId();
						windowHole.id = window.Value.ObjectId.ToString().Substring(1, window.Value.ObjectId.ToString().Length - 2); ;
						windowHole.type = "window";
						windowHole.prototype = "holes";
						windowHole.name = "window";
						windowHole.misc = new Misc();
						windowHole.selected = false;
						string newJson1 = @"{
                        ""width"": 0.18,
                        ""height"": 0.35,
                        ""altitude"": 2.5,
                        ""thickness"": 10,
                        ""infiltration"": true,
                        ""materialProperties"": {
                            ""uValueUnit"": ""Btu/(hr-ft²-°F)"",
                            ""infiltrationThickness"": 0.05,
                            ""color"": [
                                171,
                                170,
                                175
                            ],
                            ""absorptivity"": ""0.459"",
                            ""shadingCoefficient"": 0.647,
                            ""materialAssembly"": ""Double Glazing – 1/4” Gray Tint Glass with 1/4” Air Space"",
                            ""thicknessUnit"": ""in"",
                            ""total_Thickness"": ""0.75"",
                            ""infoText"": ""Glass + Air + Glass"",
                            ""uValue"": ""0.56"",
                            ""transmissivity"": ""0.479""
                            }
                        }";
						var initialJson = JObject.Parse(newJson1);
						initialJson["altitude"] = window.Value.StartPoint.Z;
						initialJson["width"] = window.Value.Width;
						initialJson["height"] = window.Value.Height;
						string modifiedJson = initialJson.ToString();
						windowHole.properties = JObject.Parse(modifiedJson);
						windowHole.visible = true;
						windowHole.tag = new List<object> { "window", "buildingEssential" };
						Point startWindow = new Point(window.Value.StartPoint.X, window.Value.StartPoint.Y, 0.0);
						List<Point> points = new List<Point>();
						points.Add(startWindow);
						points.Add(point1);
						List<Point> wallLength = new List<Point>();
						wallLength.Add(point2);
						wallLength.Add(point1);
						//windowHole.offset = 0.5123508399999992;
						double offset = GetLength(points) / GetLength(wallLength);
						windowHole.offset = offset;
						foreach (var line in lines)
						{

							if (line.Value.vertices.Count != 0
								&& (Math.Abs(point1.Position.X - vertices[line.Value.vertices[0]].x) < 0.01
								&& Math.Abs(point1.Position.Y - vertices[line.Value.vertices[0]].y) < 0.01)
								&& (Math.Abs(point2.Position.X - vertices[line.Value.vertices[1]].x) < 0.01
								&& Math.Abs(point2.Position.Y - vertices[line.Value.vertices[1]].y) < 0.01))
							{
								windowHole.line = line.Key;
								lines[windowHole.line].holes.Add(windowHole.id);
								break;
							}
							if (line.Value.vertices.Count != 0
								&& (Math.Abs(point1.Position.X - vertices[line.Value.vertices[1]].x) < 0.01
								&& Math.Abs(point1.Position.Y - vertices[line.Value.vertices[1]].y) < 0.01)
								&& (Math.Abs(point2.Position.X - vertices[line.Value.vertices[0]].x) < 0.01
								&& Math.Abs(point2.Position.Y - vertices[line.Value.vertices[0]].y) < 0.01))
							{
								windowHole.line = line.Key;
								lines[windowHole.line].holes.Add(windowHole.id);
								break;
							}
						}
						Normal normal1 = new Normal();
						normal1.x = window.Value.Normal.X;
						normal1.y = window.Value.Normal.Y;
						normal1.z = window.Value.Normal.Z;
						windowHole.normal = normal1;

						holes.Add(windowHole.id, windowHole);
						break;
					}

				}
			}

			foreach (var door in doors)
			{
				foreach (var wall in linesDictionary)
				{
					if (IntersectBoundingBoxes(door.Value.Bounds, wall.Value.Bounds))
					{
						Point point1 = new Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
						Point point2 = new Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);

						Holes hole = new Holes();
						// hole.id = GenerateId();
						hole.id = door.Value.ObjectId.ToString().Substring(1, door.Value.ObjectId.ToString().Length - 2); ;
						hole.type = "door";
						hole.prototype = "holes";
						hole.name = "door";
						hole.misc = new Misc();
						hole.selected = false;
						string newJson = @"{
                        ""width"": 0.18,
                        ""height"": 0.35,
                        ""altitude"": 0,
                        ""thickness"": 0.1016,
                        ""flip"": true,
                        ""infiltration"": true,
                        }";
						var initialJson = JObject.Parse(newJson);
						//initialJson["altitude"] = door.Value.StartPoint.Z;
						initialJson["width"] = door.Value.Width;
						initialJson["height"] = door.Value.Height;
						// Set the desired length value here
						string modifiedJson = initialJson.ToString();
						hole.properties = JObject.Parse(modifiedJson);
						//hole.properties = JObject.Parse(newJson);
						hole.visible = true;
						hole.tag = new List<object> { "door", "buildingEssential" };
						Point startWindow = new Point(door.Value.StartPoint.X, door.Value.StartPoint.Y, 0.0);
						Point endWindow = new Point(door.Value.EndPoint.X, door.Value.EndPoint.Y, 0.0);
						List<Point> points = new List<Point>();
						points.Add(startWindow);
						points.Add(endWindow);

						Point windowStartingVertex = new Point();
						Point wallStartingVertex = new Point();

						if (isStartingVertex(points))
						{
							windowStartingVertex = startWindow;
						}
						else
						{
							windowStartingVertex = endWindow;
						}
						points.Clear();
						points.Add(point1);
						points.Add(point2);

						if (isStartingVertex(points))
						{
							wallStartingVertex = point1;
						}
						else
						{
							wallStartingVertex = point2;
						}
						points.Clear();
						points.Add(wallStartingVertex);
						points.Add(windowStartingVertex);

						List<Point> wallLength = new List<Point>();
						wallLength.Add(point2);
						wallLength.Add(point1);
						// double offset = (GetLength(points) + (door.Value.Width / 2 ) ) / GetLength(wallLength);
						double offset = (GetLength(points)) / GetLength(wallLength);

						hole.offset = offset;


						//hole.offset = 0.5123508399999992;
						foreach (var line in lines)
						{

							if (line.Value.vertices.Count != 0
								&& (Math.Abs(point1.Position.X - vertices[line.Value.vertices[0]].x) < 0.01
								&& Math.Abs(point1.Position.Y - vertices[line.Value.vertices[0]].y) < 0.01)
								&& (Math.Abs(point2.Position.X - vertices[line.Value.vertices[1]].x) < 0.01
								&& Math.Abs(point2.Position.Y - vertices[line.Value.vertices[1]].y) < 0.01))
							{
								hole.line = line.Key;
								lines[hole.line].holes.Add(hole.id);
								break;
							}
							if (line.Value.vertices.Count != 0
								&& (Math.Abs(point1.Position.X - vertices[line.Value.vertices[1]].x) < 0.01
								&& Math.Abs(point1.Position.Y - vertices[line.Value.vertices[1]].y) < 0.01)
								&& (Math.Abs(point2.Position.X - vertices[line.Value.vertices[0]].x) < 0.01
								&& Math.Abs(point2.Position.Y - vertices[line.Value.vertices[0]].y) < 0.01))
							{
								hole.line = line.Key;
								lines[hole.line].holes.Add(hole.id);
								break;
							}
						}
						Normal normal = new Normal();
						normal.x = door.Value.Normal.X;
						normal.y = door.Value.Normal.Y;
						normal.z = door.Value.Normal.Z;
						hole.normal = normal;
						hole.space = "";


						holes.Add(hole.id, hole);
						break;
					}

				}
			}

		}

		public static double PolygonArea(List<List<double>> vertices)
		{
			int n = vertices.Count;
			if (n < 3)
				return 0; // Not a valid polygon

			// Append the first vertex to the end to close the polygon
			vertices.Add(vertices[0]);

			// Calculate the area using the Shoelace formula
			double area = 0;
			for (int i = 0; i < n; i++)
			{
				area += (vertices[i][0] * vertices[i + 1][1]) - (vertices[i + 1][0] * vertices[i][1]);
			}

			// Take the absolute value and divide by 2
			area = Math.Abs(area) / 2;
			return area;
		}
		public static List<double> ComparePoints(List<double> point1, List<double> point2)
		{
			double x1 = point1[0];
			double y1 = point1[1];
			double x2 = point2[0];
			double y2 = point2[1];

			if (x1 < x2)
			{
				return point1;
			}
			else if (x1 > x2)
			{
				return point2;
			}
			else
			{
				if (y1 < y2)
				{
					return point1;
				}
				else if (y1 > y2)
				{
					return point2;
				}
				else
				{
					return point1;
				}
			}
		}

		public static List<double> ComparePoints(List<double> point1, List<double> point2, List<double> result)
		{

			double x1 = point1[0];
			double y1 = point1[1];
			double x2 = point2[0];
			double y2 = point2[1];

			if (x1 < x2)
			{
				result[0] = x1;
			}
			else if (x1 >= x2)
			{
				result[0] = x2;
			}

			if (y1 < y2)
			{
				result[1] = y1;
			}
			else if (y1 >= y2)
			{
				result[1] = y2;
			}
			return result;
		}
		public Layer MakeLayer()
		{
			var layer1 = new Layer();
			layer1.id = "layer-1";
			layer1.altitude = 0;
			layer1.order = 0;
			layer1.opacity = 1;
			layer1.name = "default";
			layer1.visible = true;
			Tuple<double, double> centerOfRotation = Tuple.Create(0.0, 0.0);
			List<Line2D> wallsWithEndpoints = new List<Line2D>();
			Dictionary<string, Vertex> verticesCopyList = new Dictionary<string, Vertex>();
			foreach (var line in this.lines)
			{
				wallsWithEndpoints.Add(line.Value);
			}



			layer1.vertices = vertices;
			layer1.lines = lines;
			layer1.holes = holes;
			layer1.zones = zones;
			List<List<List<double>>> roomAreas = new List<List<List<double>>>();
			List<List<List<double>>> rooms = new List<List<List<double>>>();

			List<string> spaceIdList = new List<string>();

			int count = 101;


			foreach (var kvp in spacesDictionary)
			{
				List<List<double>> room = new List<List<double>>();

				var surfaces = kvp.Value.Surfaces;
				List<double> minPoint = new List<double>();
				minPoint.Add(double.MaxValue);
				minPoint.Add(double.MaxValue);
				List<double> result = new List<double>();
				result.Add(double.MaxValue);
				result.Add(double.MaxValue);
				foreach (var surface in surfaces)
				{
					var wall = surface;
					List<double> startPoint = new List<double>();
					List<double> endPoint = new List<double>();

					startPoint.Add(wall[0][0]);
					startPoint.Add(wall[0][1]);
					endPoint.Add(wall[1][0]);
					endPoint.Add(wall[1][1]);

					minPoint = ComparePoints(minPoint, startPoint, result);
					minPoint = ComparePoints(minPoint, endPoint, result);



				}

				var k = minPoint;
				var dx = kvp.Value.Bounds[0].X - minPoint[0];
				var dy = kvp.Value.Bounds[0].Y - minPoint[1];

				Dictionary<int, Point> uniqueRoomVertices = new Dictionary<int, Point>();
				int index = 0;
				foreach (var surface in surfaces)
				{
					var wall = surface;

					Point startPoint = new Point(wall[0][0] + dx, wall[0][1] + dy, 0.0);
					Point endPoint = new Point(wall[1][0] + dx, wall[1][1] + dy, 0.0);

					if (IsUniquePoint(startPoint, uniqueRoomVertices))
					{
						List<double> points = new List<double>();
						points.Add(wall[0][0] + dx);
						points.Add(wall[0][1] + dy);
						uniqueRoomVertices.Add(index++, startPoint);
						room.Add(points);
					}
					if (IsUniquePoint(endPoint, uniqueRoomVertices))
					{
						List<double> points = new List<double>();
						points.Add(wall[1][0] + dx);
						points.Add(wall[1][1] + dy);
						uniqueRoomVertices.Add(index++, endPoint);
						room.Add(points);
					}
				}
				spaceIdList.Add(kvp.Value.ObjectId.ToString().Substring(1, kvp.Value.ObjectId.ToString().Length - 2));
				rooms.Add(room);


			}
			int spaceCount = 0;
			foreach (var loop in rooms)
			{
				Dictionary<string, Vertex> roomsDictionary = new Dictionary<string, Vertex>();
				foreach (var point in loop)
				{
					foreach (var vertex in vertices)
					{
						if (Math.Abs(vertex.Value.x - point[0]) <= 1.5 && Math.Abs(vertex.Value.y - point[1]) <= 1.5)
						{
							if (!roomsDictionary.ContainsKey(vertex.Key))
							{
								roomsDictionary.Add(vertex.Key, vertex.Value);
								break;
							}

						}

					}
				}
				MakeArea(roomsDictionary, count++, spaceIdList[spaceCount]);
				spaceCount++;
			}
			layer1.areas = areas;
			layer1.items = new Dictionary<string, Items>();
			Selected objeSelected = new Selected();
			objeSelected.areas = new List<object>();
			objeSelected.vertices = new List<object>();
			objeSelected.items = new List<object> { };
			objeSelected.holes = new List<object>();
			objeSelected.lines = new List<object>();

			layer1.selected = objeSelected;
			layers.Add("layer-1", layer1);
			return layer1;

		}



		#region Grids

		/* public class Zone
		 {
			 public string BlockId { get; set; }
			 public string ObjectId { get; set; }
			 public string BlockName { get; set; }
			 public List<Component.Point> Bounds { get; set; }

			 public Component.Point StartPoint { get; set; }
			 public Component.Point EndPoint { get; set; }

			 public int TotalNumberOfSpaces { get; set; }
			 public int TotalNumberOfZones { get; set; }

			 public double Area { get; set; }

			 public string DisplayName { get; set; }

			 public string Name { get; set; }

			 public string Layer { get; set; }

			 public string LayerId { get; set; }

			 public List<string> SpaceIds { get; set; }
		 }*/
		public class Point
		{
			public Point()
			{
			}
			private Point3d _position = new Point3d();


			public Point3d Position
			{
				get
				{
					return _position;
				}
				set
				{
					_position = value != null ? value : new Point3d();
				}
			}



			public Point(double x, double y, double z)
			{
				Position = new Point3d(x, y, z);
			}




		}
		public class Grid
		{
			public string id { get; set; }
			public string type { get; set; }
			public GridProperties properties { get; set; }
		}

		public class GridProperties
		{
			public int step { get; set; }
			public List<string> colors { get; set; }
		}
		#endregion

		public class Groups
		{
		}

		public class Meta
		{
		}

		#region Guides
		public class Guides
		{
			public Horizontal horizontal { get; set; }
			public Vertical vertical { get; set; }
			public Circular circular { get; set; }
		}

		public class Horizontal
		{
		}

		public class Vertical
		{
		}

		public class Circular
		{
		}
		#endregion

		#region Airside System
		public class AirsideSystem
		{
			//public string Name { get; set; }
			//public string Type { get; set; }
			//public Infiltration Infiltration { get; set; }
		}

		public class Infiltration
		{
			public string Method { get; set; }
			public string ConstructionQuality { get; set; }
			public double ACH { get; set; }
			// Not sure about the type of list
			public List<string> Spaces { get; set; }
		}
		#endregion

		#region Building Design
		public class BuildingDesign
		{
			public BuildingDetails building_details { get; set; }
			public MaterialLibrary material_library { get; set; }
			//public string building_Orientation { get; set; }
		}

		public class BuildingDetails
		{
			public int wall_height { get; set; }
			public string wall_height_unit { get; set; }
			public string buildingLevel { get; set; }
			public SurroundingFloorAirConditions surroundingFloorAirConditions { get; set; }
		}

		public class SurroundingFloorAirConditions
		{
			public bool lowerFloor { get; set; }
			public bool upperFloor { get; set; }
		}

		public class MaterialLibrary
		{
			public ExposedWall Exposed_Wall { get; set; }
			public PartitionWall Partition_Wall { get; set; }
			public GlassWall Glass_Wall { get; set; }
			public Floor Floor { get; set; }
			public Roof Roof { get; set; }
			public Ceiling Ceiling { get; set; }
		}

		public class ExposedWall
		{
			public string uValueUnit { get; set; }
			public double absorptivity { get; set; }
			public string materialAssembly { get; set; }
			public string wallGroup { get; set; }
			public string thicknessUnit { get; set; }
			public double total_Thickness { get; set; }
			public double uValue { get; set; }
			public double transmissivity { get; set; }
			public int colorAdjustmentFactor { get; set; }
		}

		public class PartitionWall
		{
			public string materialAssembly { get; set; }
			public string infoText { get; set; }
			public string total_Thickness { get; set; }
			public string uValue { get; set; }
			public string transmissivity { get; set; }
			public string absorptivity { get; set; }
			public string uValueUnit { get; set; }
			public string thicknessUnit { get; set; }
		}

		public class GlassWall
		{
			public string uValueUnit { get; set; }
			public double infiltrationThickness { get; set; }
			public List<int> color { get; set; }
			public string absorptivity { get; set; }
			public double shadingCoefficient { get; set; }
			public string materialAssembly { get; set; }
			public string thicknessUnit { get; set; }
			public string total_Thickness { get; set; }
			public string infoText { get; set; }
			public string uValue { get; set; }
			public string transmissivity { get; set; }
		}

/*		public class Floor
		{
			public string materialAssembly { get; set; }
			public string infoText { get; set; }
			public string total_Thickness { get; set; }
			public string uValue { get; set; }
			public string transmissivity { get; set; }
			public string absorptivity { get; set; }
			public string uValueUnit { get; set; }
			public string thicknessUnit { get; set; }
		}*/

		public class FloorMat
		{
			public string id { get; set; }
			public FloorMatProperties properties { get; set; }
		}
		public class FloorMatProperties
		{
			public string name { get; set; }
			public string unit { get; set; }
			public string type { get; set; }
			public double area { get; set; }
			public MaterialProperties materialProperties { get; set; }
		}

		public class Roof
		{
			public string uValueUnit { get; set; }
			public int f { get; set; }
			public double absorptivity { get; set; }
			public double uValueSolarLoad { get; set; }
			public string materialAssembly { get; set; }
			public int roofNo { get; set; }
			public string thicknessUnit { get; set; }
			public double total_Thickness { get; set; }
			public string infoText { get; set; }
			public double uValue { get; set; }
			public double transmissivity { get; set; }
			public int colorAdjustmentFactor { get; set; }
		}

		public class Ceiling
		{
			public string uValueUnit { get; set; }
			public int f { get; set; }
			public double absorptivity { get; set; }
			public double uValueSolarLoad { get; set; }
			public string materialAssembly { get; set; }
			public int roofNo { get; set; }
			public string thicknessUnit { get; set; }
			public double total_Thickness { get; set; }
			public string infoText { get; set; }
			public double uValue { get; set; }
			public double transmissivity { get; set; }
			public int colorAdjustmentFactor { get; set; }
		}

		public class CeilingMat
		{
			public string id { get; set; }
			public CeilingMatProperties properties { get; set; }
		}
		public class CeilingMatProperties
		{
			public string name { get; set; }
			public string unit { get; set; }
			public string type { get; set; }
			public double area { get; set; }
			public RoofCeilingMaterialProperties materialProperties { get; set; }
		}
		#endregion

		#region Layers & Others
		public class Layer
		{
			public string id { get; set; }
			public int altitude { get; set; }
			public int order { get; set; }
			public int opacity { get; set; }
			public string name { get; set; }
			public bool visible { get; set; }
			public Dictionary<string, Vertex> vertices { get; set; }
			public Dictionary<string, Line2D> lines { get; set; }
			public Dictionary<string, Holes> holes { get; set; }
			public Dictionary<string, Areas> areas { get; set; }
			public Dictionary<string, Items> items { get; set; }

			public Dictionary<string, Component.Zone> zones { get; set; }
			/*public Dictionary<string, FloorMat> floorMat { get; set; }
            public Dictionary<string, CeilingMat> ceilingMat { get; set; }*/
			public Selected selected { get; set; }
		}


		public class Selected
		{
			public List<object> vertices { get; set; }
			public List<object> lines { get; set; }
			public List<object> holes { get; set; }
			public List<object> areas { get; set; }
			public List<object> items { get; set; }
		}

		public class Misc
		{
		}

		public class Normal
		{
			public double x { get; set; }
			public double y { get; set; }
			public double z { get; set; }
		}
		#endregion

		#region Vertices
		public class Vertex
		{

			public string id { get; set; }
			public string type { get; set; }
			public string prototype { get; set; }
			public string name { get; set; }
			public Misc misc { get; set; }
			public bool selected { get; set; }
			public VertexProperties properties { get; set; }
			public bool visible { get; set; }
			public List<object> tag { get; set; }
			public double x { get; set; }
			public double y { get; set; }
			public List<string> lines { get; set; }
			public List<object> areas { get; set; }
		}

		public class VertexProperties
		{
		}
		#endregion

		#region Lines
		public class Line2D
		{

			public string id { get; set; }
			public string type { get; set; }
			public string prototype { get; set; }
			public string name { get; set; }
			public Misc misc { get; set; }
			public bool selected { get; set; }
			public JObject properties { get; set; }
			public bool visible { get; set; }
			public List<object> tag { get; set; }
			public List<string> vertices { get; set; }
			public List<string> holes { get; set; }

		}


		public class LineProperties
		{
			public string type { get; set; }
			public double length { get; set; }
			public double thickness { get; set; }
			public bool userSetPartition { get; set; }
			public MaterialProperties materialProperties { get; set; }
		}
		#endregion

		#region Areas
		public class Areas
		{

			public string id { get; set; }
			public string type { get; set; }
			public string prototype { get; set; }
			public string name { get; set; }
			public Misc misc { get; set; }
			public bool selected { get; set; }
			public JObject properties { get; set; }
			public bool visible { get; set; }
			public List<object> tag { get; set; }
			public List<string> vertices { get; set; }
			public List<object> holes { get; set; }
			public Dictionary<string, double> fluidPoint { get; set; }
		}

		public class AreaProperties
		{
			public FloorMatProperties Floor { get; set; }
			public CeilingMatProperties Ceiling { get; set; }
			public FidelitySimulationInputs fidelitySimulationInputs { get; set; }
		}

		public class FidelitySimulationInputs
		{
			public string method = "engineeringSimlation";
			public Appliances appliances = new Appliances();
			public Lighting lighting = new Lighting();
			public Occupancy occupancy = new Occupancy();
			public string spaceType = "Office";
		}

		public class Appliances
		{
			public int sensibleAppliancesLoad = 1;
			public int latentAppliancesLoad = 0;
			public string appliancesLoadPerAreaUnit = "W/sq.ft";
		}

		public class Lighting
		{
			public double lightingLoadPerArea = 1.11;
			public string lightingLoadPerAreaUnit = "W/sq.ft";
		}

		public class Occupancy
		{
			public string occupantActivity = "Manikin Standing";
			public string occupantType = "defaultASHRAE";
			public int numberOfManikins = 0;
			public ManikinType manikinType { get; set; }
		}

		public class ManikinType
		{
			public int men = 0;
			public int women = 0;
			public int children = 0;
		}
		#endregion

		#region Holes
		public class Holes
		{

			public string id { get; set; }
			public string type { get; set; }
			public string prototype { get; set; }
			public string name { get; set; }
			public Misc misc { get; set; }
			public bool selected { get; set; }
			public JObject properties { get; set; }
			public bool visible { get; set; }
			public List<object> tag { get; set; }
			public double offset { get; set; }
			public string line { get; set; }
			//public string mountType { get; set; }
			//public string diffuserType { get; set; }
			public Normal normal { get; set; }
			public string space { get; set; }
		}

		public class HoleProperties
		{
		}

		public class DoorProperties : HoleProperties
		{
			public double width { get; set; }
			public double height { get; set; }
			public double thickness { get; set; }
			public double altitude { get; set; }
			public bool flip { get; set; }
			public bool infiltration { get; set; }
		}

		public class WindowProperties : HoleProperties
		{
			public double width { get; set; }
			public double height { get; set; }
			public double altitude { get; set; }
			public double thickness { get; set; }
			public bool infiltration { get; set; }
			public MaterialProperties materialProperties { get; set; }
		}

		public class ThermostatProperties : HoleProperties
		{
			public double altitude { get; set; }
			public bool flip { get; set; }
		}
		#endregion

		#region Items
		public class Items
		{
			public string id { get; set; }
			public string type { get; set; }
			public string prototype { get; set; }
			public string name { get; set; }
			public Misc misc { get; set; }
			public bool selected { get; set; }
			public ItemProperties properties { get; set; }
			public bool visible { get; set; }
			public List<object> tag { get; set; }
			public double x { get; set; }
			public double y { get; set; }
			public double rotation { get; set; }
			public string mountType { get; set; }
			public string diffuserType { get; set; }
		}

		public class ItemProperties
		{
		}

		public class SleepingManikinProperties : ItemProperties
		{
			public int altitude { get; set; }
		}

		public class ElectronicApplianceProperties : ItemProperties
		{
			public string width { get; set; }
			public string length { get; set; }
			public string height { get; set; }
			public string heatGain { get; set; }
		}

		public class ProjectorProperties : ItemProperties
		{
			public string width { get; set; }
			public string length { get; set; }
			public string height { get; set; }
			public int altitude { get; set; }
			public string heatGain { get; set; }
		}

		public class LightProperties : ItemProperties
		{
			public string size { get; set; }
			public string inputPower { get; set; }
			public string heatGain { get; set; }
		}
		#endregion

		#region Airside System Ducted Components Properties
		public class SidewallGrilleSupplyProperties : HoleProperties
		{
			public double altitude { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public string width { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public string height { get; set; }
			public bool flip { get; set; }
			public double defaultCoolingRH { get; set; }
			public double bladeDeflection { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class SidewallGrilleReturnProperties : HoleProperties
		{
			public string width { get; set; }
			public string height { get; set; }
			public double altitude { get; set; }
			public bool flip { get; set; }
			public double bladeDeflection { get; set; }
		}

		public class SidewallLinearBarGrilleSupplyProperties : HoleProperties
		{
			public double altitude { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double flowAngle { get; set; }
			public string width { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public string height { get; set; }
			public bool flip { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class SidewallLinearBarGrilleReturnProperties : HoleProperties
		{
			public double altitude { get; set; }
			public bool flip { get; set; }
			public double flowAngle { get; set; }
			public string width { get; set; }
			public string height { get; set; }
		}

		public class GrilleSupplyProperties : ItemProperties
		{
			public double defaultHeatingTemperature { get; set; }
			public string width { get; set; }
			public string length { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double defaultCoolingRH { get; set; }
			public double bladeDeflection { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class GrilleReturnProperties : ItemProperties
		{
			public string width { get; set; }
			public string length { get; set; }
			public double bladeDeflection { get; set; }
		}

		public class LinearBarGrilleSupplyProperties : ItemProperties
		{
			public double defaultHeatingTemperature { get; set; }
			public double flowAngle { get; set; }
			public string width { get; set; }
			public string length { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class LinearBarGrilleReturnProperties : ItemProperties
		{
			public string width { get; set; }
			public string length { get; set; }
			public double flowAngle { get; set; }
		}

		public class LinearDoubleSlotSupplyProperties : ItemProperties
		{
			public double defaultHeatingTemperature { get; set; }
			public double flowAngle { get; set; }
			public string width { get; set; }
			public string length { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class SwirlFaceSupplyProperties : ItemProperties
		{
			public string size { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double flowAngle { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double twistAngle { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class UnderfloorTwistPatternSupplyProperties : ItemProperties
		{
			public string size { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double flowAngle { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double twistAngle { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class SquarePlaqueSupplyProperties : ItemProperties
		{
			public string size { get; set; }
			public double flowAngle { get; set; }
			public double coolingAirFlowRate { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double defaultCoolingRH { get; set; }
			public double defaultHeatingRH { get; set; }
		}
		#endregion

		#region Airside System Ductless Component Properties 
		public class SplitACProperties : HoleProperties
		{
			public double coolingFreshAirIntake { get; set; }
			public double altitude { get; set; }
			public CapacityRange CapacityRange { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double heatingSwingAngle { get; set; }
			public SwingAngleRange swingAngleRange { get; set; }
			public bool flip { get; set; }
			public double coolingSwingAngle { get; set; }
			public double defaultCoolingRH { get; set; }
			public bool IsInverterAC { get; set; }
			public SetTemperature SetTemperature { get; set; }
			public SupplyAirFlowRange SupplyAirFlowRange { get; set; }
			public double ByPassFactor { get; set; }
			public double heatingFreshAirIntake { get; set; }
			public string capacity { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class CassetteACProperties : ItemProperties
		{
			public double coolingFreshAirIntake { get; set; }
			public CapacityRange CapacityRange { get; set; }
			public double defaultHeatingTemperature { get; set; }
			public double heatingAirFlowRate { get; set; }
			public double defaultHeatingRH { get; set; }
			public double heatingSwingAngle { get; set; }
			public SwingAngleRange swingAngleRange { get; set; }
			public double coolingSwingAngle { get; set; }
			public double defaultCoolingRH { get; set; }
			public bool IsInverterAC { get; set; }
			public SetTemperature SetTemperature { get; set; }
			public SupplyAirFlowRange SupplyAirFlowRange { get; set; }
			public double ByPassFactor { get; set; }
			public double heatingFreshAirIntake { get; set; }
			public string capacity { get; set; }
			public double defaultCoolingTemperature { get; set; }
			public double coolingAirFlowRate { get; set; }
		}

		public class CapacityRange
		{
			public double Min { get; set; }
			public double Max { get; set; }
		}

		public class SwingAngleRange
		{
			public double Min { get; set; }
			public double Max { get; set; }
		}

		public class SetTemperature
		{
			public double Cooling { get; set; }
			public double Heating { get; set; }
		}

		public class SupplyAirFlowRange
		{
			public double High { get; set; }
			public double Medium { get; set; }
			public double Low { get; set; }
		}
		#endregion

		#region Material Properties
		public class MaterialProperties
		{
			public string materialAssembly;
			public double total_Thickness;
			public double uValue;
			public string uValueUnit = "Btu/(hr-ft²-°F)";
			public double transmissivity;
			public double absorptivity;
			public string thicknessUnit = "in";
		}

		public class ExposedWallMaterialProperties : MaterialProperties
		{
			public string wallGroup;
			public double colorAdjustmentFactor;
		}

		public class RoofCeilingMaterialProperties : MaterialProperties
		{
			public double uValueSolarLoad;
			public double colorAdjustmentFactor;
			public double f;
		}

		public class GlassMaterialProperties : MaterialProperties
		{
			public double shadingCoefficient;
			public List<int> color;
		}

		#endregion

		/*public class DesignShiftOffset
        {
            public double offsetXAdd { get; set; }
            public double offsetXSub { get; set; }
            public double offsetYAdd { get; set; }
            public double offsetYSub { get; set; }
        }*/
	}
}


