using System;
using UnityEngine;
using System.Collections.Generic;

public class UniverseData
{
	public struct Satelite 
	{
		public string Name;
		public Vector3 Position;
		public float Size;
		public Quaternion Rotation;
		public Color Color;
	}

	public struct Planet 
	{
		public string Name;
		public Vector3 Position;
		public float Size;
		public Quaternion Rotation;
		public Color Color;

		public float HoleBotLeftX;
		public float HoleBotLeftY;
		public float HoleTopRightX;
		public float HoleTopRightY;
	}

	#region universe data
	public Vector3 UniverseSize;
	public string Name;
	#endregion

	#region planets data
	public List<Satelite> Satelites;
	public List<Planet> Planets;
	#endregion

	public UniverseData()
	{
		Planets = new List<Planet>(4);
		Satelites = new List<Satelite>(16);
	}
}

