using UnityEngine;
using System;

namespace MeshTools {

	[Serializable]
	public class PolygonArea
	{
		//local vertice (mesh vertices) without duplicates
		public Vector3[] PolygonVertices;

		public Vector3[] PolygonDirections;

		//local vertice (mesh vertices)
		public Vector3[] RawPolygonVertices;

		public Vector3[] RawVerticesDirections;

		//local centroid
		public Vector3 Center;

		public Vector3 Normal;

		public float Scale = 1.0f;

		public PolygonArea(int verticesNum, int rawVerticesNum)
		{
			PolygonVertices = new Vector3[verticesNum];
			PolygonDirections = new Vector3[verticesNum];
			RawPolygonVertices = new Vector3[rawVerticesNum];
			RawVerticesDirections = new Vector3[rawVerticesNum];
		}

		public void SetScale(float newScale)
		{
			Scale = newScale;

		}

		public Vector3 ComputeVertex(int idx)
		{
			return PolygonVertices[idx] - (PolygonDirections[idx] * (1 - Scale));
		}

		public Vector3 ComputeDisplayedVertex(int idx, float spacing)
		{
			return RawPolygonVertices[idx] - (RawVerticesDirections[idx] * (1 - Scale)) + (Normal * spacing);
		}

		public Vector3 ComputeDisplayedBorder(int idx, float spacing)
		{
			return PolygonVertices[idx] - (PolygonDirections[idx] * (1 - Scale)) + (Normal * spacing);
		}
	}

}