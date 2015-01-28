﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MIConvexHull;

namespace MeshTools {

	[RequireComponent(typeof(MeshTools))]
	[ExecuteInEditMode]
	public class GreeblesGenerator : MonoBehaviour {

		public int PointsCount = 8;

		public Material VoronoiDebugMaterial;

		private MeshTools meshTools;

		public List<VoronoiMesh<Vertex3, Cell3, VoronoiEdge<Vertex3, Cell3>>> VoronoiRegions { get; private set; }

		// Use this for initialization
		void Start () {
			meshTools = GetComponent<MeshTools>();

//			GameObject greebles = new GameObject("Greebles");
//			greebles.transform.parent = transform;
//			greebles.transform.localPosition = Vector3.zero;
//			greebles.transform.localScale = Vector3.one;
//
//			MeshFilter meshFilter = (MeshFilter)greebles.AddComponent(typeof(MeshFilter));
//			meshFilter.mesh = generateGreebles();
//
//			MeshRenderer renderer = greebles.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
//			renderer.material = GreeblesMaerial;
//			renderer.castShadows = true;
//			renderer.receiveShadows = true;
		}

		public Mesh GenerateGreebles()
		{
			//we need generate set of random points inside polygon
			if(VoronoiRegions != null)
				VoronoiRegions.Clear();

			VoronoiRegions = new List<VoronoiMesh<Vertex3, Cell3, VoronoiEdge<Vertex3, Cell3>>>(meshTools.PolygonAreas.Count);

			foreach(PolygonArea polyArea in meshTools.PolygonAreas){
				Vertex3[] vertices = new Vertex3[PointsCount + polyArea.PolygonVertices.Length];

				//transform to 2D space
				Vector2[] pointsSet2D = new Vector2[polyArea.PolygonVertices.Length + 1];
				float z = 0f;
				for (int i = 0; i < polyArea.PolygonVertices.Length; i++)
				{
					Vector3 tmpVec = transform.TransformPoint(polyArea.ComputeVertex(i));
					vertices[i] = new Vertex3(tmpVec);
					pointsSet2D[i] = Quaternion.FromToRotation(polyArea.Normal, Vector3.forward) * tmpVec;
					z = tmpVec.z;
				}
				pointsSet2D[pointsSet2D.Length - 1] = pointsSet2D[0];

				//make 2d convex polygon
				ConvexPoly2 convexPoly = new ConvexPoly2(pointsSet2D);
				Bounds2 convexPolyBonds = convexPoly.CalcBounds();

				//generate new points inside
				for(int k = 0; k < PointsCount; k++){
					Vector3 additionalPoint = new Vector2(Random.Range(convexPolyBonds.Min.x, convexPolyBonds.Max.x), 
					                                      Random.Range(convexPolyBonds.Min.y, convexPolyBonds.Max.y));
					int maxAttempts = 1000;
					int currentAttempt = 0;
					while(!convexPoly.IsInside(additionalPoint)){
						additionalPoint.x = Random.Range(convexPolyBonds.Min.x, convexPolyBonds.Max.x);
						additionalPoint.y = Random.Range(convexPolyBonds.Min.y, convexPolyBonds.Max.y);
						//Debug.Log(additionalPoint);
						if(currentAttempt == maxAttempts - 1)
							return null;

						currentAttempt++;
					}

					additionalPoint.z = z;
					additionalPoint = Quaternion.FromToRotation(Vector3.forward, polyArea.Normal) * additionalPoint;
					vertices[polyArea.PolygonVertices.Length + k] = new Vertex3(additionalPoint);
				}
				
				VoronoiRegions.Add(VoronoiMesh.Create<Vertex3, Cell3>(vertices));
			}

//			Mesh mesh = new Mesh();


//			#region Vertices
//			Vector3 p0 = new Vector3( -length * .5f,	-width * .5f, height * .5f );
//			Vector3 p1 = new Vector3( length * .5f, 	-width * .5f, height * .5f );
//			Vector3 p2 = new Vector3( length * .5f, 	-width * .5f, -height * .5f );
//			Vector3 p3 = new Vector3( -length * .5f,	-width * .5f, -height * .5f );	
//			
//			Vector3 p4 = new Vector3( -length * .5f,	width * .5f,  height * .5f );
//			Vector3 p5 = new Vector3( length * .5f, 	width * .5f,  height * .5f );
//			Vector3 p6 = new Vector3( length * .5f, 	width * .5f,  -height * .5f );
//			Vector3 p7 = new Vector3( -length * .5f,	width * .5f,  -height * .5f );
//			
//			Vector3[] vertices = new Vector3[]
//			{
//				// Bottom
//				p0, p1, p2, p3,
//				
//				// Left
//				p7, p4, p0, p3,
//				
//				// Front
//				p4, p5, p1, p0,
//				
//				// Back
//				p6, p7, p3, p2,
//				
//				// Right
//				p5, p6, p2, p1,
//				
//				// Top
//				p7, p6, p5, p4
//			};
//			#endregion
//			
//			#region Normales
//			Vector3 up 		= invertNormals ? Vector3.down 		: Vector3.up;
//			Vector3 down 	= invertNormals ? Vector3.up 		: Vector3.down;
//			Vector3 front 	= invertNormals ? Vector3.back 		: Vector3.forward;
//			Vector3 back 	= invertNormals ? Vector3.forward 	: Vector3.back;
//			Vector3 left 	= invertNormals ? Vector3.right 	: Vector3.left;
//			Vector3 right 	= invertNormals ? Vector3.left 		: Vector3.right;
//			
//			Vector3[] normales = new Vector3[]
//			{
//				// Bottom
//				down, down, down, down,
//				
//				// Left
//				left, left, left, left,
//				
//				// Front
//				front, front, front, front,
//				
//				// Back
//				back, back, back, back,
//				
//				// Right
//				right, right, right, right,
//				
//				// Top
//				up, up, up, up
//			};
//			#endregion	
//			
//			#region UVs
//			Vector2 _00 = new Vector2( 0f, 0f );
//			Vector2 _10 = new Vector2( 1f, 0f );
//			Vector2 _01 = new Vector2( 0f, 1f );
//			Vector2 _11 = new Vector2( 1f, 1f );
//			
//			Vector2[] uvs = new Vector2[]
//			{
//				// Bottom
//				_11, _01, _00, _10,
//				
//				// Left
//				_11, _01, _00, _10,
//				
//				// Front
//				_11, _01, _00, _10,
//				
//				// Back
//				_11, _01, _00, _10,
//				
//				// Right
//				_11, _01, _00, _10,
//				
//				// Top
//				_11, _01, _00, _10,
//			};
//			#endregion
//			
//			#region Triangles
//			int[] triangles = new int[]
//			{
//				// Bottom
//				3, 1, 0,
//				3, 2, 1,			
//				
//				// Left
//				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
//				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
//				
//				// Front
//				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
//				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
//				
//				// Back
//				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
//				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
//				
//				// Right
//				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
//				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
//				
//				// Top
//				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
//				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
//				
//			};
//			#endregion
//			
//			mesh.vertices = vertices;
//			mesh.normals = normales;
//			mesh.uv = uvs;
//			mesh.triangles = triangles;
			
//			mesh.RecalculateBounds();
//			mesh.Optimize();
			return null;
		}
	}
}
