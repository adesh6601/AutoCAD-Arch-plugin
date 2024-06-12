using System.Collections.Generic;
using Collection;
using Component;

namespace Model
{
	public class Builder
	{
		public void Build(Entities entities, Building building)
		{
			AddCurtainWallsToBuilding(entities, building);
			AddDoorsToBuilding(entities, building);
			AddOpeningsToBuilding(entities, building);
			AddWallsToBuilding(entities, building);
			AddWindowsToBuilding(entities, building);
			AddWindowAssembliesToBuilding(entities, building);

			AddBlockReferencesToBuilding(entities, building);
			AddMultiViewBlockReferencesToBuilding(entities, building);

			AddSpacesToBuilding(entities, building);
			AddZonesToBuilding(entities, building);
		}

		public void AddCurtainWallsToBuilding(Entities entities, Building building)
		{
			foreach (CurtainWall curtainWall in entities.CurtainWalls)
			{
				string handleId = curtainWall.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].CurtainWalls.Add(curtainWall);
				}
			}
		}

		public void AddDoorsToBuilding(Entities entities, Building building)
		{
			foreach (Door door in entities.Doors)
			{
				string handleId = door.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Doors.Add(door);
				}
			}
		}

		public void AddOpeningsToBuilding(Entities entities, Building building)
		{
			foreach (Opening opening in entities.Openings)
			{
				string handleId = opening.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Openings.Add(opening);
				}
			}
		}

		public void AddWallsToBuilding(Entities entities, Building building)
		{
			foreach (Wall wall in entities.Walls)
			{
				string handleId = wall.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Walls.Add(wall);
				}
			}
		}

		public void AddWindowsToBuilding(Entities entities, Building building)
		{
			foreach (Window window in entities.Windows)
			{
				string handleId = window.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Windows.Add(window);
				}
			}
		}

		public void AddWindowAssembliesToBuilding(Entities entities, Building building)
		{
			foreach (WindowAssembly windowAssembly in entities.WindowAssemblies)
			{
				string handleId = windowAssembly.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].WindowAssemblies.Add(windowAssembly);
				}
			}
		}

		public void AddBlockReferencesToBuilding(Entities entities, Building building)
		{
			foreach (BlockReference blockReference in entities.BlockReferences)
			{
				string handleId = blockReference.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].BlockReferences.Add(blockReference);
				}
			}
		}

		public void AddMultiViewBlockReferencesToBuilding(Entities entities, Building building)
		{
			foreach (MultiViewBlockReference multiViewBlockReferences in entities.MultiViewBlockReferences)
			{
				string handleId = multiViewBlockReferences.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].MultiViewBlockReferences.Add(multiViewBlockReferences);
				}
			}
		}

		public void AddSpacesToBuilding(Entities entities, Building building)
		{
			foreach (Space space in entities.Spaces)
			{
				string handleId = space.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Spaces.Add(space);
				}
			}
		}

		public void AddZonesToBuilding(Entities entities, Building building)
		{
			foreach (Zone zone in entities.Zones)
			{
				string handleId = zone.HandleId;

				List<string> positions = entities.Positions[handleId];

				foreach (string position in positions)
				{
					string[] buildingAndFloor = position.Split('.');

					string buildingId = buildingAndFloor[0];
					string floorId = buildingAndFloor[1];

					if (!building.Floors.ContainsKey(floorId))
					{
						building.Floors[floorId] = new Floor();
					}

					building.Floors[floorId].Zones.Add(zone);
				}
			}
		}
	}
}
