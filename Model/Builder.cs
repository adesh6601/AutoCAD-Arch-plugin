using System.Collections.Generic;
using Collection;
using Component;

namespace Model
{
	public class Builder
	{
		public ConvertedEntities Entities;
		public Building Building;

		public Builder(ConvertedEntities entites)
		{
			Entities = entites;
		}

		public void Build()
		{
			AddCurtainWallsToBuilding();
			AddDoorsToBuilding();
			AddOpeningsToBuilding();
			AddWallsToBuilding();
			AddWindowsToBuilding();
			AddWindowAssembliesToBuilding();

			AddBlockReferencesToBuilding();
			AddMultiViewBlockReferencesToBuilding();

			AddSpacesToBuilding();
			AddZonesToBuilding();
		}

		public Building GetBuilding()
		{
			return Building;
		}

		public void AddCurtainWallsToBuilding()
		{
			foreach (CurtainWall curtainWall in Entities.CurtainWalls)
			{
				string handleId = curtainWall.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].CurtainWalls.Add(curtainWall);
				}
			}
		}

		public void AddDoorsToBuilding()
		{
			foreach (Door door in Entities.Doors)
			{
				string handleId = door.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Doors.Add(door);
				}
			}
		}

		public void AddOpeningsToBuilding()
		{
			foreach (Opening opening in Entities.Openings)
			{
				string handleId = opening.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Openings.Add(opening);
				}
			}
		}

		public void AddWallsToBuilding()
		{
			foreach (Wall wall in Entities.Walls)
			{
				string handleId = wall.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Walls.Add(wall);
				}
			}
		}

		public void AddWindowsToBuilding()
		{
			foreach (Window window in Entities.Windows)
			{
				string handleId = window.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Windows.Add(window);
				}
			}
		}

		public void AddWindowAssembliesToBuilding()
		{
			foreach (WindowAssembly windowAssembly in Entities.WindowAssemblies)
			{
				string handleId = windowAssembly.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].WindowAssemblies.Add(windowAssembly);
				}
			}
		}

		public void AddBlockReferencesToBuilding()
		{
			foreach (BlockReference blockReference in Entities.BlockReferences)
			{
				string handleId = blockReference.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].BlockReferences.Add(blockReference);
				}
			}
		}

		public void AddMultiViewBlockReferencesToBuilding()
		{
			foreach (MultiViewBlockReference multiViewBlockReferences in Entities.MultiViewBlockReferences)
			{
				string handleId = multiViewBlockReferences.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].MultiViewBlockReferences.Add(multiViewBlockReferences);
				}
			}
		}

		public void AddSpacesToBuilding()
		{
			foreach (Space space in Entities.Spaces)
			{
				string handleId = space.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Spaces.Add(space);
				}
			}
		}

		public void AddZonesToBuilding()
		{
			foreach (Zone zone in Entities.Zones)
			{
				string handleId = zone.HandleId;

				List<string> positions = Entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string building = buildingAndFloor[0];
					string floor = buildingAndFloor[1];

					if (!Building.Floors.ContainsKey(floor))
					{
						Building.Floors[floor] = new Floor();
					}

					Building.Floors[floor].Zones.Add(zone);
				}
			}
		}
	}
}
