using System.Collections.Generic;
using UnityEngine;
using Poly2Tri;

static class Utils
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void MakeDoubleSidedMesh(Mesh mesh){
		var vertices = mesh.vertices;
		var uv = mesh.uv;
		var normals = mesh.normals;
		var szV = vertices.Length;
		var newVerts = new Vector3[szV*2];
		var newUv = new Vector2[szV*2];
		var newNorms = new Vector3[szV*2];
		for (var j=0; j< szV; j++){
			// duplicate vertices and uvs:
			newVerts[j] = newVerts[j+szV] = vertices[j];
			newUv[j] = newUv[j+szV] = uv[j];
			// copy the original normals...
			newNorms[j] = normals[j];
			// and revert the new ones
			newNorms[j+szV] = -normals[j];
		}
		var triangles = mesh.triangles;
		var szT = triangles.Length;
		var newTris = new int[szT*2]; // double the triangles
		for (var i=0; i< szT; i+=3){
			// copy the original triangle
			newTris[i] = triangles[i];
			newTris[i+1] = triangles[i+1];
			newTris[i+2] = triangles[i+2];
			// save the new reversed triangle
			var j = i+szT; 
			newTris[j] = triangles[i]+szV;
			newTris[j+2] = triangles[i+1]+szV;
			newTris[j+1] = triangles[i+2]+szV;
		}
		mesh.vertices = newVerts;
		mesh.uv = newUv;
		mesh.normals = newNorms;
		mesh.triangles = newTris; // assign triangles last!
	}
	
	//plane
	public static Mesh GeneratePlaneMesh(float length, float width, int resX, int resZ)
	{
		// You can change that line to provide another MeshFilter
		Mesh mesh = new Mesh();
		mesh.Clear();
		
		#region Vertices		
		Vector3[] vertices = new Vector3[ resX * resZ ];
		for(int z = 0; z < resZ; z++)
		{
			// [ -length / 2, length / 2 ]
			float zPos = ((float)z / (resZ - 1) - .5f) * length;
			for(int x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices[ x + z * resX ] = new Vector3( xPos, 0f, zPos );
			}
		}
		#endregion
		
		#region Normales
		Vector3[] normales = new Vector3[ vertices.Length ];
		for( int n = 0; n < normales.Length; n++ )
			normales[n] = Vector3.up;
		#endregion
		
		#region UVs		
		Vector2[] uvs = new Vector2[ vertices.Length ];
		for(int v = 0; v < resZ; v++)
		{
			for(int u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
			}
		}
		#endregion
		
		#region Triangles
		int nbFaces = (resX - 1) * (resZ - 1);
		int[] triangles = new int[ nbFaces * 6 ];
		int t = 0;
		for(int face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);
			
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
			
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		return mesh;
	}

	public static void PlaceDebugText(GameObject parent, TextMesh prefab, Vector3 pos, string text){
		TextMesh tm = GameObject.Instantiate( prefab, Vector3.zero, Quaternion.identity ) as TextMesh;
		tm.text = text;
		tm.transform.parent = parent.transform;
		tm.transform.localPosition = pos;
	}

	public static bool IsClockwise(Vector3[] vertices)
	{
		double sum = 0.0;
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 v1 = vertices[i];
			Vector3 v2 = vertices[(i + 1) % vertices.Length];
			sum += (v2.x - v1.x) * (v2.y + v1.y);
		}
		return sum > 0.0;
	}

	public static void MakeSomeNoise(MeshFilter meshFilter) {
		Vector3[] vertices = meshFilter.mesh.vertices;
		Vector3[] normals = meshFilter.mesh.normals;

		for (int i = 0; i < vertices.Length; i++) {    
			//Vector3 pos = vertices[i].normalized * 1f;
			float sample = Mathf.Floor(Noise.Value3D(vertices[i], 10.0f).value / 0.25f) * 0.25f;
			vertices[i] *= (1.0f + sample * 0.35f);
		}

		meshFilter.mesh.vertices = vertices;
		meshFilter.mesh.RecalculateBounds();
		meshFilter.mesh.RecalculateNormals();
		meshFilter.mesh.Optimize();
	}

	public static void Greeble(MeshFilter meshFilter) {
		Vector3[] vertices = meshFilter.mesh.vertices;
		Vector3[] normals = meshFilter.mesh.normals;
		
		for (int i = 0; i < vertices.Length; i++) {    
			//Vector3 pos = vertices[i].normalized * 1f;
			float sample = /*Mathf.Floor(*/Noise.Value3D(vertices[i], 1.0f).value/* / 1.0f) * 1.0f*/;
			vertices[i] *= (1.0f + sample * 0.25f);
		}
		
		meshFilter.mesh.vertices = vertices;
		meshFilter.mesh.RecalculateBounds();
		meshFilter.mesh.RecalculateNormals();
		meshFilter.mesh.Optimize();
	}

	public static void GenerateHoleOnTriangle(MeshFilter meshFilter, int tirangleNum, TextMesh debugText)
	{
		//the maximum rectangle area occurs when the midpoints of two of the sides of the 
		//triangle were joined to make a side of the rectangle and its area is thus 50% or 
		//half of the area of the triangle or 1/4 of the base times height
		Mesh mesh = meshFilter.mesh;

		int v1Index = mesh.triangles[tirangleNum * 3];
		int v2Index = mesh.triangles[tirangleNum * 3 + 1];
		int v3Index = mesh.triangles[tirangleNum * 3 + 2];

		List<int> verticesList = new List<int>(3);
		verticesList.Add(v1Index);
		verticesList.Add(v2Index);
		verticesList.Add(v3Index);

		//shuffle our vertices
//		int tmpFromVList = Random.Range(0, verticesList.Count);
		int p1Rnd = v1Index/*verticesList[tmpFromVList]*/;
		Vector3 p1 = mesh.vertices[p1Rnd];
//		verticesList.RemoveAt(tmpFromVList);
//		tmpFromVList = Random.Range(0, verticesList.Count);
		int p2Rnd = v2Index/*verticesList[tmpFromVList]*/;
		Vector3 p2 = mesh.vertices[p2Rnd];
//		verticesList.RemoveAt(tmpFromVList);
		Vector3 p3 = mesh.vertices[v3Index/*verticesList[0]*/];

		int tv1 = p1Rnd;
		int tv2 = p2Rnd;
		int tv3 = v3Index /*verticesList[0]*/;

		//find out middle points. 2-nd side pick randomly
		Vector3 middlePoint1 = (p1 + p2) / 2;
		//store 2-nd point
		int p3Rnd = Random.Range(0, 2);

//		Vector3 tmp1;
//		Vector3 tmp2;
//		if(p3Rnd == 0){
//			tmp1 = p1;
//			tmp2 = p2;
//			tv1 = p1Rnd;
//			tv2 = p2Rnd;
//
//			if(debugText != null){
//				PlaceDebugText(meshFilter.gameObject, debugText, p1, "tv1");
//				PlaceDebugText(meshFilter.gameObject, debugText, p2, "tv2");
//				PlaceDebugText(meshFilter.gameObject, debugText, p3, "tv3");
//			}
//		} else {
//			tmp1 = p2;
//			tmp2 = p1;
//			tv1 = p2Rnd;
//			tv2 = p1Rnd;
//
//			if(debugText != null){
//				PlaceDebugText(meshFilter.gameObject, debugText, p2, "tv1");
//				PlaceDebugText(meshFilter.gameObject, debugText, p1, "tv2");
//				PlaceDebugText(meshFilter.gameObject, debugText, p3, "tv3");
//			}
//		}

		Vector3 middlePoint2 = (p3 + p2/*tmp2*/) / 2;

		//tmp1 - p3 is our opposite side
		//project middle points on opposite triangle side
		Vector3 projectedPoint1 = Math3D.ProjectPointOnLineSegment(p1/*tmp1*/, p3, middlePoint1);
		Vector3 projectedPoint2 = Math3D.ProjectPointOnLineSegment(p1/*tmp1*/, p3, middlePoint2);

		//now randomly resize our rectangle so its point will not belong to triagle sides
		//Vector3 center = new Vector3((middlePoint1 + projectedPoint2) / 2); // is the diagonal center

		float tmpScale = Random.Range(0.05f, 0.45f);
		Vector3 finalRectangleV1 = middlePoint1 - (middlePoint1 - projectedPoint1) * tmpScale;
		Vector3 finalRectangleV2 = middlePoint2 - (middlePoint2 - projectedPoint2) * tmpScale;
		tmpScale = Random.Range(0.05f, 0.45f);
		Vector3 finalRectangleV3 = projectedPoint1 + (middlePoint1 - projectedPoint1) * tmpScale;
		Vector3 finalRectangleV4 = projectedPoint2 + (middlePoint2 - projectedPoint2) * tmpScale;
		tmpScale = Random.Range(0.05f, 0.45f);
		finalRectangleV1 = finalRectangleV1 + (finalRectangleV2 - finalRectangleV1) * tmpScale;
		finalRectangleV3 = finalRectangleV3 + (finalRectangleV4 - finalRectangleV3) * tmpScale;
		tmpScale = Random.Range(0.05f, 0.45f);
		//rewrite cuz shrinking should be twice more
		finalRectangleV2 = finalRectangleV2 - (finalRectangleV2 - finalRectangleV1) * tmpScale;
		finalRectangleV4 = finalRectangleV4 - (finalRectangleV4 - finalRectangleV3) * tmpScale;

		if(debugText != null){
			PlaceDebugText(meshFilter.gameObject, debugText, finalRectangleV1, "rv1");
			PlaceDebugText(meshFilter.gameObject, debugText, finalRectangleV2, "rv2");
			PlaceDebugText(meshFilter.gameObject, debugText, finalRectangleV3, "rv3");
			PlaceDebugText(meshFilter.gameObject, debugText, finalRectangleV4, "rv4");
		}
		
		//now TRIANGULATE!!
		List<Vector3> vertices = new List<Vector3>(mesh.vertices);
		List<Vector2> uv = new List<Vector2>(mesh.uv);
		List<Vector3> normals = new List<Vector3>(mesh.normals);
		List<int> triangles = new List<int>(mesh.triangles);

		int startPos = tirangleNum * 3;

		//remove old v's
		//vertices.RemoveAt(tirangleNum * 3);
		//vertices.RemoveAt(tirangleNum * 3 + 1);
		//vertices.RemoveAt(tirangleNum * 3 + 2);

		Vector3[] rectVertices = new Vector3[4];
		rectVertices[0] = finalRectangleV1;
		rectVertices[1] = finalRectangleV2;
		rectVertices[2] = finalRectangleV3;
		rectVertices[3] = finalRectangleV4;
		vertices.AddRange(rectVertices);

		//duplicating last triang vertex uv for now
		Vector2[] rectUvs = new Vector2[4];
		rectUvs[0] = uv[triangles[v3Index]];
		rectUvs[1] = uv[triangles[v3Index]];
		rectUvs[2] = uv[triangles[v3Index]];
		rectUvs[3] = uv[triangles[v3Index]];
		uv.AddRange(rectUvs);

		Vector3 AB = finalRectangleV2 - finalRectangleV1;
		Vector3 AC = finalRectangleV3 - finalRectangleV1;
		
		//Calculate the normal
		Vector3 normal = Vector3.Normalize(Vector3.Cross(AB, AC));
		Vector3[] rectNormals = new Vector3[4];
		rectNormals[0] = normal;
		rectNormals[1] = normal;
		rectNormals[2] = normal;
		rectNormals[3] = normal;
		normals.AddRange(rectNormals);

		//now fun part, triangles
		//remove target triangle
		triangles.RemoveRange(startPos, 3);

		int rv1 = vertices.Count - 4;
		int rv2 = vertices.Count - 3;
		int rv3 = vertices.Count - 2;
		int rv4 = vertices.Count - 1;

//		Vector3[] checkWindingOrderVertices = new Vector3[3]
//		{
//			finalRectangleV1,
//			tmp2,
//			finalRectangleV2
//		};
//
//		bool worder = IsClockwise(checkWindingOrderVertices);
//		if(worder){
//			Debug.Log("CW");
//		} else {
//			Debug.Log("CCW");
//		}

		int[] damnTriangles = new int[21]
		{
//			v1Index, v2Index, v3Index
			rv1, tv2, rv2,
			rv2, tv2, tv3,
			rv2, tv3, rv4,
			rv4, tv3, tv1,
			rv4, tv1, rv3,
			rv3, tv1, rv1,
			rv1, tv1, tv2

		};

		triangles.InsertRange(startPos, damnTriangles);

		mesh.vertices = vertices.ToArray();
		mesh.uv = uv.ToArray();
		mesh.normals = normals.ToArray();
		mesh.triangles = triangles.ToArray();

		//mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		//mesh.Optimize();
	}

	public static Mesh GenerateBoxMesh(bool invertNormals){
		// You can change that line to provide another MeshFilter
		Mesh mesh = new Mesh();
		mesh.Clear();
		
		float length = 1f;
		float width = 1f;
		float height = 1f;
		
		#region Vertices
		Vector3 p0 = new Vector3( -length * .5f,	-width * .5f, height * .5f );
		Vector3 p1 = new Vector3( length * .5f, 	-width * .5f, height * .5f );
		Vector3 p2 = new Vector3( length * .5f, 	-width * .5f, -height * .5f );
		Vector3 p3 = new Vector3( -length * .5f,	-width * .5f, -height * .5f );	
		
		Vector3 p4 = new Vector3( -length * .5f,	width * .5f,  height * .5f );
		Vector3 p5 = new Vector3( length * .5f, 	width * .5f,  height * .5f );
		Vector3 p6 = new Vector3( length * .5f, 	width * .5f,  -height * .5f );
		Vector3 p7 = new Vector3( -length * .5f,	width * .5f,  -height * .5f );
		
		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, p2, p3,
			
			// Left
			p7, p4, p0, p3,
			
			// Front
			p4, p5, p1, p0,
			
			// Back
			p6, p7, p3, p2,
			
			// Right
			p5, p6, p2, p1,
			
			// Top
			p7, p6, p5, p4
		};
		#endregion
		
		#region Normales
		Vector3 up 		= invertNormals ? Vector3.down 		: Vector3.up;
		Vector3 down 	= invertNormals ? Vector3.up 		: Vector3.down;
		Vector3 front 	= invertNormals ? Vector3.back 		: Vector3.forward;
		Vector3 back 	= invertNormals ? Vector3.forward 	: Vector3.back;
		Vector3 left 	= invertNormals ? Vector3.right 	: Vector3.left;
		Vector3 right 	= invertNormals ? Vector3.left 		: Vector3.right;
		
		Vector3[] normales = new Vector3[]
		{
			// Bottom
			down, down, down, down,
			
			// Left
			left, left, left, left,
			
			// Front
			front, front, front, front,
			
			// Back
			back, back, back, back,
			
			// Right
			right, right, right, right,
			
			// Top
			up, up, up, up
		};
		#endregion	
		
		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( 1f, 0f );
		Vector2 _01 = new Vector2( 0f, 1f );
		Vector2 _11 = new Vector2( 1f, 1f );
		
		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,
			
			// Left
			_11, _01, _00, _10,
			
			// Front
			_11, _01, _00, _10,
			
			// Back
			_11, _01, _00, _10,
			
			// Right
			_11, _01, _00, _10,
			
			// Top
			_11, _01, _00, _10,
		};
		#endregion
		
		#region Triangles
		int[] triangles = new int[]
		{
			// Bottom
			3, 1, 0,
			3, 2, 1,			
			
			// Left
			3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
			3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
			
			// Front
			3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
			3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
			
			// Back
			3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
			3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
			
			// Right
			3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
			3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
			
			// Top
			3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
			3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
			
		};
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		return mesh;
	}

	//uv messed up
	public static Mesh GenerateBoxWithHoleMesh(float botLeftX, float botLeftY, float topRightX, float topRightY){
		if(botLeftX > 0.5f)
			botLeftX = 0.5f;
		else if (botLeftX < 0.0f)
			botLeftX = 0.0f;
		
		if(botLeftY > 0.5f)
			botLeftY = 0.5f;
		else if (botLeftY < 0.0f)
			botLeftY = 0.0f;
		
		if(topRightX > 0.5f)
			topRightX = 0.5f;
		else if (topRightX < 0.0f)
			topRightX = 0.0f;
		
		if(topRightY > 0.5f)
			topRightY = 0.5f;
		else if (topRightY < 0.0f)
			topRightY = 0.0f;
		
		Mesh mesh = new Mesh();
		mesh.Clear();
		
		float length = 1f;
		float width = 1f;
		float height = 1f;
		
		#region Vertices
		Vector3 p0 = new Vector3( -length * .5f,	-width * .5f, height * .5f );
		Vector3 p1 = new Vector3( length * .5f, 	-width * .5f, height * .5f );
		Vector3 p2 = new Vector3( length * .5f, 	-width * .5f, -height * .5f );
		Vector3 p3 = new Vector3( -length * .5f,	-width * .5f, -height * .5f );
		
		Vector3 ph0 = new Vector3( -length * botLeftX,	-width * .5f, height * botLeftY );
		Vector3 ph1 = new Vector3( length * topRightX, 	-width * .5f, height * botLeftY );
		Vector3 ph2 = new Vector3( length * topRightX, 	-width * .5f, -height * topRightY );
		Vector3 ph3 = new Vector3( -length * botLeftX,	-width * .5f, -height * topRightY );
		
		Vector3 p4 = new Vector3( -length * .5f,	width * .5f,  height * .5f );
		Vector3 p5 = new Vector3( length * .5f, 	width * .5f,  height * .5f );
		Vector3 p6 = new Vector3( length * .5f, 	width * .5f,  -height * .5f );
		Vector3 p7 = new Vector3( -length * .5f,	width * .5f,  -height * .5f );
		
		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, ph1, ph0,
			p1, p2, ph2, ph1,
			p2, p3, ph3, ph2,
			p3, p1, ph0, ph3,
			
			// Left
			p7, p4, p0, p3,
			
			// Front
			p4, p5, p1, p0,
			
			// Back
			p6, p7, p3, p2,
			
			// Right
			p5, p6, p2, p1,
			
			// Top
			p7, p6, p5, p4
		};
		#endregion
		
		#region Normales
		Vector3 up 		= Vector3.up;
		Vector3 down 	= Vector3.down;
		Vector3 front 	= Vector3.forward;
		Vector3 back 	= Vector3.back;
		Vector3 left 	= Vector3.left;
		Vector3 right 	= Vector3.right;
		
		Vector3[] normales = new Vector3[]
		{
			// Bottom
			down, down, down, down,
			down, down, down, down,
			down, down, down, down,
			down, down, down, down,
			
			// Left
			left, left, left, left,
			
			// Front
			front, front, front, front,
			
			// Back
			back, back, back, back,
			
			// Right
			right, right, right, right,
			
			// Top
			up, up, up, up
		};
		#endregion	
		
		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( 1f, 0f );
		Vector2 _01 = new Vector2( 0f, 1f );
		Vector2 _11 = new Vector2( 1f, 1f );
		
		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,
			_11, _01, _00, _10,
			_11, _01, _00, _10,
			_11, _01, _00, _10,
			
			// Left
			_11, _01, _00, _10,
			
			// Front
			_11, _01, _00, _10,
			
			// Back
			_11, _01, _00, _10,
			
			// Right
			_11, _01, _00, _10,
			
			// Top
			_11, _01, _00, _10,
		};
		#endregion
		
		#region Triangles
		int[] triangles = new int[]
		{
			// Bottom
			3, 1, 0,
			3, 2, 1,
			
			6, 1, 2,
			6, 5, 1,
			
			9, 6, 10,
			9, 5, 6,
			
			9, 3, 0,
			9, 10, 3,
			
			// Left
			3 + 12 + 4 * 1, 1 + 12 + 4 * 1, 0 + 12 + 4 * 1,
			3 + 12 + 4 * 1, 2 + 12 + 4 * 1, 1 + 12 + 4 * 1,
			
			// Front
			3 + 12 + 4 * 2, 1 + 12 + 4 * 2, 0 + 12 + 4 * 2,
			3 + 12 + 4 * 2, 2 + 12 + 4 * 2, 1 + 12 + 4 * 2,
			
			// Back
			3 + 12 + 4 * 3, 1 + 12 + 4 * 3, 0 + 12 + 4 * 3,
			3 + 12 + 4 * 3, 2 + 12 + 4 * 3, 1 + 12 + 4 * 3,
			
			// Right
			3 + 12 + 4 * 4, 1 + 12 + 4 * 4, 0 + 12 + 4 * 4,
			3 + 12 + 4 * 4, 2 + 12 + 4 * 4, 1 + 12 + 4 * 4,
			
			// Top
			3 + 12 + 4 * 5, 1 + 12 + 4 * 5, 0 + 12 + 4 * 5,
			3 + 12 + 4 * 5, 2 + 12 + 4 * 5, 1 + 12 + 4 * 5,
			
		};
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		return mesh;
	}
}