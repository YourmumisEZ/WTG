﻿using System;

namespace WhereTo_Go
{
	public class Coords
	{
		public string Longitude {
			get;
			set;
		}
		public string Latitutde {
			get;
			set;
		}
		public Coords ()
		{
			

		}
		public Coords (string longitude, string latitude)
		{
			Longitude=longitude;
			Latitutde=latitude;

		}
	}
}

