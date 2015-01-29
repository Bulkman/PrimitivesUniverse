using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MeshTools {

	[CustomEditor(typeof(GreeblesGenerator))]
	public class GreeblesGeneratorEditor : Editor {

		#region Private fields
		private GreeblesGenerator componentRef;
		private bool drawVoronoi;
		private bool checkIntersection;
		private bool drawDelaunay;
		private bool drawPoints;
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

			drawVoronoi = EditorGUILayout.Toggle("Draw Voronoi", drawVoronoi);
			checkIntersection = EditorGUILayout.Toggle("Check Intersection", checkIntersection);
			drawDelaunay = EditorGUILayout.Toggle("Draw Delaunay", drawDelaunay);
			drawPoints = EditorGUILayout.Toggle("Draw Points", drawPoints);

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


				if(drawVoronoi){
					GL.Color( Color.red );
					foreach(var region in componentRef.VoronoiRegions)
					{
						foreach(var edge in region.RegionMesh.Edges)
						{
							Vector2 circumcenterV1 = edge.Source.Circumcenter;
							if(!region.ConvexPoly.ContainsPoint(circumcenterV1) && checkIntersection){
								float sign = 0;
								Vector2 tmpVec = Vector2.zero;
								region.ConvexPoly.ClosestSurfacePoint(circumcenterV1, out tmpVec, out sign);
								if(sign >= 0)
									circumcenterV1 = tmpVec;
							}
							Vector3 v1 = circumcenterV1;
							v1.z = region.Z;
							v1 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v1;
							v1 += region.Normal * 0.001f;

							Vector2 circumcenterV2 = edge.Target.Circumcenter;
							if(!region.ConvexPoly.ContainsPoint(circumcenterV2) && checkIntersection){
								float sign = 0;
								Vector2 tmpVec = Vector2.zero;
								region.ConvexPoly.ClosestSurfacePoint(circumcenterV2, out tmpVec, out sign);
								if(sign >= 0)
									circumcenterV2 = tmpVec;
							}
							Vector3 v2 = circumcenterV2;
							v2.z = region.Z;
							v2 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v2;
							v2 += region.Normal * 0.001f;

							GL.Vertex(v1);
							GL.Vertex(v2);
						}
					}
				}

				if(drawDelaunay){
					GL.Color( Color.blue );
					foreach(var region in componentRef.VoronoiRegions)
					{
						foreach(var cell in region.RegionMesh.Vertices)
						{
							Vector3 v1 = cell.Vertices[0].ToVector3();
							v1.z = region.Z;
							v1 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v1;
							v1 += region.Normal * 0.001f;
							
							Vector3 v2 = cell.Vertices[1].ToVector3();
							v2.z = region.Z;
							v2 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v2;
							v2 += region.Normal * 0.001f;

							Vector3 v3 = cell.Vertices[2].ToVector3();
							v3.z = region.Z;
							v3 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v3;
							v3 += region.Normal * 0.001f;

							GL.Vertex(v1);
							GL.Vertex(v2);
							
							GL.Vertex(v1);
							GL.Vertex(v3);
							
							GL.Vertex(v3);
							GL.Vertex(v2);
						}
					}
				}

				GL.End();
				GL.PopMatrix();

				if(drawPoints){
					Handles.color = Color.yellow;
					foreach(var point in componentRef.VoronoiPoints)
					{
						Handles.DotCap(0, point, Quaternion.identity, 0.01f);
					}
				}
			}
		}
		#endregion
	}
}
