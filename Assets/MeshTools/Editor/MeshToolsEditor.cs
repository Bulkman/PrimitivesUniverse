using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace MeshTools {

	[CustomEditor(typeof(MeshTools))]
	public class MeshToolsEditor : Editor
	{
		#region Private fields
		private MeshTools componentRef;
		private Transform refTransform;
		private Mesh mesh;
		private Vector3[] meshVertices;
		private RaycastHit hit;

		private GUIStyle labelsGUIStyle;
		private GUIStyle sectionCaptionGUIStyle;
		private GUIStyle polygonEntryGUIStyle;
		private GUIStyle buttonsGUIStyle;

		//new properties
		private MeshToolsEditorProperties newProperties;

		private List<FaceSelection> selectedFaces;
		private bool selectionModified = false;
		private bool showAreas = true;
		private bool showHandles = false;

		private string globalPropertiesPath = "Assets/MeshTools/MeshToolsEditorProperties.asset";

		private Material handleWireMaterial;
		#endregion

		#region Messages
		private void OnEnable()
		{
			handleWireMaterial = (Material)EditorGUIUtility.LoadRequired ("SceneView/HandleLines.mat");

			if(newProperties == null){
				//Debug.Log("ScriptableObject.CreateInstance");
				//try load properties asset
				newProperties = (MeshToolsEditorProperties)AssetDatabase.LoadAssetAtPath(globalPropertiesPath, typeof(MeshToolsEditorProperties));
				
				//if load fails then create asset
				if(newProperties == null){
					//Debug.Log("MeshDebuger: load failed");
					newProperties = ScriptableObject.CreateInstance("MeshToolsEditorProperties") as MeshToolsEditorProperties;
					string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (globalPropertiesPath);
					AssetDatabase.CreateAsset (newProperties, assetPathAndName);
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh();
				}
			}
			componentRef = target as MeshTools;
			refTransform = componentRef.transform;

			labelsGUIStyle = new GUIStyle();
			labelsGUIStyle.fontSize = 16;
			labelsGUIStyle.fontStyle = FontStyle.Bold;
			labelsGUIStyle.normal.textColor = Color.cyan;

			sectionCaptionGUIStyle = new GUIStyle();
			sectionCaptionGUIStyle.fontSize = 11;
			sectionCaptionGUIStyle.fontStyle = FontStyle.Bold;
			sectionCaptionGUIStyle.alignment = TextAnchor.MiddleCenter;
			sectionCaptionGUIStyle.normal.textColor = Color.grey;

			polygonEntryGUIStyle = new GUIStyle(EditorStyles.numberField);
			polygonEntryGUIStyle.padding = new RectOffset(16, 6, 6, 6);

			buttonsGUIStyle = new GUIStyle(EditorStyles.miniButton);

			selectedFaces = new List<FaceSelection>(componentRef.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3);
		}

		public override void OnInspectorGUI () 
		{
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("-------------------- Global properties --------------------", sectionCaptionGUIStyle);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Path: " + globalPropertiesPath);
			newProperties.EnableEditing = EditorGUILayout.Toggle("Enable editing", newProperties.EnableEditing);
			newProperties.SelectionStrength = EditorGUILayout.FloatField("Selection strength", newProperties.SelectionStrength);
			newProperties.SelectionColor = EditorGUILayout.ColorField("Selection color", newProperties.SelectionColor);
			newProperties.VerticesDisplayMode = (MeshToolsEditorProperties.VerticesDrawMode)EditorGUILayout.EnumPopup("Vertices display mode", newProperties.VerticesDisplayMode);
			newProperties.NormalsDisplayMode = (MeshToolsEditorProperties.NormalsDrawMode)EditorGUILayout.EnumPopup("Normals display mode", newProperties.NormalsDisplayMode);
			newProperties.NormalsScale = EditorGUILayout.FloatField("Normals scale", newProperties.NormalsScale);
			newProperties.NormalsColor = EditorGUILayout.ColorField("Normals color", newProperties.NormalsColor);
			newProperties.LabelsColor = EditorGUILayout.ColorField("Labels color", newProperties.LabelsColor);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("-------------------- Polygon area tool --------------------", sectionCaptionGUIStyle);
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplayMaterial"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DisplaySpacing"), true);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Selected faces count: " + selectedFaces.Count.ToString(), EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150));

			EditorGUILayout.BeginHorizontal();
			if(selectedFaces.Count > 0){
				if(GUILayout.Button("Add", buttonsGUIStyle, GUILayout.ExpandWidth(false))){

					//removing duplicates and find polygon center, normal
					Vector3 polygonCenter = Vector3.zero;
					Vector3 polygonNormal = Vector3.zero;
					float polygonArea = 0f;

					List<Vector3> tmpVerticesList = new List<Vector3>(selectedFaces.Count * 3);

					foreach (FaceSelection face in selectedFaces){
						if (!tmpVerticesList.Contains(face.LocalFaceVertices[0]))
							tmpVerticesList.Add(face.LocalFaceVertices[0]);
						if (!tmpVerticesList.Contains(face.LocalFaceVertices[1]))
							tmpVerticesList.Add(face.LocalFaceVertices[1]);
						if (!tmpVerticesList.Contains(face.LocalFaceVertices[2]))
							tmpVerticesList.Add(face.LocalFaceVertices[2]);

						float triangleArea = Utils.TriangleArea(face.LocalFaceVertices[0], face.LocalFaceVertices[1], face.LocalFaceVertices[2]);
						polygonArea += triangleArea;
						polygonCenter += face.LocalFaceCentroid * triangleArea;
						polygonNormal += face.FaceNormal;
					}
					
					polygonNormal /= selectedFaces.Count;
					polygonCenter /= polygonArea;

					PolygonArea polyArea = new PolygonArea(tmpVerticesList.Count, selectedFaces.Count * 3);
					polyArea.Center = polygonCenter;
					polyArea.Normal = polygonNormal;

					for(int i = 0; i < selectedFaces.Count; i++){
						polyArea.RawPolygonVertices[i * 3] = selectedFaces[i].LocalFaceVertices[0];
						polyArea.RawPolygonVertices[i * 3 + 1] = selectedFaces[i].LocalFaceVertices[1];
						polyArea.RawPolygonVertices[i * 3 + 2] = selectedFaces[i].LocalFaceVertices[2];

						polyArea.RawVerticesDirections[i * 3] = selectedFaces[i].LocalFaceVertices[0] - polygonCenter;
						polyArea.RawVerticesDirections[i * 3 + 1] = selectedFaces[i].LocalFaceVertices[1] - polygonCenter;
						polyArea.RawVerticesDirections[i * 3 + 2] = selectedFaces[i].LocalFaceVertices[2] - polygonCenter;
					}

					//sort clockwise
					Vector3 tmpVert = tmpVerticesList[0];
					tmpVerticesList.Sort(delegate(Vector3 v1, Vector3 v2) 
					{
						float angle1 = Math3D.SignedVectorAngle(v1 - polygonCenter,  tmpVert - polygonCenter, polygonNormal);
						float angle2 = Math3D.SignedVectorAngle(v2 - polygonCenter,  tmpVert - polygonCenter, polygonNormal);
						//Debug.Log("a1: " + angle1 + " - a2: " + angle2);
						if (angle1 < angle2) 
						{ 
							return -1; 
						} 
						else if (angle1 > angle2) 
						{ 
							return 1; 
						} 
						return 0; 
					});
					
					tmpVerticesList.CopyTo(polyArea.PolygonVertices);

					for(int v = 0; v < tmpVerticesList.Count; v++){
						polyArea.PolygonDirections[v] = tmpVerticesList[v] - polygonCenter;
					}

					componentRef.PolygonAreas.Add(polyArea);
				}
			}

			if(componentRef.PolygonAreas.Count > 0){
				if(GUILayout.Button("Clear", buttonsGUIStyle, GUILayout.ExpandWidth(false))){
					componentRef.PolygonAreas.Clear();
				}
			}

			if(componentRef.PolygonAreas.Count > 0){
				showAreas = GUILayout.Toggle(showAreas, "Show Polygons", buttonsGUIStyle, GUILayout.ExpandWidth(false));
			}

			if(componentRef.PolygonAreas.Count > 0){
				showHandles = GUILayout.Toggle(showHandles, "Show Handles", buttonsGUIStyle, GUILayout.ExpandWidth(false));
			}

			EditorGUILayout.EndHorizontal();

			SerializedProperty list = serializedObject.FindProperty("PolygonAreas");

			EditorGUILayout.Space();
			for (int i = 0; i < componentRef.PolygonAreas.Count; i++) {
				EditorGUILayout.BeginHorizontal(polygonEntryGUIStyle);
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("Polygon: " + i), true, GUILayout.ExpandWidth(true));
				if(GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.ExpandWidth(false))){
					componentRef.PolygonAreas.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
			}

			//EditorGUILayout.PropertyField(serializedObject.FindProperty("PolygonAreas"), true);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("-------------------------------------------------------------", sectionCaptionGUIStyle);
			EditorGUILayout.Space();

			if (GUI.changed){
				EditorUtility.SetDirty (componentRef);
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void OnSceneGUI ()
		{
			try{
				
				inputProccesing();
				
				selectionProccesing();
				
				areasProccesing();
				
				normalsDisplayModeProccesing();
				
				if(selectionModified){
					selectionModified = false;
					Repaint();
				}
				
			} catch (Exception ex){
				Debug.LogError(ex.StackTrace);
			}
			
			if (GUI.changed){
				EditorUtility.SetDirty (componentRef);
				serializedObject.ApplyModifiedProperties();
			}
		}
		#endregion
		
		#region Private methods
		private void inputProccesing()
		{
			//Debug.Log(GUIUtility.hotControl);
			if ((Event.current.type == EventType.MouseDown) && (Event.current.button == 1) && !Event.current.control){
				
				selectedFaces.Clear();
				selectionModified = true;
				
				if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit)){
					
					MeshCollider meshCollider = hit.collider as MeshCollider;
					
					if (meshCollider != null || meshCollider.sharedMesh != null){
						
						//check if its same object
						if(meshCollider.gameObject == componentRef.gameObject){
							
							mesh = meshCollider.sharedMesh;
							meshVertices = mesh.vertices;
							int[] triangles = mesh.triangles;
							
							FaceSelection faceSelection = new FaceSelection();
							faceSelection.FaceVerticesIndexes[0] = triangles[hit.triangleIndex * 3 + 0];
							faceSelection.FaceVerticesIndexes[1] = triangles[hit.triangleIndex * 3 + 1];
							faceSelection.FaceVerticesIndexes[2] = triangles[hit.triangleIndex * 3 + 2];
							
							faceSelection.TriangleIndex = hit.triangleIndex;
							selectedFaces.Add(faceSelection);
							
						}
					}
				}
				
			} else if ((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)){
				
				/*if (!Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit)){
						selectedFaces.Clear();
						selectionModified = true;
					}*/
				
			} else if((Event.current.type == EventType.MouseDown) && (Event.current.button == 1) && Event.current.control){
				
				if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit)){
					
					MeshCollider meshCollider = hit.collider as MeshCollider;
					
					if (meshCollider != null || meshCollider.sharedMesh != null){
						
						//check if its same object
						if(meshCollider.gameObject == componentRef.gameObject){
							
							//deselect if we hit this face again
							bool deselect = false;
							foreach(FaceSelection facesSelection in selectedFaces){
								if(facesSelection.TriangleIndex == hit.triangleIndex){
									selectedFaces.Remove(facesSelection);
									selectionModified = true;
									deselect = true;
									break;
								}
							}
							
							if(!deselect){
								
								mesh = meshCollider.sharedMesh;
								meshVertices = mesh.vertices;
								int[] triangles = mesh.triangles;
								
								FaceSelection faceSelection = new FaceSelection();
								faceSelection.FaceVerticesIndexes[0] = triangles[hit.triangleIndex * 3 + 0];
								faceSelection.FaceVerticesIndexes[1] = triangles[hit.triangleIndex * 3 + 1];
								faceSelection.FaceVerticesIndexes[2] = triangles[hit.triangleIndex * 3 + 2];
								
								faceSelection.TriangleIndex = hit.triangleIndex;

								recalculateSelectionFace(faceSelection);

								selectedFaces.Add(faceSelection);
								selectionModified = true;
							}
						}
					}
				}
			}
		}

		private void selectionProccesing ()
		{
			if(selectedFaces != null){
				foreach(FaceSelection facesSelection in selectedFaces){
					if(refTransform.hasChanged){
						recalculateSelectionFace(facesSelection);
					}
					
					Handles.color = newProperties.SelectionColor;
					Handles.DrawAAPolyLine(newProperties.SelectionStrength, facesSelection.FaceVertices);
					
					labelsGUIStyle.normal.textColor = newProperties.LabelsColor;
					
					if(newProperties.VerticesDisplayMode == MeshToolsEditorProperties.VerticesDrawMode.ShowIndices){
						Handles.Label(facesSelection.FaceVertices[0], facesSelection.FaceVerticesIndexes[0].ToString(), labelsGUIStyle);
						Handles.Label(facesSelection.FaceVertices[1], facesSelection.FaceVerticesIndexes[1].ToString(), labelsGUIStyle);
						Handles.Label(facesSelection.FaceVertices[2], facesSelection.FaceVerticesIndexes[2].ToString(), labelsGUIStyle);
					} else if(newProperties.VerticesDisplayMode == MeshToolsEditorProperties.VerticesDrawMode.ShowPosition){
						Handles.Label(facesSelection.FaceVertices[0], Utils.Vector3ToString(facesSelection.FaceVertices[0]), labelsGUIStyle);
						Handles.Label(facesSelection.FaceVertices[1], Utils.Vector3ToString(facesSelection.FaceVertices[1]), labelsGUIStyle);
						Handles.Label(facesSelection.FaceVertices[2], Utils.Vector3ToString(facesSelection.FaceVertices[2]), labelsGUIStyle);
					}
					
					Handles.Label(facesSelection.FaceCentroid, facesSelection.TriangleIndex.ToString(), labelsGUIStyle);
					
					if(newProperties.NormalsDisplayMode == MeshToolsEditorProperties.NormalsDrawMode.ShowSelectionVerticesNormals){
						Handles.color = newProperties.NormalsColor;
						Handles.DrawLine(facesSelection.FaceVertices[0], facesSelection.FaceVertices[0] + mesh.normals[facesSelection.FaceVerticesIndexes[0]] * newProperties.NormalsScale);
						Handles.DrawLine(facesSelection.FaceVertices[1], facesSelection.FaceVertices[1] + mesh.normals[facesSelection.FaceVerticesIndexes[1]] * newProperties.NormalsScale);
						Handles.DrawLine(facesSelection.FaceVertices[2], facesSelection.FaceVertices[2] + mesh.normals[facesSelection.FaceVerticesIndexes[2]] * newProperties.NormalsScale);
					} else if(newProperties.NormalsDisplayMode == MeshToolsEditorProperties.NormalsDrawMode.ShowSelectionFaceNormal){
						Handles.color = newProperties.NormalsColor;
						
						Handles.DrawLine(facesSelection.FaceCentroid, facesSelection.FaceCentroid + facesSelection.FaceNormal * newProperties.NormalsScale);
						Handles.DrawLine(facesSelection.FaceCentroid, facesSelection.FaceCentroid + Vector3.Cross(facesSelection.FaceNormal, facesSelection.FaceVertices[0] - facesSelection.FaceCentroid) * newProperties.NormalsScale);
					}
					
					if(newProperties.EnableEditing){
						//recalculate again
						recalculateSelectionFace(facesSelection);

						meshVertices[facesSelection.FaceVerticesIndexes[0]] = refTransform.InverseTransformPoint(Handles.DoPositionHandle (facesSelection.FaceVertices[0], Quaternion.identity));
						meshVertices[facesSelection.FaceVerticesIndexes[1]] = refTransform.InverseTransformPoint(Handles.DoPositionHandle (facesSelection.FaceVertices[1], Quaternion.identity));
						meshVertices[facesSelection.FaceVerticesIndexes[2]] = refTransform.InverseTransformPoint(Handles.DoPositionHandle (facesSelection.FaceVertices[2], Quaternion.identity));
					}
					
					mesh.vertices = meshVertices;
				}
			}
		}

		private void areasProccesing()
		{
			//show areas
			if(componentRef.PolygonAreas != null && showAreas){
				
				foreach(PolygonArea polyArea in componentRef.PolygonAreas){
					Quaternion rotation = Quaternion.LookRotation(polyArea.Normal);

					//Handles.DrawSolidRectangleWithOutline(polyArea.PolygonVertices, Color.green, Color.black);
					if (Event.current.type == EventType.Repaint)
					{
						componentRef.DisplayMaterial.SetPass (0);
						GL.PushMatrix ();
						GL.MultMatrix (Handles.matrix);
						Color faceColor = new Color(0f, 1f, 0f, 0.5f);
						if (faceColor.a > 0f)
						{
							GL.Begin (GL.TRIANGLES);
							for (int i = 0; i < polyArea.RawPolygonVertices.Length - 1; i+=3)
							{
								GL.Color (faceColor);
								GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedVertex(i, componentRef.DisplaySpacing)));
								GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedVertex(i + 1, componentRef.DisplaySpacing)));
								GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedVertex(i + 2, componentRef.DisplaySpacing)));
							}
							GL.End ();
						}
						Color outlineColor = new Color(1f, 1f, 1f, 1.0f);
						if (outlineColor.a > 0f)
						{
							GL.Begin (GL.LINES);
							GL.Color (outlineColor);
							for (int j = 0; j < polyArea.PolygonVertices.Length - 1; j++)
							{
								GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedBorder(j, componentRef.DisplaySpacing)));
								GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedBorder(j + 1, componentRef.DisplaySpacing)));
							}
							GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedBorder(polyArea.PolygonVertices.Length - 1, componentRef.DisplaySpacing)));
							GL.Vertex (refTransform.TransformPoint(polyArea.ComputeDisplayedBorder(0, componentRef.DisplaySpacing)));
							GL.End ();
						}
						GL.PopMatrix ();


