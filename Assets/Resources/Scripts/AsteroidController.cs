using UnityEngine;
using System.Collections;

public class AsteroidController : MonoBehaviour {
	public float MinSize;

	public float MaxSize;

	public float Force;

	public float AutoDestroyTime = 2.0f;

	public Transform ExplosionEffect;

	private Rigidbody cachedRigidbody;

	private SpaceGridObject rootGrid;

	private Vector3 dir;
	private Vector3 cross;
	private Vector3 side;
	private Vector3 target;

	// Use this for initialization
	void Start () {
		cachedRigidbody = rigidbody;

		if (cachedRigidbody == null) {
			UnityEngine.Debug.LogError("Asteroid has no rigidbody.");
		}

		cachedRigidbody.mass *= Random.Range(5f, 50f);

		//cachedRigidbody.AddRelativeForce(Random.onUnitSphere * Random.Range(5000000f, 20000000f));

		renderer.material.color = new Color(Random.value, Random.value, Random.value);

		float scale = Random.Range(MinSize, MaxSize);
		transform.localScale = new Vector3(scale, scale, scale);

		rootGrid = GameObject.FindGameObjectWithTag("UniverseRootObject").GetComponent<SpaceGridObject>();

		target = rootGrid.GridBoundsCenter + Random.onUnitSphere * 100f;
		dir = target - transform.position;
		side = Vector3.Cross(dir, transform.forward);

		//StartCoroutine(CheckOutOfBounds());
	}

	void OnCollisionrEnter(Collider other) {
		if(!other.tag.Equals("Asteroid")){
			Destroy(Instantiate(ExplosionEffect.gameObject, transform.position, Random.rotation), AutoDestroyTime);
		}
	}

	void FixedUpdate () {
		float dst = Vector3.Distance(target, transform.position);
		if(dst > rootGrid.GridBoundsSize.x * 0.75f){
			rigidbody.drag = 0.65f;
		} else if(dst < rootGrid.GridBoundsSize.x * 0.15f){
			rigidbody.drag = 0.05f;
		}

		dir = target - transform.position;
		rigidbody.AddForce(dir.normalized * Force);
		cross = Vector3.Cross(dir, side);
		rigidbody.AddForce(cross.normalized * Force);
	}

	// Update is called once per frame
	IEnumerator CheckOutOfBounds () {
		while (true){
			if(!rootGrid.GridBounds.Contains(transform.position)){
				// reflect our old velocity off the contact point's normal vector
				//Vector3 reflectedVelocity = Vector3.Reflect(cachedRigidbody.velocity, cachedRigidbody.velocity);        
				
				// assign the reflected velocity back to the rigidbody
				cachedRigidbody.velocity = (transform.position - (rootGrid.GridBoundsCenter + Random.onUnitSphere * 100f)).normalized * -cachedRigidbody.velocity.magnitude;
			}

			yield return new WaitForSeconds(1.0f);
		}
	}
}
