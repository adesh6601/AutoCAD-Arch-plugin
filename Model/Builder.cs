using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collection;

namespace Model
{
	public class Builder
	{
		public Building Building;
		public Floor Floor;
		public ConvertedEntities Entites;

		public Builder(ConvertedEntities entites)
		{
			Entites = entites;
		}

		public void Build()
		{

		}

	}
}
