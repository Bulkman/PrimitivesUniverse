using UnityEngine;
using System;

namespace MeshTools {

	[Serializable]
	public class FaceSelection
	{
		public int TriangleIndex;
		
		//last index == first index for polyline
		public Vector3[] FaceVertices;

		public Vector3[] LocalFaceVertices;

		public Vector3[] VerticesNormals;

		public Vector3 FaceNormal;

		public Vector3 FaceCentroid;

		public Vector3 LocalFaceCentroid;
		
		public int[] FaceVerticesIndexes;
		
		public FaceSelection()
		{
			FaceVertices = new Vector3[4];
			LocalFaceVertices = new Vector3[3];
			FaceVerticesIndexes = new int[3];
			VerticesNormals = new Vector3[3];
		}
	}

}
