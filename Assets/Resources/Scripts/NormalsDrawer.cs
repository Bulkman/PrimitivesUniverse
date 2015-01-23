using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class NormalsDrawer : MonoBehaviour
{	public float NormalsScale = 1.0f;

	private Mesh mesh;
	// Use this for initialization
	void Start ()
	{
		mesh = GetComponent<MeshFilter>().mesh;
	}

	void OnDrawGizmos() {
		if(mesh == null)
			mesh = GetComponent<MeshFilter>().mesh;

		for(int i = 0; i < mesh.vertices.Length; i++){
			Gizmos.color = Color.red;

			Gizmos.DrawLine(transform.TransformPoint(mesh.vertices[i]), transform.TransformPoint(mesh.vertices[i] + mesh.normals[i] * NormalsScale));
		}
	}
}

