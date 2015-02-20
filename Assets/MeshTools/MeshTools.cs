using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MeshTools {

	[RequireComponent(typeof(MeshCollider))]
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class MeshTools : MonoBehaviour
	{
		public Material DisplayMaterial;

		[Range(0.001f, 0.1f)]
		public float DisplaySpacing;

		public List<PolygonArea> PolygonAreas;

		private void Awake()
		{
			if(PolygonAreas == null)
				PolygonAreas = new List<PolygonArea>();
		}
	}

}
