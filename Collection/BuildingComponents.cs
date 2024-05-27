using Autodesk.Aec.Arch.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
/*using Component.BuildingComponents;*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

/*using static Component.DataFormatter;
*/
namespace Collection
{
    public class BuildingComponents
    {
        public  Dictionary<string, Component.Wall> WallsDic { get; set; }

        public Dictionary<string, Component.CurtainWall> curtainWall { get; set; }
        public  Dictionary<string, Component.Window> Windows { get; set; }

        public Dictionary<string, Component.DoorwindowAssembly> doorwindowAssembly { get; set; }
        public  Dictionary<string, Component.Door> Doors { get; set; }
        public Dictionary<string, Component.Opening> openings { get; set; }
        public  Dictionary<string, Component.Space> Spaces{ get; set; }
        public  Dictionary<string, Component.Zone> Zones { get; set; }
        public Dictionary<string, Component.BlockReference> BlockReferences { get; set; }

        public Dictionary<string, Component.MultiViewBlockReference> MultiViewBlockReferences { get; set; }

        public string Path { get; set; }
        public Dictionary<string, List<string>> xRefs { get; set; }
        public Dictionary<string, List<string>> DivisionLevels { get; set; }


        public BuildingComponents( List<Autodesk.Aec.Arch.DatabaseServices.Wall> walls, List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout> curtainWallLayoutlist, List<Autodesk.Aec.Arch.DatabaseServices.Window> windows, List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly> windowAssembly, List<Autodesk.Aec.Arch.DatabaseServices.Door> doors, List<Autodesk.Aec.Arch.DatabaseServices.Opening> openingDict, List<Autodesk.Aec.Arch.DatabaseServices.Space> spaces, List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference> multiViewBlockReferences, List<Autodesk.AutoCAD.DatabaseServices.BlockReference> blockReferences, List<Autodesk.Aec.Arch.DatabaseServices.Zone> zones, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle> wallStyleDictionary, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle> curtainWallStylesDictionary  ,Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle> windowStyleDictionary, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle> windowAssemblyStylesDictionary, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle> doorStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDictionary, StringCollection xRef, Dictionary<string, List<string>> levelsAndDivision)
        {

            WallsDic = AddWallsToDictionary(walls, wallStyleDictionary, materialDictionary);
            curtainWall = AddCurtainWallLayoutToDictionary(curtainWallLayoutlist, curtainWallStylesDictionary,materialDictionary);
            doorwindowAssembly = AddDoorWindowAssemblyToDictionary(windowAssembly, windowAssemblyStylesDictionary, materialDictionary);
            openings = AddOpeningsToDictionary(openingDict, materialDictionary);
            Windows = AddWindowsToDictionary(windows, windowStyleDictionary, materialDictionary);
            Doors = AddDoorsToDictionary(doors, doorStyleDictionary, materialDictionary);
            Spaces = AddSpacesToDictionary(spaces);
            Zones = AddZonesToDictionary(zones);
            BlockReferences = AddBlockReferencesToDictionary(blockReferences);
            MultiViewBlockReferences = AddMultiViewBlockReferencesToDictionary(multiViewBlockReferences);
            xRefs = new Dictionary<string, List<string>>();
            xRefs = XRefsDic(xRef);
            DivisionLevels = new Dictionary<string, List<string>>();
            DivisionLevels = levelsAndDivision;
        }
        public Dictionary<string, List<string>> XRefsDic(StringCollection xRef)
        {
            Dictionary<string, List<string>> XRefsDic = new Dictionary<string, List<string>>(); 
            List<string> xRefsList = new List<string>();
            foreach(var name in xRef) 
            {
                xRefsList.Add(name);
            }
            XRefsDic.Add("External References", xRefsList);
            return XRefsDic;

        }
        public Dictionary<string, Component.Wall> AddWallsToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Wall> walls, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WallStyle> wallStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.Wall> Walls = new Dictionary<string, Component.Wall>();
            foreach (var wall in walls)
            {
                Component.Wall wallObj = new Component.Wall();
                wallObj.DisplayName = wall.DisplayName;
                wallObj.LayerId = wall.LayerId.OldId.ToString();
                wallObj.ObjectId = wall.ObjectId.OldId.ToString();
                wallObj.BlockId = wall.BlockId.OldId.ToString();
                wallObj.BlockName = wall.BlockName;
                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(wall.Bounds.Value.MaxPoint.X, wall.Bounds.Value.MaxPoint.Y, wall.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(wall.Bounds.Value.MinPoint.X, wall.Bounds.Value.MinPoint.Y, wall.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                wallObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(wall.StartPoint.X, wall.StartPoint.Y, wall.StartPoint.Z);
                wallObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(wall.EndPoint.X, wall.EndPoint.Y, wall.EndPoint.Z);
                wallObj.EndPoint = endPoint;
                wallObj.BaseHeight = wall.BaseHeight;
                wallObj.Width = wall.Width;
                wallObj.Layer = wall.Layer;
                wallObj.Length = wall.Length;
                wallObj.ObjectHandle = wall.Handle;
                wallObj.ObjectHandleId = wall.Handle.ToString();
                wallObj.MaterialId = wall.MaterialId.ToString();
                wallObj.StyleID = wall.StyleId.ToString();
                wallObj.StyleHandle = wall.StyleId.Handle;
                wallObj.MaterialHandle = wall.MaterialId.Handle;
                wallObj.Color = wall.Color;

                var wallStyle = wallStyleDictionary[wall.StyleId.Handle.ToString()];
                wallObj.Style = wallStyle.Name;

                var wallMaterial = materialDict[wall.MaterialId.Handle.ToString()];
                //wallObj.Opacity = wallMaterial.Opacity ;
                wallObj.Translucence = wallMaterial.Translucence;
                wallObj.SelfIllumination = wallMaterial.SelfIllumination;
                wallObj.Reflectivity = wallMaterial.Reflectivity;
                wallObj.ColorBleedScale = wallMaterial.ColorBleedScale;
                wallObj.IndirectBumpScale = wallMaterial.IndirectBumpScale;
                wallObj.ReflectanceScale = wallMaterial.ReflectanceScale;
                wallObj.TransmittanceScale = wallMaterial.TransmittanceScale;
                wallObj.TwoSided = wallMaterial.TwoSided;
                wallObj.Luminance = wallMaterial.Luminance;
                wallObj.Description = wallMaterial.Description;
                wallObj.MaterialName = wallMaterial.Name;
                //wallObj.Diffuse = wallMaterial.Diffuse;
                wallObj.Ambient = wallMaterial.Ambient;

                Walls.Add(wallObj.ObjectId.ToString(), wallObj);
                 
            }
            return Walls;
        }

        public Dictionary<string, Component.CurtainWall> AddCurtainWallLayoutToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayout> walls, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.CurtainWallLayoutStyle> wallStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.CurtainWall> Walls = new Dictionary<string, Component.CurtainWall>();
            foreach (var wall in walls)
            {
                Component.CurtainWall wallObj = new Component.CurtainWall();
                wallObj.DisplayName = wall.DisplayName;
                wallObj.LayerId = wall.LayerId.OldId.ToString();
                wallObj.ObjectId = wall.ObjectId.OldId.ToString();
                wallObj.BlockId = wall.BlockId.OldId.ToString();
                wallObj.BlockName = wall.BlockName;
                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(wall.Bounds.Value.MaxPoint.X, wall.Bounds.Value.MaxPoint.Y, wall.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(wall.Bounds.Value.MinPoint.X, wall.Bounds.Value.MinPoint.Y, wall.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                wallObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(wall.StartPoint.X, wall.StartPoint.Y, wall.StartPoint.Z);
                wallObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(wall.EndPoint.X, wall.EndPoint.Y, wall.EndPoint.Z);
                wallObj.EndPoint = endPoint;
                wallObj.BaseHeight = wall.BaseHeight;
                //wallObj.Width = wall.Width;
                wallObj.Layer = wall.Layer;
                wallObj.Length = wall.Length;
                wallObj.ObjectHandle = wall.Handle;
                wallObj.ObjectHandleId = wall.Handle.ToString();
                wallObj.MaterialId = wall.MaterialId.ToString();
                wallObj.StyleID = wall.StyleId.ToString();
                wallObj.StyleHandle = wall.StyleId.Handle;
                wallObj.MaterialHandle = wall.MaterialId.Handle;
                wallObj.Color = wall.Color;

                var wallStyle = wallStyleDictionary[wall.StyleId.Handle.ToString()];
                wallObj.Style = wallStyle.Name;

                var wallMaterial = materialDict[wall.MaterialId.Handle.ToString()];
                //wallObj.Opacity = wallMaterial.Opacity ;
                wallObj.Translucence = wallMaterial.Translucence;
                wallObj.SelfIllumination = wallMaterial.SelfIllumination;
                wallObj.Reflectivity = wallMaterial.Reflectivity;
                wallObj.ColorBleedScale = wallMaterial.ColorBleedScale;
                wallObj.IndirectBumpScale = wallMaterial.IndirectBumpScale;
                wallObj.ReflectanceScale = wallMaterial.ReflectanceScale;
                wallObj.TransmittanceScale = wallMaterial.TransmittanceScale;
                wallObj.TwoSided = wallMaterial.TwoSided;
                wallObj.Luminance = wallMaterial.Luminance;
                wallObj.Description = wallMaterial.Description;
                wallObj.MaterialName = wallMaterial.Name;
                //wallObj.Diffuse = wallMaterial.Diffuse;
                wallObj.Ambient = wallMaterial.Ambient;
                wallObj.CellCount = wall.CellCount;
                Walls.Add(wallObj.ObjectId.ToString(), wallObj);

            }
            return Walls;
        }
        public Dictionary<string, Component.Window> AddWindowsToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Window> windows, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowStyle> windowStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.Window>  windowsDictionary = new Dictionary<string, Component.Window>();
            foreach (var window in windows)
            {
                Component.Window windowObj = new Component.Window();
                windowObj.DisplayName = window.DisplayName;
                windowObj.LayerId = window.LayerId.OldId.ToString();
                windowObj.ObjectId = window.ObjectId.OldId.ToString();
                windowObj.BlockId = window.BlockId.OldId.ToString();
                windowObj.BlockName = window.BlockName;
                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(window.Bounds.Value.MaxPoint.X, window.Bounds.Value.MaxPoint.Y, window.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(window.Bounds.Value.MinPoint.X, window.Bounds.Value.MinPoint.Y, window.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                windowObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(window.StartPoint.X, window.StartPoint.Y, window.StartPoint.Z);
                windowObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(window.EndPoint.X, window.EndPoint.Y, window.EndPoint.Z);
                windowObj.EndPoint = endPoint;

                windowObj.Height = window.Height;
                windowObj.Width = window.Width;
                windowObj.Layer = window.Layer;
                windowObj.Area = window.Area;
                windowObj.Handle = window.Handle;
                windowObj.HandleId = window.Handle.ToString();
                windowObj.MaterialId = window.MaterialId.ToString();
                windowObj.StyleID = window.StyleId.ToString();
                windowObj.StyleHandle = window.StyleId.Handle;
                windowObj.MaterialHandle = window.MaterialId.Handle;
                windowObj.Color = window.Color;
                foreach (var wall in WallsDic)
                {

                    if (IntersectBoundingBoxes((Extents3d)window.Bounds, wall.Value.Bounds))
                    {
                        Component.Point point1 = new Component.Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
						Component.Point point2 = new Component.Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);



                        Component.Point doorStart = new Component.Point(window.StartPoint.X, window.StartPoint.Y, 0.0);
                        List<Component.Point> points = new List<Component.Point>();
                        points.Add(doorStart);
                        points.Add(point1);
                        List<Component.Point> wallLength = new List<Component.Point>();
                        wallLength.Add(point2);
                        wallLength.Add(point1);
                        //windowHole.offset = 0.5123508399999992;
                        double offset = GetLength(points) / GetLength(wallLength);
                        windowObj.Offset = offset;
                        foreach (var line in WallsDic)
                        {

                            if (
                                 (Math.Abs(point1.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.StartPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.EndPoint.Y) < 0.01))
                            {
                                windowObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                            if (
                                (Math.Abs(point1.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.EndPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.StartPoint.Y) < 0.01))
                            {
                                windowObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                        }

                    }

                }
                Component.Point Normal = new Component.Point( window.Normal.X, window.Normal.Y, window.Normal.Z);
                windowObj.Normal = Normal;
                /*doorObj.Thickness = door.Thickness;*/


                var wawindowStyle = windowStyleDictionary[window.StyleId.Handle.ToString()];
                windowObj.Style = wawindowStyle.Name;

                var windowMaterial = materialDict[window.MaterialId.Handle.ToString()];
                //windowObj.Opacity = windowMaterial.Opacity;
                windowObj.Translucence = windowMaterial.Translucence;
                windowObj.SelfIllumination = windowMaterial.SelfIllumination;
                windowObj.Reflectivity = windowMaterial.Reflectivity;
                windowObj.ColorBleedScale = windowMaterial.ColorBleedScale;
                windowObj.IndirectBumpScale = windowMaterial.IndirectBumpScale;
                windowObj.ReflectanceScale = windowMaterial.ReflectanceScale;
                windowObj.TransmittanceScale = windowMaterial.TransmittanceScale;
                windowObj.TwoSided = windowMaterial.TwoSided;
                windowObj.Luminance = windowMaterial.Luminance;
                windowObj.Description = windowMaterial.Description;
                windowObj.MaterialName = windowMaterial.Name;
                //windowObj.Diffuse = windowMaterial.Diffuse;
                windowObj.Ambient = windowMaterial.Ambient;
                windowsDictionary.Add(windowObj.ObjectId.ToString(), windowObj);



            }
            return windowsDictionary;
        }
        public Dictionary<string, Component.Door> AddDoorsToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Door> doors, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.DoorStyle> doorStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.Door>  doorsDictionary = new Dictionary<string, Component.Door>();
            foreach (var door in doors)
            {
                Component.Door doorObj = new Component.Door();
                doorObj.DisplayName = door.DisplayName;
                doorObj.LayerId = door.LayerId.OldId.ToString();
                doorObj.ObjectId = door.ObjectId.OldId.ToString();
                doorObj.BlockId = door.BlockId.OldId.ToString();
                doorObj.BlockName = door.BlockName;

                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(door.Bounds.Value.MaxPoint.X, door.Bounds.Value.MaxPoint.Y, door.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(door.Bounds.Value.MinPoint.X, door.Bounds.Value.MinPoint.Y, door.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                doorObj.Bounds = bounds;
                doorObj.CollisionType = door.CollisionType.ToString();
                Component.Point startPoint = new Component.Point(door.StartPoint.X, door.StartPoint.Y, door.StartPoint.Z);
                doorObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(door.EndPoint.X, door.EndPoint.Y, door.EndPoint.Z);
                doorObj.EndPoint = endPoint;
                doorObj.HandleId = door.Handle.ToString();
                doorObj.Handle = door.Handle;
                doorObj.MaterialId = door.MaterialId.ToString();
                doorObj.StyleID = door.StyleId.ToString();
                doorObj.StyleHandle = door.StyleId.Handle;
                doorObj.MaterialHandle = door.MaterialId.Handle;
                doorObj.Color = door.Color;
                foreach (var wall in WallsDic)
                {

                    if (IntersectBoundingBoxes((Extents3d)door.Bounds, wall.Value.Bounds))
                    {
                        Component.Point point1 = new Component.Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
                        Component.Point point2 = new Component.Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);

                        
                       
                        Component.Point doorStart = new Component.Point(door.StartPoint.X, door.StartPoint.Y, 0.0);
                        List<Component.Point> points = new List<Component.Point>();
                        points.Add(doorStart);
                        points.Add(point1);
                        List<Component.Point> wallLength = new List<Component.Point>();
                        wallLength.Add(point2);
                        wallLength.Add(point1);
                        //windowHole.offset = 0.5123508399999992;
                        double offset = GetLength(points) / GetLength(wallLength);
                        doorObj.Offset = offset;
                        foreach (var line in WallsDic)
                        {

                            if (
                                 (Math.Abs(point1.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.StartPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.EndPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                            if (
                                (Math.Abs(point1.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.EndPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.StartPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                        }
                       
                    }

                }
               

                doorObj.Height = door.Height;
                doorObj.Width = door.Width;
                doorObj.Layer = door.Layer;
                doorObj.Area = door.Area;
                Component.Point Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);
                doorObj.Normal = Normal;
                /*doorObj.Thickness = door.Thickness;*/


                var doorStyle = doorStyleDictionary[door.StyleId.Handle.ToString()];
                doorObj.Style = doorStyle.Name;

                var doorMaterial = materialDict[door.MaterialId.Handle.ToString()];
                //doorObj.Opacity = doorMaterial.Opacity;
                doorObj.Translucence = doorMaterial.Translucence;
                doorObj.SelfIllumination = doorMaterial.SelfIllumination;
                doorObj.Reflectivity = doorMaterial.Reflectivity;
                doorObj.ColorBleedScale = doorMaterial.ColorBleedScale;
                doorObj.IndirectBumpScale = doorMaterial.IndirectBumpScale;
                doorObj.ReflectanceScale = doorMaterial.ReflectanceScale;
                doorObj.TransmittanceScale = doorMaterial.TransmittanceScale;
                doorObj.TwoSided = doorMaterial.TwoSided;
                doorObj.Luminance = doorMaterial.Luminance;
                doorObj.Description = doorMaterial.Description;
                doorObj.MaterialName = doorMaterial.Name;
                //doorObj.Diffuse = doorMaterial.Diffuse;
                doorObj.Ambient = doorMaterial.Ambient;
                doorsDictionary.Add(doorObj.ObjectId.ToString(), doorObj);

            }
            return doorsDictionary;
        }

        public Dictionary<string, Component.DoorwindowAssembly> AddDoorWindowAssemblyToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.WindowAssembly> doors, Dictionary<string, Autodesk.Aec.Arch.DatabaseServices.WindowAssemblyStyle> doorStyleDictionary, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.DoorwindowAssembly> doorsDictionary = new Dictionary<string, Component.DoorwindowAssembly>();
            foreach (var door in doors)
            {
                Component.DoorwindowAssembly doorObj = new Component.DoorwindowAssembly();
                doorObj.DisplayName = door.DisplayName;
                doorObj.LayerId = door.LayerId.OldId.ToString();
                doorObj.ObjectId = door.ObjectId.OldId.ToString();
                doorObj.BlockId = door.BlockId.OldId.ToString();
                doorObj.BlockName = door.BlockName;

                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(door.Bounds.Value.MaxPoint.X, door.Bounds.Value.MaxPoint.Y, door.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(door.Bounds.Value.MinPoint.X, door.Bounds.Value.MinPoint.Y, door.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                doorObj.Bounds = bounds;
                doorObj.CollisionType = door.CollisionType.ToString();
                Component.Point startPoint = new Component.Point(door.StartPoint.X, door.StartPoint.Y, door.StartPoint.Z);
                doorObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(door.EndPoint.X, door.EndPoint.Y, door.EndPoint.Z);
                doorObj.EndPoint = endPoint;
                doorObj.HandleId = door.Handle.ToString();
                doorObj.Handle = door.Handle;
                doorObj.MaterialId = door.MaterialId.ToString();
                doorObj.StyleID = door.StyleId.ToString();
                doorObj.StyleHandle = door.StyleId.Handle;
                doorObj.MaterialHandle = door.MaterialId.Handle;
                doorObj.Color = door.Color;
                doorObj.CollisionType = door.CollisionType.ToString();
                doorObj.CellCount = door.CellCount;

                foreach (var wall in WallsDic)
                {

                    if (IntersectBoundingBoxes((Extents3d)door.Bounds, wall.Value.Bounds))
                    {
                        Component.Point point1 = new Component.Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
                        Component.Point point2 = new Component.Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);



                        Component.Point doorStart = new Component.Point(door.StartPoint.X, door.StartPoint.Y, 0.0);
                        List<Component.Point> points = new List<Component.Point>();
                        points.Add(doorStart);
                        points.Add(point1);
                        List<Component.Point> wallLength = new List<Component.Point>();
                        wallLength.Add(point2);
                        wallLength.Add(point1);
                        //windowHole.offset = 0.5123508399999992;
                        double offset = GetLength(points) / GetLength(wallLength);
                        doorObj.Offset = offset;
                        foreach (var line in WallsDic)
                        {

                            if (
                                 (Math.Abs(point1.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.StartPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.EndPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                            if (
                                (Math.Abs(point1.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.EndPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.StartPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                        }

                    }

                }


                doorObj.Height = door.Height;
               // doorObj.Width = door.Width;
                doorObj.Layer = door.Layer;
                doorObj.Area = door.Area;
                Component.Point Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);
                doorObj.Normal = Normal;
                /*doorObj.Thickness = door.Thickness;*/


                var doorStyle = doorStyleDictionary[door.StyleId.Handle.ToString()];
                doorObj.Style = doorStyle.Name;

                var doorMaterial = materialDict[door.MaterialId.Handle.ToString()];
                //doorObj.Opacity = doorMaterial.Opacity;
                doorObj.Translucence = doorMaterial.Translucence;
                doorObj.SelfIllumination = doorMaterial.SelfIllumination;
                doorObj.Reflectivity = doorMaterial.Reflectivity;
                doorObj.ColorBleedScale = doorMaterial.ColorBleedScale;
                doorObj.IndirectBumpScale = doorMaterial.IndirectBumpScale;
                doorObj.ReflectanceScale = doorMaterial.ReflectanceScale;
                doorObj.TransmittanceScale = doorMaterial.TransmittanceScale;
                doorObj.TwoSided = doorMaterial.TwoSided;
                doorObj.Luminance = doorMaterial.Luminance;
                doorObj.Description = doorMaterial.Description;
                doorObj.MaterialName = doorMaterial.Name;
                //doorObj.Diffuse = doorMaterial.Diffuse;
                doorObj.Ambient = doorMaterial.Ambient;
                doorsDictionary.Add(doorObj.ObjectId.ToString(), doorObj);

            }
            return doorsDictionary;
        }


        public Dictionary<string, Component.Opening> AddOpeningsToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Opening> doors, Dictionary<string, Autodesk.AutoCAD.DatabaseServices.Material> materialDict)
        {
            Dictionary<string, Component.Opening> doorsDictionary = new Dictionary<string, Component.Opening>();
            foreach (var door in doors)
            {
                Component.Opening doorObj = new Component.Opening();
                doorObj.DisplayName = door.DisplayName;
                doorObj.LayerId = door.LayerId.OldId.ToString();
                doorObj.ObjectId = door.ObjectId.OldId.ToString();
                //doorObj.BlockId = door.BlockId.OldId.ToString();
                //doorObj.BlockName = door.BlockName;

                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(door.Bounds.Value.MaxPoint.X, door.Bounds.Value.MaxPoint.Y, door.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(door.Bounds.Value.MinPoint.X, door.Bounds.Value.MinPoint.Y, door.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                doorObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(door.StartPoint.X, door.StartPoint.Y, door.StartPoint.Z);
                doorObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(door.EndPoint.X, door.EndPoint.Y, door.EndPoint.Z);
                doorObj.EndPoint = endPoint;
                doorObj.HandleId = door.Handle.ToString();
                doorObj.Handle = door.Handle;
                doorObj.MaterialId = door.MaterialId.ToString();
                doorObj.CollisionType = door.CollisionType.ToString();
                doorObj.ShapeType = door.ShapeType.ToString();
/*                doorObj.StyleID = door.StyleId.ToString();
                doorObj.StyleHandle = door.StyleId.Handle;*/
                doorObj.MaterialHandle = door.MaterialId.Handle;
                doorObj.Color = door.Color;
                doorObj.LineTypeID = door.LinetypeId.ToString();
                foreach (var wall in WallsDic)
                {

                    if (IntersectBoundingBoxes((Extents3d)door.Bounds, wall.Value.Bounds))
                    {
                        Component.Point point1 = new Component.Point(wall.Value.StartPoint.X, wall.Value.StartPoint.Y, wall.Value.StartPoint.Z);
                        Component.Point point2 = new Component.Point(wall.Value.EndPoint.X, wall.Value.EndPoint.Y, wall.Value.EndPoint.Z);



                        Component.Point doorStart = new Component.Point(door.StartPoint.X, door.StartPoint.Y, 0.0);
                        List<Component.Point> points = new List<Component.Point>();
                        points.Add(doorStart);
                        points.Add(point1);
                        List<Component.Point> wallLength = new List<Component.Point>();
                        wallLength.Add(point2);
                        wallLength.Add(point1);
                        //windowHole.offset = 0.5123508399999992;
                        double offset = GetLength(points) / GetLength(wallLength);
                        doorObj.Offset = offset;
                        foreach (var line in WallsDic)
                        {

                            if (
                                 (Math.Abs(point1.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.StartPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.EndPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                            if (
                                (Math.Abs(point1.X - line.Value.EndPoint.X) < 0.01
                                && Math.Abs(point1.Y - line.Value.EndPoint.Y) < 0.01)
                                && (Math.Abs(point2.X - line.Value.StartPoint.X) < 0.01
                                && Math.Abs(point2.Y - line.Value.StartPoint.Y) < 0.01))
                            {
                                doorObj.WallId = wall.Value.ObjectId;
                                break;
                            }
                        }

                    }

                }


                doorObj.Height = door.Height;
                doorObj.Width = door.Width;
                doorObj.Layer = door.Layer;
                doorObj.Area = door.Area;
                Component.Point Normal = new Component.Point(door.Normal.X, door.Normal.Y, door.Normal.Z);
                doorObj.Normal = Normal;
                /*doorObj.Thickness = door.Thickness;*/


                
                doorsDictionary.Add(doorObj.ObjectId.ToString(), doorObj);

            }
            return doorsDictionary;
        }

        public Dictionary<string, Component.Space> AddSpacesToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Space> spaces)
        {
            Dictionary<string, Component.Space>  spacesDictionary = new Dictionary<string, Component.Space>();
            foreach (var space in spaces)
            {
                Component.Space spaceObj = new Component.Space();

                spaceObj.DisplayName = space.DisplayName;
                spaceObj.LayerId = space.LayerId.OldId.ToString();
                spaceObj.ObjectId = space.ObjectId.OldId.ToString();
                spaceObj.BlockId = space.BlockId.OldId.ToString();
                spaceObj.BlockName = space.BlockName;
                spaceObj.Name = space.Name;
                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(space.Bounds.Value.MaxPoint.X, space.Bounds.Value.MaxPoint.Y, space.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(space.Bounds.Value.MinPoint.X, space.Bounds.Value.MinPoint.Y, space.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                spaceObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(space.StartPoint.X, space.StartPoint.Y, space.StartPoint.Z);
                spaceObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(space.EndPoint.X, space.EndPoint.Y, space.EndPoint.Z);
                spaceObj.EndPoint = endPoint;
                spaceObj.Area = space.Area;
                spaceObj.Layer = space.Layer;
                spaceObj.HandleId = space.Handle.ToString();
                spaceObj.Handle = space.Handle;
                spaceObj.MaterialId = space.MaterialId.ToString();
                spaceObj.StyleID = space.StyleId.ToString();
                spaceObj.StyleHandle = space.StyleId.Handle;
                spaceObj.MaterialHandle = space.MaterialId.Handle;
                spaceObj.Color = space.Color;
                var surfaces = space.Surfaces;
                
                List<List<List<double>>> walls = new List<List<List<double>>>();

                foreach (var surface in surfaces)
                {
                    var wall = (Autodesk.Aec.Arch.DatabaseServices.SpaceSurface)surface;
                    List<double> start = new List<double>();
                    List<double> end = new List<double>();
                    List<List<double>> wallCoordinates = new List<List<double>>();
                    start.Add(wall.StartPoint.X);
                    start.Add(wall.StartPoint.Y);
                    end.Add(wall.EndPoint.X);
                    end.Add(wall.EndPoint.Y);
                    wallCoordinates.Add(start);
                    wallCoordinates.Add(end);
                    walls.Add(wallCoordinates);
                }

                spaceObj.Surfaces = walls;
                //spaceObj.translatedSurfaces = GetTranslatedCoordinatesOfWall(walls,spaceObj.Bounds);
                

                spacesDictionary.Add(spaceObj.ObjectId.ToString(), spaceObj);
                // Pseudo-code for transforming wall coordinates to match their associated spaces


                
            }
            
            return spacesDictionary;
        }
        public Dictionary<string, Component.Zone> AddZonesToDictionary(List<Autodesk.Aec.Arch.DatabaseServices.Zone> zones)
        {
            Dictionary<string, Component.Zone>  zonesDictionary  = new Dictionary<string, Component.Zone>();
            foreach (var zone in zones)
            {
                var SpacesList = zone.Spaces;
                var Zones = zone.Zones;
                List<string> ids = new List<string>();
                List<Component.Space> spacesList = new List<Component.Space>();
                foreach (var space in SpacesList)
                {

                    ids.Add(space.ToString().Substring(1, space.ToString().Length - 2));
                    spacesList.Add(Spaces[space.ToString().Substring(1, space.ToString().Length - 2)]);
                }
                List<string> zoneIds = new List<string>();
                foreach (var z in Zones)
                {

                    zoneIds.Add(z.ToString().Substring(1, z.ToString().Length - 2));

                }

                Component.Zone zoneObj = new Component.Zone();
                zoneObj.Name = zone.Name;
                zoneObj.DisplayName = zone.DisplayName;
                zoneObj.LayerId = zone.LayerId.OldId.ToString();
                zoneObj.ObjectId = zone.ObjectId.OldId.ToString();
                zoneObj.BlockId = zone.BlockId.OldId.ToString();
                zoneObj.BlockName = zone.BlockName;
                zoneObj.HandleId = zone.Handle.ToString();
                zoneObj.Handle = zone.Handle;
                zoneObj.MaterialId = zone.MaterialId.ToString();
                zoneObj.StyleID = zone.StyleId.ToString();
                zoneObj.StyleHandle = zone.StyleId.Handle;
                zoneObj.MaterialHandle = zone.MaterialId.Handle;
                zoneObj.Color = zone.Color;
                List<Component.Point> bounds = new List<Component.Point>();
                Component.Point maxPoint = new Component.Point(zone.Bounds.Value.MaxPoint.X, zone.Bounds.Value.MaxPoint.Y, zone.Bounds.Value.MaxPoint.Z);
                Component.Point minPoint = new Component.Point(zone.Bounds.Value.MinPoint.X, zone.Bounds.Value.MinPoint.Y, zone.Bounds.Value.MinPoint.Z);
                bounds.Add(minPoint);
                bounds.Add(maxPoint);
                zoneObj.Bounds = bounds;

                Component.Point startPoint = new Component.Point(zone.StartPoint.X, zone.StartPoint.Y, zone.StartPoint.Z);
                zoneObj.StartPoint = startPoint;
                Component.Point endPoint = new Component.Point(zone.EndPoint.X, zone.EndPoint.Y, zone.EndPoint.Z);
                zoneObj.EndPoint = endPoint;
                zoneObj.SpaceIds = ids;
                zoneObj.ZoneIds = zoneIds;
                zoneObj.TotalNumberOfSpaces = zone.TotalNumberOfSpaces;
                zoneObj.TotalNumberOfZones = zone.TotalNumberOfZones;
                zoneObj.Area = zone.Area;
                zoneObj.Layer = zone.Layer;
                zoneObj.Spaces = spacesList;
                zonesDictionary.Add(zoneObj.ObjectId.ToString(), zoneObj);
            }
            return zonesDictionary;

        }
        public Dictionary<string, Component.BlockReference> AddBlockReferencesToDictionary(List<Autodesk.AutoCAD.DatabaseServices.BlockReference> blockReferences)
        {
            Dictionary<string, Component.BlockReference> blockReferenceDictionary = new Dictionary<string, Component.BlockReference>();
            foreach (var blockReference in blockReferences)
            {
                try 
                {
                    Component.BlockReference wallObj = new Component.BlockReference();
                    wallObj.DisplayName = blockReference.Name;
                    wallObj.LayerId = blockReference.LayerId.OldId.ToString();
                    wallObj.ObjectId = blockReference.ObjectId.OldId.ToString();
                    wallObj.BlockId = blockReference.BlockId.OldId.ToString();
                    wallObj.BlockName = blockReference.BlockName;
                    //wallObj.BlockUnit = blockReference.BlockUnit.ToString();
                    List<Component.Point> bounds = new List<Component.Point>();
                    Component.Point maxPoint = new Component.Point(blockReference.Bounds.Value.MaxPoint.X, blockReference.Bounds.Value.MaxPoint.Y, blockReference.Bounds.Value.MaxPoint.Z);
                    Component.Point minPoint = new Component.Point(blockReference.Bounds.Value.MinPoint.X, blockReference.Bounds.Value.MinPoint.Y, blockReference.Bounds.Value.MinPoint.Z);
                    bounds.Add(minPoint);
                    bounds.Add(maxPoint);
                    wallObj.Bounds = bounds;
                    wallObj.HandleId = blockReference.Handle.ToString();
                    wallObj.Handle = blockReference.Handle;
                    wallObj.MaterialId = blockReference.MaterialId.ToString();
                    wallObj.MaterialHandle = blockReference.MaterialId.Handle;
                    wallObj.Color = blockReference.Color;
                    Component.Point position = new Component.Point(blockReference.Position.X, blockReference.Position.Y, blockReference.Position.Z);
                    wallObj.Position = position;
                    Component.Point scaleFactor = new Component.Point(blockReference.ScaleFactors.X, blockReference.ScaleFactors.Y, blockReference.ScaleFactors.Z);
                    wallObj.ScaleFactor = scaleFactor;
                    wallObj.Rotation = blockReference.Rotation;
                    wallObj.Layer = blockReference.Layer;
                    blockReferenceDictionary.Add(wallObj.ObjectId.ToString(), wallObj);
                }catch(Exception ex) 
                {
                    continue;
                }
                
            }
            return blockReferenceDictionary;
        }

        public Dictionary<string, Component.MultiViewBlockReference> AddMultiViewBlockReferencesToDictionary(List<Autodesk.Aec.DatabaseServices.MultiViewBlockReference> multiViewBlockReferences)
        {
            Dictionary<string, Component.MultiViewBlockReference> multiViewBlockReferenceDictionary = new Dictionary<string, Component.MultiViewBlockReference>();
            foreach (var blockReference in multiViewBlockReferences)
            {
                try
                {
                    Component.MultiViewBlockReference wallObj = new Component.MultiViewBlockReference();
                    wallObj.DisplayName = blockReference.DisplayName;
                    wallObj.LayerId = blockReference.LayerId.OldId.ToString();
                    wallObj.ObjectId = blockReference.ObjectId.OldId.ToString();
                    wallObj.BlockId = blockReference.BlockId.OldId.ToString();
                    wallObj.BlockName = blockReference.BlockName;
                    //wallObj.BlockUnit = blockReference.BlockUnit.ToString();
                    List<Component.Point> bounds = new List<Component.Point>();
                    Component.Point maxPoint = new Component.Point(blockReference.Bounds.Value.MaxPoint.X, blockReference.Bounds.Value.MaxPoint.Y, blockReference.Bounds.Value.MaxPoint.Z);
                    Component.Point minPoint = new Component.Point(blockReference.Bounds.Value.MinPoint.X, blockReference.Bounds.Value.MinPoint.Y, blockReference.Bounds.Value.MinPoint.Z);
                    bounds.Add(minPoint);
                    bounds.Add(maxPoint);
                    wallObj.Bounds = bounds;
                    wallObj.HandleId = blockReference.Handle.ToString();
                    wallObj.Handle = blockReference.Handle;
                    wallObj.MaterialId = blockReference.MaterialId.ToString();
                    wallObj.StyleID = blockReference.StyleId.ToString();
                    wallObj.StyleHandle = blockReference.StyleId.Handle;
                    wallObj.MaterialHandle = blockReference.MaterialId.Handle;
                    wallObj.Color = blockReference.Color;
                    Component.Point startPoint = new Component.Point(blockReference.StartPoint.X, blockReference.StartPoint.Y, blockReference.StartPoint.Z);
                    wallObj.StartPoint = startPoint;
                    Component.Point endPoint = new Component.Point(blockReference.EndPoint.X, blockReference.EndPoint.Y, blockReference.EndPoint.Z);
                    wallObj.EndPoint = endPoint;
                    wallObj.Rotation = blockReference.Rotation;
                    wallObj.Layer = blockReference.Layer;
                    multiViewBlockReferenceDictionary.Add(wallObj.ObjectId.ToString(), wallObj);
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            return multiViewBlockReferenceDictionary;
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
        public List<List<List<double>>> GetTranslatedCoordinatesOfWall(List<List<List<double>>> walls, List<Component.Point> Bounds)
        {
            List<List<List<double>>> translatedWallsList = new List<List<List<double>>>();
            List<double> minPoint = new List<double>();
            minPoint.Add(double.MaxValue);
            minPoint.Add(double.MaxValue);
            List<double> result = new List<double>();
            result.Add(double.MaxValue);
            result.Add(double.MaxValue);
            foreach (var wall in walls)
            {
                //var wall = (Autodesk.Aec.Arch.DatabaseServices.SpaceSurface)surface;
                List<double> startPoint = new List<double>();
                List<double> endPoint = new List<double>();

                startPoint.Add(wall[0][0]);
                startPoint.Add(wall[0][1]);
                endPoint.Add(wall[1][0]);
                endPoint.Add(wall[1][1]);

                minPoint = ComparePoints(minPoint, startPoint, result);
                minPoint = ComparePoints(minPoint, endPoint, result);



            }

            var  dx = Bounds[0].X - minPoint[0];
            var dy = Bounds[0].Y - minPoint[1];

            foreach(var wall in walls)
            {
                List<List<double>> translatedWall = new List<List<double>>();
                List<double> startPoint = new List<double>();
                startPoint.Add(wall[0][0] + dx);
                startPoint.Add(wall[0][1] + dy);
                List<double> endPoint = new List<double>();
                endPoint.Add(wall[1][0] + dx);
                endPoint.Add(wall[1][1] + dy);
                translatedWall.Add(startPoint);
                translatedWall.Add(endPoint);

                translatedWallsList.Add(translatedWall);
            }

            /*            List<Component.Wall> wallsList = new List<Component.Wall>();
                        foreach (var wall in walls)
                        {
                            foreach (var line in WallsDic)
                            {
                                if(( Math.Abs(line.Value.StartPoint.X - (wall[0][0] + dx)) <= 1.5 && Math.Abs(line.Value.StartPoint.Y - (wall[0][1]+dy)) <= 1.5) &&
                                    (Math.Abs(line.Value.EndPoint.X - (wall[1][0] + dx)) <= 1.5 && Math.Abs(line.Value.EndPoint.Y - (wall[1][1]+dy)) <= 1.5))
                                    {
                                        wallsList.Add(WallsDic[line.Key]);
                                        break;
                                    }
                                else if((Math.Abs(line.Value.EndPoint.X - (wall[0][0] + dx)) <= 1.5 && Math.Abs(line.Value.EndPoint.Y - (wall[0][1] + dy)) <= 1.5) &&
                                    (Math.Abs(line.Value.StartPoint.X - (wall[1][0] + dx)) <= 1.5 && Math.Abs(line.Value.StartPoint.Y - (wall[1][1] + dy)) <= 1.5))
                                    {
                                        wallsList.Add(WallsDic[line.Key]);
                                        break;
                                    }
                            }

               
                        }
            
                        return wallsList;*/

            return translatedWallsList;
        }
        public static double GetLength(List<Component.Point> inPoint)
        {
            var length = Math.Pow(Math.Pow((inPoint[1].X - inPoint[0].X), 2) + Math.Pow((inPoint[1].Y - inPoint[0].Y), 2), 0.5);
            return length;
        }
        public static bool IntersectBoundingBoxes(Extents3d ext1, List<Component.Point> ext2)
        {

            // Check for intersection along X axis
            if (ext1.MaxPoint.X < ext2[0].X || ext2[1].X < ext1.MinPoint.X)
                return false;

            // Check for intersection along Y axis
            if (ext1.MaxPoint.Y < ext2[0].Y || ext2[1].Y < ext1.MinPoint.Y)
                return false;

            // If no axis has a gap, the bounding boxes intersect
            return true;
        }

    }
}
