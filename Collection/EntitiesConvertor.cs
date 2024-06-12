using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Aec.DatabaseServices;

namespace Collection
{
	public class EntitiesConvertor
	{
		public UnitsValue UnitsValue;

		public void Convert(ACADEntities entities, Entities convertedEntities)
		{
			ConvertCurtainWalls(entities, convertedEntities);
			ConvertDoors(entities, convertedEntities);
			ConvertOpenings(entities, convertedEntities);
			ConvertWalls(entities, convertedEntities);
			ConvertWindows(entities, convertedEntities);
			ConvertWindowAssemblies(entities, convertedEntities);

			ConvertBlockReferences(entities, convertedEntities);
			ConvertMultiViewBlockReferences(entities, convertedEntities);

			ConvertSpaces(entities, convertedEntities);
			ConvertZones(entities, convertedEntities);
		}

		public void ConvertCurtainWalls(ACADEntities entities, Entities convertedEntities)
		{
			foreach(CurtainWallLayout curtainWall in entities.CurtainWalls)
			{
				Component.CurtainWall convertedCurtainWall = new Component.CurtainWall();

				convertedCurtainWall.DisplayName = curtainWall.DisplayName;

				convertedCurtainWall.BlockName = curtainWall.BlockName;
				convertedCurtainWall.BlockId = curtainWall.BlockId.ToString();

				convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

				convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X, curtainWall.Bounds.Value.MaxPoint.Y, curtainWall.Bounds.Value.MaxPoint.Z));
				convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X, curtainWall.Bounds.Value.MinPoint.Y, curtainWall.Bounds.Value.MinPoint.Z));

				convertedCurtainWall.Handle = curtainWall.Handle;
				convertedCurtainWall.HandleId = curtainWall.Handle.ToString();

				convertedCurtainWall.Layer = curtainWall.Layer;
				convertedCurtainWall.LayerId = curtainWall.LayerId.ToString();

				convertedCurtainWall.MaterialHandle = curtainWall.MaterialId.Handle;
				convertedCurtainWall.MaterialId = curtainWall.MaterialId.ToString();

				convertedCurtainWall.Color = curtainWall.Color;

				convertedCurtainWall.CellCount = curtainWall.CellCount;

				convertedCurtainWall.Description = curtainWall.Description;

				convertedCurtainWall.Length = curtainWall.Length;
				//convertedCurtainWall.Width = 
				convertedCurtainWall.BaseHeight = curtainWall.BaseHeight;
				//convertedCurtainWall.Area = 

				convertedCurtainWall.StartPoint = new Component.Point(curtainWall.StartPoint.X, curtainWall.StartPoint.Y, curtainWall.StartPoint.Z);
				convertedCurtainWall.EndPoint = new Component.Point(curtainWall.EndPoint.X, curtainWall.EndPoint.Y, curtainWall.EndPoint.Z);
				
				//convertedCurtainWall.Rotation = 

				convertedCurtainWall.CollisionType = curtainWall.CollisionType.ToString();

				convertedCurtainWall.StyleHandle = curtainWall.StyleId.Handle;
				convertedCurtainWall.Style = entities.CurtainWallLayoutStyles[curtainWall.StyleId.Handle.ToString()].Name;
				convertedCurtainWall.StyleId = curtainWall.StyleId.ToString();

				Material curtainWallMaterial = entities.Materials[curtainWall.MaterialId.Handle.ToString()];

				convertedCurtainWall.MaterialName = curtainWallMaterial.Name;

				convertedCurtainWall.Ambient = curtainWallMaterial.Ambient;
				convertedCurtainWall.ColorBleedScale = curtainWallMaterial.ColorBleedScale;
				convertedCurtainWall.IndirectBumpScale = curtainWallMaterial.IndirectBumpScale;
				convertedCurtainWall.Luminance = curtainWallMaterial.Luminance;
				convertedCurtainWall.ReflectanceScale = curtainWallMaterial.ReflectanceScale;
				convertedCurtainWall.Reflectivity = curtainWallMaterial.Reflectivity;
				convertedCurtainWall.SelfIllumination = curtainWallMaterial.SelfIllumination;
				convertedCurtainWall.Translucence = curtainWallMaterial.Translucence;
				convertedCurtainWall.TransmittanceScale = curtainWallMaterial.TransmittanceScale;
				convertedCurtainWall.TwoSided = curtainWallMaterial.TwoSided;

				convertedEntities.CurtainWalls.Add(convertedCurtainWall);
			}
		}

		public void ConvertDoors(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Door door in entities.Doors)
			{
				Component.Door convertedDoor = new Component.Door();

				convertedDoor.DisplayName = door.DisplayName;

				convertedDoor.BlockName = door.BlockName;
				convertedDoor.BlockId = door.BlockId.ToString();

				convertedDoor.ObjectId = door.ObjectId.ToString();

				convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MaxPoint.X, door.Bounds.Value.MaxPoint.Y, door.Bounds.Value.MaxPoint.Z));
				convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MinPoint.X, door.Bounds.Value.MinPoint.Y, door.Bounds.Value.MinPoint.Z));

				convertedDoor.Handle = door.Handle;
				convertedDoor.HandleId = door.Handle.ToString();

				convertedDoor.Layer = door.Layer;
				convertedDoor.LayerId = door.LayerId.ToString();

				convertedDoor.MaterialHandle = door.MaterialId.Handle;
				convertedDoor.MaterialId = door.MaterialId.ToString();

				convertedDoor.Color = door.Color;

				convertedDoor.Description = door.Description;
				//convertedDoor.WallId = 

				convertedDoor.Width = door.Width;
				convertedDoor.Height = door.Height;
				convertedDoor.Area = door.Area;

				convertedDoor.StartPoint = new Component.Point(door.StartPoint.X, door.StartPoint.Y, door.StartPoint.Z);
				convertedDoor.EndPoint = new Component.Point(door.EndPoint.X, door.EndPoint.Y, door.EndPoint.Z);

				convertedDoor.Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);
				//convertedDoor.Offset = 

				convertedDoor.CollisionType = door.CollisionType.ToString();

				convertedDoor.StyleHandle = door.StyleId.Handle;
				convertedDoor.Style = entities.DoorStyles[door.StyleId.Handle.ToString()].Name;
				convertedDoor.StyleId = door.StyleId.ToString();

				Material doorMaterial = entities.Materials[door.MaterialId.Handle.ToString()];

				convertedDoor.MaterialName = doorMaterial.Name;

				convertedDoor.Ambient = doorMaterial.Ambient;
				convertedDoor.ColorBleedScale = doorMaterial.ColorBleedScale;
				convertedDoor.IndirectBumpScale = doorMaterial.IndirectBumpScale;
				convertedDoor.Luminance = doorMaterial.Luminance;
				convertedDoor.ReflectanceScale = doorMaterial.ReflectanceScale;
				convertedDoor.Reflectivity = doorMaterial.Reflectivity;
				convertedDoor.SelfIllumination = doorMaterial.SelfIllumination;
				convertedDoor.Translucence = doorMaterial.Translucence;
				convertedDoor.TransmittanceScale = doorMaterial.TransmittanceScale;
				convertedDoor.TwoSided = doorMaterial.TwoSided;

				convertedEntities.Doors.Add(convertedDoor);
			}
		}

		public void ConvertOpenings(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Opening opening in entities.Openings)
			{
				Component.Opening convertedOpening = new Component.Opening();

				convertedOpening.DisplayName = opening.DisplayName;

				convertedOpening.BlockName = opening.BlockName;
				convertedOpening.BlockId = opening.BlockId.ToString();

				convertedOpening.ObjectId = opening.ObjectId.ToString();

				convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MaxPoint.X, opening.Bounds.Value.MaxPoint.Y, opening.Bounds.Value.MaxPoint.Z));
				convertedOpening.Bounds.Add(new Component.Point(opening.Bounds.Value.MinPoint.X, opening.Bounds.Value.MinPoint.Y, opening.Bounds.Value.MinPoint.Z));

				convertedOpening.Handle = opening.Handle;
				convertedOpening.HandleId = opening.Handle.ToString();

				convertedOpening.Layer = opening.Layer;
				convertedOpening.LayerId = opening.LayerId.ToString();

				convertedOpening.MaterialHandle = opening.MaterialId.Handle;
				convertedOpening.MaterialId = opening.MaterialId.ToString();

				convertedOpening.Color = opening.Color;

				convertedOpening.ShapeType = opening.ShapeType.ToString();
				convertedOpening.LineTypeID = opening.LinetypeId.ToString();
				//convertedOpening.WallId = 

				convertedOpening.Width = opening.Width;
				convertedOpening.Height = opening.Height;
				convertedOpening.Area = opening.Area;

				convertedOpening.StartPoint = new Component.Point(opening.StartPoint.X, opening.StartPoint.Y, opening.StartPoint.Z);
				convertedOpening.EndPoint = new Component.Point(opening.EndPoint.X, opening.EndPoint.Y, opening.EndPoint.Z);

				convertedOpening.Normal = new Component.Point(opening.Normal.X, opening.Normal.Y, opening.Normal.Z);
				//convertedOpening.Offset = 

				convertedOpening.CollisionType = opening.CollisionType.ToString();


				convertedEntities.Openings.Add(convertedOpening);
			}
		}
	
		public void ConvertWalls(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Wall wall in entities.Walls)
			{
				Component.Wall convertedWall = new Component.Wall();

				convertedWall.DisplayName = wall.DisplayName;

				convertedWall.BlockName = wall.BlockName;
				convertedWall.BlockId = wall.BlockId.ToString();

				convertedWall.ObjectId = wall.ObjectId.ToString();

				convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MaxPoint.X, wall.Bounds.Value.MaxPoint.Y, wall.Bounds.Value.MaxPoint.Z));
				convertedWall.Bounds.Add(new Component.Point(wall.Bounds.Value.MinPoint.X, wall.Bounds.Value.MinPoint.Y, wall.Bounds.Value.MinPoint.Z));

				convertedWall.Handle = wall.Handle;
				convertedWall.HandleId = wall.Handle.ToString();

				convertedWall.Layer = wall.Layer;
				convertedWall.LayerId = wall.LayerId.ToString();

				convertedWall.MaterialHandle = wall.MaterialId.Handle;
				convertedWall.MaterialId = wall.MaterialId.ToString();

				convertedWall.Color = wall.Color;

				convertedWall.Description = wall.Description;

				convertedWall.Length = wall.Length;
				convertedWall.Width = wall.Width; 
				convertedWall.BaseHeight = wall.BaseHeight;
				//convertedWall.Area = 

				convertedWall.StartPoint = new Component.Point(wall.StartPoint.X, wall.StartPoint.Y, wall.StartPoint.Z);
				convertedWall.EndPoint = new Component.Point(wall.EndPoint.X, wall.EndPoint.Y, wall.EndPoint.Z);

				//convertedWall.Rotation = 

				convertedWall.CollisionType = wall.CollisionType.ToString();

				convertedWall.StyleHandle = wall.StyleId.Handle;
				convertedWall.Style = entities.WallStyles[wall.StyleId.Handle.ToString()].Name;
				convertedWall.StyleId = wall.StyleId.ToString();

				Material wallMaterial = entities.Materials[wall.MaterialId.Handle.ToString()];

				convertedWall.MaterialName = wallMaterial.Name;

				convertedWall.Ambient = wallMaterial.Ambient;
				convertedWall.ColorBleedScale = wallMaterial.ColorBleedScale;
				convertedWall.IndirectBumpScale = wallMaterial.IndirectBumpScale;
				convertedWall.Luminance = wallMaterial.Luminance;
				convertedWall.ReflectanceScale = wallMaterial.ReflectanceScale;
				convertedWall.Reflectivity = wallMaterial.Reflectivity;
				convertedWall.SelfIllumination = wallMaterial.SelfIllumination;
				convertedWall.Translucence = wallMaterial.Translucence;
				convertedWall.TransmittanceScale = wallMaterial.TransmittanceScale;
				convertedWall.TwoSided = wallMaterial.TwoSided;

				convertedEntities.Walls.Add(convertedWall);
			}
		}

		public void ConvertWindows(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Window window in entities.Windows)
			{
				Component.Window convertedWindow = new Component.Window();

				convertedWindow.DisplayName = window.DisplayName;

				convertedWindow.BlockName = window.BlockName;
				convertedWindow.BlockId = window.BlockId.ToString();

				convertedWindow.ObjectId = window.ObjectId.ToString();

				convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MaxPoint.X, window.Bounds.Value.MaxPoint.Y, window.Bounds.Value.MaxPoint.Z));
				convertedWindow.Bounds.Add(new Component.Point(window.Bounds.Value.MinPoint.X, window.Bounds.Value.MinPoint.Y, window.Bounds.Value.MinPoint.Z));

				convertedWindow.Handle = window.Handle;
				convertedWindow.HandleId = window.Handle.ToString();

				convertedWindow.Layer = window.Layer;
				convertedWindow.LayerId = window.LayerId.ToString();

				convertedWindow.MaterialHandle = window.MaterialId.Handle;
				convertedWindow.MaterialId = window.MaterialId.ToString();

				convertedWindow.Color = window.Color;

				convertedWindow.Description = window.Description;
				//convertedWindow.WallId = 

				convertedWindow.Width = window.Width;
				convertedWindow.Height = window.Height;
				convertedWindow.Area = window.Area;

				convertedWindow.StartPoint = new Component.Point(window.StartPoint.X, window.StartPoint.Y, window.StartPoint.Z);
				convertedWindow.EndPoint = new Component.Point(window.EndPoint.X, window.EndPoint.Y, window.EndPoint.Z);

				convertedWindow.Normal = new Component.Point(window.Normal.X, window.Normal.Y, window.Normal.Z); ;
				convertedWindow.Altitude = window.StartPoint.Z;
				//convertedWindow.Offset = 

				convertedWindow.CollisionType = window.CollisionType.ToString();

				convertedWindow.StyleHandle = window.StyleId.Handle;
				convertedWindow.Style = entities.WindowStyles[window.StyleId.Handle.ToString()].Name;
				convertedWindow.StyleId = window.StyleId.ToString();

				Material wallMaterial = entities.Materials[window.MaterialId.Handle.ToString()];

				convertedWindow.MaterialName = wallMaterial.Name;

				convertedWindow.Ambient = wallMaterial.Ambient;
				convertedWindow.ColorBleedScale = wallMaterial.ColorBleedScale;
				convertedWindow.IndirectBumpScale = wallMaterial.IndirectBumpScale;
				convertedWindow.Luminance = wallMaterial.Luminance;
				convertedWindow.ReflectanceScale = wallMaterial.ReflectanceScale;
				convertedWindow.Reflectivity = wallMaterial.Reflectivity;
				convertedWindow.SelfIllumination = wallMaterial.SelfIllumination;
				convertedWindow.Translucence = wallMaterial.Translucence;
				convertedWindow.TransmittanceScale = wallMaterial.TransmittanceScale;
				convertedWindow.TwoSided = wallMaterial.TwoSided;

				convertedEntities.Windows.Add(convertedWindow);
			}
		}

		public void ConvertWindowAssemblies(ACADEntities entities, Entities convertedEntities)
		{
			foreach (WindowAssembly windowAssembly in entities.WindowAssemblies)
			{
				Component.WindowAssembly convertedWindowAssembly = new Component.WindowAssembly();

				convertedWindowAssembly.DisplayName = windowAssembly.DisplayName;

				convertedWindowAssembly.BlockName = windowAssembly.BlockName;
				convertedWindowAssembly.BlockId = windowAssembly.BlockId.ToString();

				convertedWindowAssembly.ObjectId = windowAssembly.ObjectId.ToString();

				convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MaxPoint.X, windowAssembly.Bounds.Value.MaxPoint.Y, windowAssembly.Bounds.Value.MaxPoint.Z));
				convertedWindowAssembly.Bounds.Add(new Component.Point(windowAssembly.Bounds.Value.MinPoint.X, windowAssembly.Bounds.Value.MinPoint.Y, windowAssembly.Bounds.Value.MinPoint.Z));

				convertedWindowAssembly.Handle = windowAssembly.Handle;
				convertedWindowAssembly.HandleId = windowAssembly.Handle.ToString();

				convertedWindowAssembly.Layer = windowAssembly.Layer;
				convertedWindowAssembly.LayerId = windowAssembly.LayerId.ToString();

				convertedWindowAssembly.MaterialHandle = windowAssembly.MaterialId.Handle;
				convertedWindowAssembly.MaterialId = windowAssembly.MaterialId.ToString();

				convertedWindowAssembly.Color = windowAssembly.Color;

				convertedWindowAssembly.CellCount = windowAssembly.CellCount;
				convertedWindowAssembly.Description = windowAssembly.Description;
				//convertedWindowAssembly.WallId = 

				convertedWindowAssembly.Length = windowAssembly.Length;
				convertedWindowAssembly.Height = windowAssembly.Height;
				convertedWindowAssembly.Area = windowAssembly.Area;

				convertedWindowAssembly.StartPoint = new Component.Point(windowAssembly.StartPoint.X, windowAssembly.StartPoint.Y, windowAssembly.StartPoint.Z);
				convertedWindowAssembly.EndPoint = new Component.Point(windowAssembly.EndPoint.X, windowAssembly.EndPoint.Y, windowAssembly.EndPoint.Z);

				convertedWindowAssembly.Normal = new Component.Point(windowAssembly.Normal.X, windowAssembly.Normal.Y, windowAssembly.Normal.Z); ;
				//convertedWindowAssembly.Offset = 

				convertedWindowAssembly.CollisionType = windowAssembly.CollisionType.ToString();

				convertedWindowAssembly.StyleHandle = windowAssembly.StyleId.Handle;
				convertedWindowAssembly.Style = entities.WindowAssemblyStyles[windowAssembly.StyleId.Handle.ToString()].Name;
				convertedWindowAssembly.StyleId = windowAssembly.StyleId.ToString();

				Material wallMaterial = entities.Materials[windowAssembly.MaterialId.Handle.ToString()];

				convertedWindowAssembly.MaterialName = wallMaterial.Name;

				convertedWindowAssembly.Ambient = wallMaterial.Ambient;
				convertedWindowAssembly.ColorBleedScale = wallMaterial.ColorBleedScale;
				convertedWindowAssembly.IndirectBumpScale = wallMaterial.IndirectBumpScale;
				convertedWindowAssembly.Luminance = wallMaterial.Luminance;
				convertedWindowAssembly.ReflectanceScale = wallMaterial.ReflectanceScale;
				convertedWindowAssembly.Reflectivity = wallMaterial.Reflectivity;
				convertedWindowAssembly.SelfIllumination = wallMaterial.SelfIllumination;
				convertedWindowAssembly.Translucence = wallMaterial.Translucence;
				convertedWindowAssembly.TransmittanceScale = wallMaterial.TransmittanceScale;
				convertedWindowAssembly.TwoSided = wallMaterial.TwoSided;

				convertedEntities.WindowAssemblies.Add(convertedWindowAssembly);
			}
		}

		public void ConvertBlockReferences(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Autodesk.AutoCAD.DatabaseServices.BlockReference blockReference in entities.BlockReferences)
			{
				Component.BlockReference convertedBlockReference = new Component.BlockReference();

				//convertedBlockReference.DisplayName = blockReference.Name;

				convertedBlockReference.BlockName = blockReference.BlockName;
				convertedBlockReference.BlockId = blockReference.BlockId.ToString();

				convertedBlockReference.ObjectId = blockReference.ObjectId.ToString();

				//convertedBlockReference.Bounds.Add(new Component.Point(blockReference.Bounds.Value.MaxPoint.X, blockReference.Bounds.Value.MaxPoint.Y, blockReference.Bounds.Value.MaxPoint.Z));
				//convertedBlockReference.Bounds.Add(new Component.Point(blockReference.Bounds.Value.MinPoint.X, blockReference.Bounds.Value.MinPoint.Y, blockReference.Bounds.Value.MinPoint.Z));

				convertedBlockReference.Handle = blockReference.Handle;
				convertedBlockReference.HandleId = blockReference.Handle.ToString();

				convertedBlockReference.Layer = blockReference.Layer;
				convertedBlockReference.LayerId = blockReference.LayerId.ToString();

				convertedBlockReference.MaterialHandle = blockReference.MaterialId.Handle;
				convertedBlockReference.MaterialId = blockReference.MaterialId.ToString();

				convertedBlockReference.Color = blockReference.Color;

				//convertedBlockReference.Length = 
				//convertedBlockReference.Width = 
				//convertedBlockReference.BaseHeight = 
				//convertedBlockReference.Area = 

				convertedBlockReference.Position = new Component.Point(blockReference.Position.X, blockReference.Position.Y, blockReference.Position.Z);
				convertedBlockReference.Rotation = blockReference.Rotation;

				convertedBlockReference.ScaleFactor = new Component.Point(blockReference.ScaleFactors.X, blockReference.ScaleFactors.Y, blockReference.ScaleFactors.Z); ;

				convertedEntities.BlockReferences.Add(convertedBlockReference);
			}
		}

		public void ConvertMultiViewBlockReferences(ACADEntities entities, Entities convertedEntities)
		{
			foreach (MultiViewBlockReference multiViewBlockReference in entities.MultiViewBlockReferences)
			{
				Component.MultiViewBlockReference convertedMultiViewBlockReference = new Component.MultiViewBlockReference();

				//convertedMultiViewBlockReference.DisplayName = multiViewBlockReference.DisplayName;

				convertedMultiViewBlockReference.BlockName = multiViewBlockReference.BlockName;
				convertedMultiViewBlockReference.BlockId = multiViewBlockReference.BlockId.ToString();

				convertedMultiViewBlockReference.ObjectId = multiViewBlockReference.ObjectId.ToString();

				//convertedMultiViewBlockReference.Bounds.Add(new Component.Point(multiViewBlockReference.Bounds.Value.MaxPoint.X, multiViewBlockReference.Bounds.Value.MaxPoint.Y, multiViewBlockReference.Bounds.Value.MaxPoint.Z));
				//convertedMultiViewBlockReference.Bounds.Add(new Component.Point(multiViewBlockReference.Bounds.Value.MinPoint.X, multiViewBlockReference.Bounds.Value.MinPoint.Y, multiViewBlockReference.Bounds.Value.MinPoint.Z));

				convertedMultiViewBlockReference.Handle = multiViewBlockReference.Handle;
				convertedMultiViewBlockReference.HandleId = multiViewBlockReference.Handle.ToString();

				convertedMultiViewBlockReference.Layer = multiViewBlockReference.Layer;
				convertedMultiViewBlockReference.LayerId = multiViewBlockReference.LayerId.ToString();

				convertedMultiViewBlockReference.MaterialHandle = multiViewBlockReference.MaterialId.Handle;
				convertedMultiViewBlockReference.MaterialId = multiViewBlockReference.MaterialId.ToString();

				convertedMultiViewBlockReference.Color = multiViewBlockReference.Color;

				//convertedMultiViewBlockReference.Length = 
				//convertedMultiViewBlockReference.Width = 
				//convertedMultiViewBlockReference.BaseHeight = 
				//convertedMultiViewBlockReference.Area = 

				convertedMultiViewBlockReference.StartPoint = new Component.Point(multiViewBlockReference.StartPoint.X, multiViewBlockReference.StartPoint.Y, multiViewBlockReference.StartPoint.Z);
				convertedMultiViewBlockReference.EndPoint = new Component.Point(multiViewBlockReference.EndPoint.X, multiViewBlockReference.EndPoint.Y, multiViewBlockReference.EndPoint.Z);

				convertedMultiViewBlockReference.Rotation = multiViewBlockReference.Rotation;

				convertedMultiViewBlockReference.StyleHandle = multiViewBlockReference.StyleId.Handle;
				convertedMultiViewBlockReference.StyleId = multiViewBlockReference.StyleId.ToString();

				convertedEntities.MultiViewBlockReferences.Add(convertedMultiViewBlockReference);
			}
		}

		public void ConvertSpaces(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Space space in entities.Spaces)
			{
				Component.Space convertedSpace= new Component.Space();

				convertedSpace.DisplayName = space.DisplayName;

				convertedSpace.BlockName = space.BlockName;
				convertedSpace.BlockId = space.BlockId.ToString();

				convertedSpace.ObjectId = space.ObjectId.ToString();

				convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MaxPoint.X, space.Bounds.Value.MaxPoint.Y, space.Bounds.Value.MaxPoint.Z));
				convertedSpace.Bounds.Add(new Component.Point(space.Bounds.Value.MinPoint.X, space.Bounds.Value.MinPoint.Y, space.Bounds.Value.MinPoint.Z));

				convertedSpace.Handle = space.Handle;
				convertedSpace.HandleId = space.Handle.ToString();

				convertedSpace.Layer = space.Layer;
				convertedSpace.LayerId = space.LayerId.ToString();

				convertedSpace.MaterialHandle = space.MaterialId.Handle;
				convertedSpace.MaterialId = space.MaterialId.ToString();

				convertedSpace.Color = space.Color;

				convertedSpace.Area = space.Area;

				convertedSpace.StartPoint = new Component.Point(space.StartPoint.X, space.StartPoint.Y, space.StartPoint.Z);
				convertedSpace.EndPoint = new Component.Point(space.EndPoint.X, space.EndPoint.Y, space.EndPoint.Z);

				//convertedSpace.Walls = 
				//convertedSpace.Surfaces = 
				//convertedSpace.TranslatedSurfaces = 

				convertedSpace.StyleHandle = space.StyleId.Handle;
				convertedSpace.StyleId = space.StyleId.ToString();

				convertedEntities.Spaces.Add(convertedSpace);
			}
		}

		public void ConvertZones(ACADEntities entities, Entities convertedEntities)
		{
			foreach (Zone zone in entities.Zones)
			{
				Component.Zone convertedZone= new Component.Zone();

				convertedZone.DisplayName = zone.DisplayName;

				convertedZone.BlockName = zone.BlockName;
				convertedZone.BlockId = zone.BlockId.ToString();

				convertedZone.ObjectId = zone.ObjectId.ToString();

				convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MaxPoint.X, zone.Bounds.Value.MaxPoint.Y, zone.Bounds.Value.MaxPoint.Z));
				convertedZone.Bounds.Add(new Component.Point(zone.Bounds.Value.MinPoint.X, zone.Bounds.Value.MinPoint.Y, zone.Bounds.Value.MinPoint.Z));

				convertedZone.Handle = zone.Handle;
				convertedZone.HandleId = zone.Handle.ToString();

				convertedZone.Layer = zone.Layer;
				convertedZone.LayerId = zone.LayerId.ToString();

				convertedZone.MaterialHandle = zone.MaterialId.Handle;
				convertedZone.MaterialId = zone.MaterialId.ToString();

				convertedZone.Color = zone.Color;

				convertedZone.Area = zone.Area;

				convertedZone.StartPoint = new Component.Point(zone.StartPoint.X, zone.StartPoint.Y, zone.StartPoint.Z);
				convertedZone.EndPoint = new Component.Point(zone.EndPoint.X, zone.EndPoint.Y, zone.EndPoint.Z);

				convertedZone.TotalNumberOfSpaces = zone.TotalNumberOfSpaces;
				convertedZone.TotalNumberOfZones = zone.TotalNumberOfZones;

				//convertedZone.Spaces = 
				//convertedZone.SpaceIds =
				//convertedZone.ZoneIds =

				convertedZone.StyleHandle = zone.StyleId.Handle;
				convertedZone.StyleId = zone.StyleId.ToString();

				convertedEntities.Zones.Add(convertedZone);
			}
		}
	}
}
