using UnityEngine;
using System.Collections;
using System;

namespace MeshTools {

	[Serializable]
	public class MeshToolsEditorProperties : ScriptableObject
	{
		public enum NormalsDrawMode {
			None,
			ShowSelectionVerticesNormals,
			ShowSelectionFaceNormal,
			ShowAllVerticesNormals,
			ShowAllFaceNormals
		}

		public enum VerticesDrawMode {
			None,
			ShowIndices,
			ShowPosition,
		}

		public bool EnableEditing = false;

		public VerticesDrawMode VerticesDisplayMode = VerticesDrawMode.None;

		public NormalsDrawMode NormalsDisplayMode = NormalsDrawMode.None;
		
		public float NormalsScale = 0.1f;
		
		public Color SelectionColor = Color.red;
		
		public float SelectionStrength = 4f;
		
		public Color LabelsColor = Color.cyan;
		
		public Color NormalsColor = Color.blue;

		public MeshToolsEditorProperties()
		{
		}
	}

}

