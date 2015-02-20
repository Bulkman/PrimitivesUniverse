using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

namespace MeshTools {

	using Path = List<IntPoint>;
	using Paths = List<List<IntPoint>>;

	[RequireComponent(typeof(MeshTools))]
	[ExecuteInEditMode]
	public class GreeblesGenerator : MonoBehaviour {

		public class StructureBase
		{
			public List<Vector3> Vertices { get; private set; }

			public Vector3 Normal { get; private set; }

			public StructureBase(List<Vector3> vertices, Vector3 normal = new Vector3())
			{
				Vertices = vertices;
				Normal = normal;
			}
		}

		public class VoronoiRegion
		{
			public Delaunay.Voronoi RegionMesh { get; private set; }

			public float Z { get; private set; }

			public Vector3 Normal { get; private set; }

			public ConvexPoly2 ConvexPoly { get; private set; }

			public VoronoiRegion(Delaunay.Voronoi regionMesh, ConvexPoly2 convexPoly, float z, Vector3 normal)
			{
				ConvexPoly = convexPoly;
				RegionMesh = regionMesh;
				Z = z;
				Normal = normal;
			}
		}

		public int PointsCount = 32;

		public int ClipperScaleFactor = 1000;

		[Range(0.01f, 1f)]
		public float MinHeight;

		[Range(0.01f, 1f)]
		public float MaxHeight;

		[Range(0.01f, 1f)]
		public float Padding;

		public Material VoronoiDebugMaterial;

		public Material GreeblesMaerial;

		public List<VoronoiRegion> VoronoiRegions { get; private set; }
		public List<StructureBase> DisplayedStructuresBase { get; private set; }
		public List<Vector3> VoronoiPoints { get; private set; }

		private List<StructureBase> structuresBase;

		private MeshTools meshTools;
		
		private GameObject greebles;

		// Use this for initialization
		void Start () {
			meshTools = GetComponent<MeshTools>();
		}

