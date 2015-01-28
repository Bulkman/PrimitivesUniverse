using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MeshTools {

	[CustomEditor(typeof(GreeblesGenerator))]
	public class GreeblesGeneratorEditor : Editor {

		#region Private fields
		private GreeblesGenerator componentRef;
		#endregion

		#region Messages
		private void OnEnable()
		{
			componentRef = target as GreeblesGenerator;
		}
		
		public override void OnInspectorGUI () 
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			if(GUILayout.Button("Generate Voronoi", GUILayout.ExpandWidth(true))){
				componentRef.GenerateGreebles();
			}

			if (GUI.changed){
				EditorUtility.SetDirty (componentRef);
				serializedObject.ApplyModifiedProperties();
			}
		}
		
		private void OnSceneGUI ()
		{
			if(componentRef.VoronoiRegions != null && componentRef.VoronoiRegions.Count > 0){
				componentRef.VoronoiDebugMaterial.SetPass (0);
				GL.PushMatrix ();
				GL.MultMatrix (Handles.matrix);
				GL.Begin( GL.LINES );
				GL.Color( Color.red );

				foreach(var region in componentRef.VoronoiRegions)
				{
					foreach(var edge in region.Edges)
					{
//					bool draw = true;
//					
//					if(!drawGhostVerts)
//					{
//						if(edge.Source.Circumcenter.x > size || edge.Source.Circumcenter.x < -size) draw = false;
//						if(edge.Target.Circumcenter.x > size || edge.Target.Circumcenter.x < -size) draw = false;
//						
//						if(edge.Source.Circumcenter.y > size || edge.Source.Circumcenter.y < -size) draw = false;
//						if(edge.Target.Circumcenter.y > size || edge.Target.Circumcenter.y < -size) draw = false;
//						
//						if(edge.Source.Circumcenter.z > size || edge.Source.Circumcenter.z < -size) draw = false;
//						if(edge.Target.Circumcenter.z > size || edge.Target.Circumcenter.z < -size) draw = false;
//					}
//					
//					if(!draw) continue;
						
						GL.Vertex3( -edge.Source.Circumcenter.x, -edge.Source.Circumcenter.y, -edge.Source.Circumcenter.z);
						GL.Vertex3( -edge.Target.Circumcenter.x, -edge.Target.Circumcenter.y, -edge.Target.Circumcenter.z);
					}
				}

				GL.End();
				GL.PopMatrix();
			}
		}
		#endregion
	}
}
