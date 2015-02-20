using UnityEngine;
using System.Collections;
using UnityEditor;
using Delaunay.Geo;
using System.Collections.Generic;

namespace MeshTools {

	[CustomEditor(typeof(GreeblesGenerator))]
	public class GreeblesGeneratorEditor : Editor {

		#region Private fields
		private GreeblesGenerator componentRef;
		private bool drawVoronoi;
		private bool drawStructuresBase;
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
			drawStructuresBase = EditorGUILayout.Toggle("Draw Structures base", drawStructuresBase);
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

					foreach(var region in componentRef.VoronoiRegions){
						foreach(Vector2 siteCoord in region.RegionMesh.SiteCoords()){
							List<Vector2> r = region.RegionMesh.Region(siteCoord);
							for(int i = 0; i < r.Count - 1; i++){
								Vector3 v1 = (Vector3)r[i];
								v1.z = region.Z;
								v1 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v1;
								v1 += region.Normal * 0.001f;
								
								Vector3 v2 = (Vector3)r[i + 1];
								v2.z = region.Z;
								v2 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * v2;
								v2 += region.Normal * 0.001f;
								
								GL.Vertex(v1);
								GL.Vertex(v2);
							}

							Vector3 lastv1 = (Vector3)r[r.Count - 1];
							lastv1.z = region.Z;
							lastv1 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * lastv1;
							lastv1 += region.Normal * 0.001f;
							
							Vector3 lastv2 = (Vector3)r[0];
							lastv2.z = region.Z;
							lastv2 = Quaternion.FromToRotation(Vector3.forward, region.Normal) * lastv2;
							lastv2 += region.Normal * 0.001f;
							
							GL.Vertex(lastv1);
							GL.Vertex(lastv2);
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

				if(drawStructuresBase){
					GL.Color( Color.blue );

					foreach(var sb in componentRef.DisplayedStructuresBase){
						for(int i = 0; i < sb.Vertices.Count - 1; i++){
							GL.Vertex(sb.Vertices[i]);
							GL.Vertex(sb.Vertices[i + 1]);
						}
						
						GL.Vertex(sb.Vertices[sb.Vertices.Count - 1]);
						GL.Vertex(sb.Vertices[0]);
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
