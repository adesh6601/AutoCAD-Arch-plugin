using Autodesk.AutoCAD.Runtime;
using Collection;
using Model;

namespace Plugin
{
    public class Plugin
    {
        public static string ProjectPath = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2024\\Sample Project.apj";

        Reader Reader = new Reader(ProjectPath);

        Entities Entities;
        ConvertedEntities ConvertedEntities;

        Building Building;

		[CommandMethod("Initiate Data Collection")]
		public void InitiateDataCollection()
        {
            Reader.ReadEntities();

            Entities = Reader.GetEntities();

            EntitiesConvertor entitiesConvertor = new EntitiesConvertor(Entities);
            entitiesConvertor.ConvertEntities();

			ConvertedEntities = entitiesConvertor.GetConvertedEntities();

            Builder builder = new Builder(ConvertedEntities);
            builder.Build();

			Building = builder.GetBuilding();
		}
    }
}
