using Autodesk.AutoCAD.Runtime;
using Collection;
using Model;

namespace Plugin
{
    public class Plugin
    {
        public static string ProjectPath = "C:\\Users\\Adesh Lad\\Documents\\Autodesk\\My Projects\\Sample Project 2024\\Sample Project.apj";

        Reader Reader = new Reader();
        Builder Builder = new Builder();

        Entities Entities = new Entities();
        Building Building = new Building();

		[CommandMethod("Initiate")]
		public void InitiateDataCollection()
        {
            Reader.ReadEntities(ProjectPath, Entities);

            Builder.Build(Entities, Building);

            int breakpoint = 0;
		}
    }
}