//						GL.PushMatrix ();
//						GL.MultMatrix (Handles.matrix);
//						faceColor = new Color(1f, 1f, 0f, 0.5f);
//						if (faceColor.a > 0f)
//						{
//							GL.Begin (GL.TRIANGLES);
//							for (int i = 0; i < polyArea.RawPolygonVertices.Length - 1; i+=3)
//							{
//								GL.Color (faceColor);
//								GL.Vertex (Quaternion.FromToRotation(polyArea.Normal, Vector3.forward) * refTransform.TransformPoint(polyArea.RawPolygonVertices[i]));
//								GL.Vertex (Quaternion.FromToRotation(polyArea.Normal, Vector3.forward) * refTransform.TransformPoint(polyArea.RawPolygonVertices[i + 1]));
//								GL.Vertex (Quaternion.FromToRotation(polyArea.Normal, Vector3.forward) * refTransform.TransformPoint(polyArea.RawPolygonVertices[i + 2]));
//							}
//							GL.End ();
//						}
//						GL.PopMatrix ();
					}	

					if(showHandles){
						Handles.color = Color.blue;
						Handles.DotCap(8000, refTransform.TransformPoint(polyArea.Center), rotation, 0.01f);
						polyArea.SetScale(Handles.ScaleValueHandle(polyArea.Scale, refTransform.TransformPoint(polyArea.Center), rotation, polyArea.Scale, Handles.CircleCap, 1f));
					}
				}
			}
		}

		private void normalsDisplayModeProccesing()
		{
			//show all normals modes
			if(newProperties.NormalsDisplayMode == MeshToolsEditorProperties.NormalsDrawMode.ShowAllVerticesNormals){
				if(mesh == null)
					mesh = componentRef.GetComponent<MeshFilter>().mesh;
				
				for(int i = 0; i < mesh.vertices.Length; i++){
					Handles.color = newProperties.NormalsColor;
					Handles.DrawLine(componentRef.transform.TransformPoint(mesh.vertices[i]), componentRef.transform.TransformPoint(mesh.vertices[i] + mesh.normals[i] * newProperties.NormalsScale));
				}
			} else if(newProperties.NormalsDisplayMode == MeshToolsEditorProperties.NormalsDrawMode.ShowAllFaceNormals){
				if(mesh == null)
					mesh = componentRef.GetComponent<MeshFilter>().mesh;
				
				for(int i = 0; i < mesh.triangles.Length - 3; i+=3){
					Vector3 v0 = componentRef.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 0]]);
					Vector3 v1 = componentRef.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
					Vector3 v2 = componentRef.transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);
					
					Handles.color = newProperties.NormalsColor;
					
					Vector3 AB = v1 - v0;
					Vector3 AC = v2 - v1;
					
					//Calculate the normal
					Vector3 normal = Vector3.Normalize(Vector3.Cross(AB, AC));
					Vector3 triangleCenter = (v0 + v1 + v2) / 3f;
					Handles.DrawLine(triangleCenter, triangleCenter + normal * newProperties.NormalsScale);
				}
			}
		}

		private void recalculateSelectionFace(FaceSelection selection)
		{
			Vector3 AB = selection.FaceVertices[1] - selection.FaceVertices[0];
			Vector3 AC = selection.FaceVertices[2] - selection.FaceVertices[1];
			
			//Calculate the normal
			Vector3 normal = Vector3.Normalize(Vector3.Cross(AB, AC));
			
			selection.FaceNormal = normal;
			selection.VerticesNormals[0] = mesh.normals[selection.FaceVerticesIndexes[0]];
			selection.VerticesNormals[1] = mesh.normals[selection.FaceVerticesIndexes[1]];
			selection.VerticesNormals[2] = mesh.normals[selection.FaceVerticesIndexes[2]];
			
			selection.FaceVertices[0] = refTransform.TransformPoint(meshVertices[selection.FaceVerticesIndexes[0]]);
			selection.FaceVertices[1] = refTransform.TransformPoint(meshVertices[selection.FaceVerticesIndexes[1]]);
			selection.FaceVertices[2] = refTransform.TransformPoint(meshVertices[selection.FaceVerticesIndexes[2]]);
			selection.FaceVertices[3] = selection.FaceVertices[0];

			selection.LocalFaceVertices[0] = meshVertices[selection.FaceVerticesIndexes[0]];
			selection.LocalFaceVertices[1] = meshVertices[selection.FaceVerticesIndexes[1]];
			selection.LocalFaceVertices[2] = meshVertices[selection.FaceVerticesIndexes[2]];

			Vector3 triangleCenter = (selection.FaceVertices[0] + selection.FaceVertices[1] + selection.FaceVertices[2]) / 3f;
			Vector3 localTriangleCenter = (selection.LocalFaceVertices[0] + selection.LocalFaceVertices[1] + selection.LocalFaceVertices[2]) / 3f;
			selection.LocalFaceCentroid = localTriangleCenter;
			selection.FaceCentroid = triangleCenter;
		}

		#endregion
	}
	
}