		public Mesh GenerateGreebles()
		{
			//we need generate set of random points inside polygon
			if(VoronoiRegions != null)
				VoronoiRegions.Clear();

			if(VoronoiPoints != null)
				VoronoiPoints.Clear();

			DisplayedStructuresBase = new List<StructureBase>(meshTools.PolygonAreas.Count * PointsCount);
			structuresBase = new List<StructureBase>(meshTools.PolygonAreas.Count * PointsCount);

			VoronoiRegions = new List<VoronoiRegion>(meshTools.PolygonAreas.Count);
			VoronoiPoints = new List<Vector3>(meshTools.PolygonAreas.Count * 3); //minimum size
			int meshVCount = 0;

			foreach(PolygonArea polyArea in meshTools.PolygonAreas){
				Vector3 transformedNormal = transform.TransformDirection(polyArea.Normal);
				//transform to 2D space
				List<Vector2> pointsSet2D = new List<Vector2>(PointsCount + polyArea.PolygonVertices.Length);
				Path subj = new Path(polyArea.PolygonVertices.Length);
				float z = 0f;
				for (int i = 0; i < polyArea.PolygonVertices.Length; i++)
				{
					Vector3 tmpVec = transform.TransformPoint(polyArea.ComputeVertex(i));
					VoronoiPoints.Add(tmpVec);
					tmpVec = Quaternion.FromToRotation(transformedNormal, Vector3.forward) * tmpVec;
					//qqq.Add(tmpVec);
					pointsSet2D.Add(tmpVec);
					subj.Add(new IntPoint(tmpVec.x * ClipperScaleFactor, tmpVec.y * ClipperScaleFactor));
					z = tmpVec.z;
				}

				//make 2d convex polygon
				ConvexPoly2 convexPoly = new ConvexPoly2(pointsSet2D.ToArray());
				Bounds2 convexPolyBonds = convexPoly.CalcBounds();

				//generate new points inside
				for(int k = 0; k < PointsCount; k++){
					Vector3 additionalPoint = new Vector3(Random.Range(convexPolyBonds.Min.x, convexPolyBonds.Max.x), 
					                                      Random.Range(convexPolyBonds.Min.y, convexPolyBonds.Max.y));
					int maxAttempts = 1000;
					int currentAttempt = 0;
					while(!convexPoly.ContainsPoint(additionalPoint)){
						additionalPoint.x = Random.Range(convexPolyBonds.Min.x, convexPolyBonds.Max.x);
						additionalPoint.y = Random.Range(convexPolyBonds.Min.y, convexPolyBonds.Max.y);
						//Debug.Log(additionalPoint);
						if(currentAttempt == maxAttempts - 1)
							return null;

						currentAttempt++;
					}

					additionalPoint.z = z;
					VoronoiPoints.Add(Quaternion.FromToRotation(Vector3.forward, transformedNormal) * additionalPoint);
					//additionalPoint = Quaternion.FromToRotation(Vector3.forward, polyArea.Normal) * additionalPoint;
					pointsSet2D.Add(additionalPoint);
				}

				List<uint> colors = new List<uint> ();
				for (int i = 0; i < pointsSet2D.Count; i++) {
					colors.Add (0);
				}

				Rect rect = new Rect (convexPolyBonds.Min.x, convexPolyBonds.Min.y, convexPolyBonds.Size.x, convexPolyBonds.Size.y);
				Delaunay.Voronoi v = new Delaunay.Voronoi(pointsSet2D, colors, rect);

				VoronoiRegions.Add(new VoronoiRegion(v, convexPoly, z, transformedNormal));

				foreach(Vector2 siteCoord in v.SiteCoords()){

					List<Vector2> r = new List<Vector2>(v.Region(siteCoord));
					List<Vector3> tmpList1 = new List<Vector3>(r.Count);
					List<Vector3> tmpList2 = new List<Vector3>(r.Count);
					Vector3 siteCoord3D = (Vector3)siteCoord;
					siteCoord3D.z = z;
					siteCoord3D = Quaternion.FromToRotation(Vector3.forward, transformedNormal) * siteCoord3D;
					siteCoord3D += transformedNormal * 0.0001f;

					bool needClipping = false;
					for(int i = 0; i < r.Count; i++){
						if(!convexPoly.IsInside(r[i])){
							needClipping = true;
							break;
						}
					}

					if(needClipping){
						Path clip = new Path(r.Count);
						for(int i = 0; i < r.Count; i++){
							clip.Add(new IntPoint(r[i].x * ClipperScaleFactor, r[i].y * ClipperScaleFactor));
						}

						Paths solution = new Paths(1);
						
						Clipper clipper = new Clipper();
						clipper.AddPath(subj, PolyType.ptSubject, true);
						clipper.AddPath(clip, PolyType.ptClip, true);
						clipper.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

						r.Clear();

						for(int i = 0; i < solution[0].Count; i++){
							r.Add(new Vector2((float)solution[0][i].X / (float)ClipperScaleFactor, (float)solution[0][i].Y / (float)ClipperScaleFactor));
						}
					}

					for(int i = 0; i < r.Count; i++){
						Vector3 sv = (Vector3)r[i];
						sv.z = z;
						sv = Quaternion.FromToRotation(Vector3.forward, transformedNormal) * sv;
						sv = sv - ((sv - siteCoord3D) * Padding);

						Vector3 tmpPos = transform.position;
						Quaternion tmpRot = transform.rotation;
						Vector3 tmpScale = transform.localScale;
						Vector3 sInv = new Vector3(1f / tmpScale.x, 1f / tmpScale.y, 1f / tmpScale.z);
						
						tmpList2.Add(Vector3.Scale(sInv, sv - tmpPos));
						meshVCount++;
						sv += transformedNormal * 0.0001f;
						tmpList1.Add(sv);
					}

					if(tmpList1.Count > 2){
						DisplayedStructuresBase.Add(new StructureBase(tmpList1));
						structuresBase.Add(new StructureBase(tmpList2, transformedNormal));
					}
				}
			}
			
			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>(meshVCount * 2);
			List<Vector3> normales = new List<Vector3>(meshVCount * 2);
			List<Vector2> uvs = new List<Vector2>(meshVCount * 2);
			List<int> triangles = new List<int>(meshVCount * 4); // large enogh?

			int vci = 0;
			foreach(StructureBase sb in structuresBase){
				float height = Random.Range(MinHeight, MaxHeight);
				for(int i = 0; i < sb.Vertices.Count; i++){
					//celing
					vertices.Add(sb.Vertices[i]);
					normales.Add(Vector3.one);
					uvs.Add(Vector3.zero);
					vci++;

					vertices.Add(sb.Vertices[i] + sb.Normal.normalized * height);
					normales.Add(Vector3.one);
					uvs.Add(Vector3.zero);
					vci++;
				}

				//indices for sides
				for(int j = vci - sb.Vertices.Count * 2; j < vci - 3; j+=2){
					triangles.Add(j);
					triangles.Add(j + 2);
					triangles.Add(j + 1);
					
					triangles.Add(j + 2);
					triangles.Add(j + 3);
					triangles.Add(j + 1);
				}

				triangles.Add(vci - 1);
				triangles.Add(vci - 2);
				triangles.Add(vci - sb.Vertices.Count * 2);
				
				triangles.Add(vci - sb.Vertices.Count * 2);
				triangles.Add(vci - sb.Vertices.Count * 2 + 1);
				triangles.Add(vci - 1);

				//indices for celling
				for(int k = 0; k < sb.Vertices.Count - 2; k++){
					triangles.Add(vci - sb.Vertices.Count * 2 + 1);
					triangles.Add(vci - sb.Vertices.Count * 2 + 1 + (2 + k * 2));
					triangles.Add(vci - sb.Vertices.Count * 2 + 1 + (4 + k * 2));
				}
			}			
			
			mesh.vertices = vertices.ToArray();
			mesh.normals = normales.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.triangles = triangles.ToArray();

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			if(greebles != null){
				DestroyImmediate(greebles);
			}

			greebles = new GameObject("Greebles");
			greebles.transform.parent = transform;
			greebles.transform.localPosition = Vector3.zero;
			greebles.transform.localScale = Vector3.one;
			
			MeshRenderer renderer = greebles.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
			renderer.material = GreeblesMaerial;
			renderer.castShadows = true;
			renderer.receiveShadows = true;
			
			MeshFilter meshFilter = (MeshFilter)greebles.AddComponent(typeof(MeshFilter));
			meshFilter.mesh = mesh;

			return mesh;
		}
	}
}
