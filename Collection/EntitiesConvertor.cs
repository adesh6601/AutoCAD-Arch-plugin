using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
//using Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Collection
{
	public class EntitiesConvertor
	{
		public Entities Entities;
		public ConvertedEntities ConvertedEntities;

		public UnitsValue UnitsValue;

		public EntitiesConvertor(Entities entities, UnitsValue unitsValue)
		{
			Entities = entities;
			UnitsValue = unitsValue;
		}

		public ConvertedEntities ConvertEntities()
		{
			return null;
		}

		public void ConvertCurtainWalls()
		{
			foreach(CurtainWallLayout curtainWall in Entities.CurtainWalls)
			{
				Component.CurtainWall convertedCurtainWall = new Component.CurtainWall();

				//convertedCurtainWall.Name = 
				convertedCurtainWall.DisplayName = curtainWall.DisplayName;

				convertedCurtainWall.BlockName = curtainWall.BlockName;
				convertedCurtainWall.BlockId = curtainWall.BlockId.ToString();

				convertedCurtainWall.ObjectId = curtainWall.ObjectId.ToString();

				convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MaxPoint.X, curtainWall.Bounds.Value.MaxPoint.Y, curtainWall.Bounds.Value.MaxPoint.Z));
				convertedCurtainWall.Bounds.Add(new Component.Point(curtainWall.Bounds.Value.MinPoint.X, curtainWall.Bounds.Value.MinPoint.Y, curtainWall.Bounds.Value.MinPoint.Z));

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

				convertedCurtainWall.ObjectHandle = curtainWall.Handle;
				convertedCurtainWall.ObjectHandleId = curtainWall.Handle.ToString();

				convertedCurtainWall.StyleHandle = curtainWall.StyleId.Handle;
				convertedCurtainWall.Style = Entities.WallStyles[curtainWall.StyleId.Handle.ToString()].Name;
				convertedCurtainWall.StyleId = curtainWall.StyleId.ToString();

				Material curtainWallMaterial = Entities.Materials[curtainWall.MaterialId.Handle.ToString()];

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

				ConvertedEntities.CurtainWalls.Add(convertedCurtainWall);
			}
		}

		public void ConvertDoors()
		{
			foreach (Door door in Entities.Doors)
			{
				Component.Door convertedDoor = new Component.Door();

				//convertedDoor.Name = 
				convertedDoor.DisplayName = door.DisplayName;

				convertedDoor.BlockName = door.BlockName;
				convertedDoor.BlockId = door.BlockId.ToString();

				convertedDoor.ObjectId = door.ObjectId.ToString();

				convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MaxPoint.X, door.Bounds.Value.MaxPoint.Y, door.Bounds.Value.MaxPoint.Z));
				convertedDoor.Bounds.Add(new Component.Point(door.Bounds.Value.MinPoint.X, door.Bounds.Value.MinPoint.Y, door.Bounds.Value.MinPoint.Z));

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
				//convertedDoor.Offset = door.

				convertedDoor.CollisionType = door.CollisionType.ToString();

				convertedDoor.Handle = door.Handle;
				convertedDoor.HandleId = door.Handle.ToString();

				convertedDoor.StyleHandle = door.StyleId.Handle;
				convertedDoor.Style = Entities.DoorStyles[door.StyleId.Handle.ToString()].Name;
				convertedDoor.StyleId = door.StyleId.ToString();

				Material doorMaterial = Entities.Materials[door.MaterialId.Handle.ToString()];

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

				ConvertedEntities.Doors.Add(convertedDoor);
			}
		}
	}
}
