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
							Vector2 circumcenterV2 = edge.Target.Circumcenter;

							if(!region.ConvexPoly.ContainsPoint(circumcenterV1) && checkIntersection){
								Vector2 tmpVec;
								if(region.ConvexPoly.IntersectLine(circumcenterV1, circumcenterV2, out tmpVec)){
									circumcenterV1 = tmpVec;
								}
							}

							if(!region.ConvexPoly.ContainsPoint(circumcenterV2) && checkIntersection){
								Vector2 tmpVec;
								if(region.ConvexPoly.IntersectLine(circumcenterV1, circumcenterV2, out tmpVec)){
									circumcenterV2 = tmpVec;
								}
							}

							Vector3 v1 = circumcenterV1;
							v1.z = region.Z;
							v1 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v1;
							v1 += region.Normal * 0.001f;

							Vector3 v2 = circumcenterV2;
							v2.z = region.Z;
							v2 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v2;
							v2 += region.Normal * 0.001f;

							GL.Vertex(v1);
							GL.Vertex(v2);
						}

						GL.Color( Color.magenta );
						Vector3 bounds00 = region.ConvexPoly.CalcBounds().Point00;
						bounds00.z = region.Z;
						bounds00 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * bounds00;
						bounds00 += region.Normal * 0.001f;

						Vector3 bounds01 = region.ConvexPoly.CalcBounds().Point01;
						bounds01.z = region.Z;
						bounds01 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * bounds01;
						bounds01 += region.Normal * 0.001f;

						Vector3 bounds11 = region.ConvexPoly.CalcBounds().Point11;
						bounds11.z = region.Z;
						bounds11 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * bounds11;
						bounds11 += region.Normal * 0.001f;

						Vector3 bounds10 = region.ConvexPoly.CalcBounds().Point10;
						bounds10.z = region.Z;
						bounds10 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * bounds10;
						bounds10 += region.Normal * 0.001f;

						GL.Vertex(bounds00);
						GL.Vertex(bounds01);

						GL.Vertex(bounds01);
						GL.Vertex(bounds11);

						GL.Vertex(bounds11);
						GL.Vertex(bounds10);

						GL.Vertex(bounds10);
						GL.Vertex(bounds00);
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
						Handles.DotCap(0, point, Quaternion.identity, 0.003f);
					}
				}
			}
		}
		#endregion
	}
}
