using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Collection.DataFormatter;

namespace Collection
{
	public class Building
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
		public Dictionary<string, Collection.DataFormatter.Layer> Floors { get; set; }

		public Building(Dictionary<string, Collection.DataFormatter.Layer> floors)
		{

			componenets();
			this.Floors = floors;
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
			string buildingTemplate = "office";

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

			//buildingDesign.building_Orientation = "0";

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
	}
}
