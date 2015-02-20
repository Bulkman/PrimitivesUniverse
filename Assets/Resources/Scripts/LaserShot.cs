using UnityEngine;
using System.Collections;

public class LaserShot : MonoBehaviour {	
	// Default life of laser beam
	public float Life = 2.0f;
	// Default velocity of laser beam
	public float Velocity = 1000.0f;
	// Reference to impact effect prefab to spawn upon impact
	public Transform ImpactEffect;
	// Reference to explosion effect prefab to spawn if object is destroyed
	public Transform ExplosionEffect;
	// "Fired By" Reference to ignore collision detection for the ship that fired the laser
	public Transform FiredBy {get; set;}

	public float AutoDestroyTime = 2.0f;
	
	// Private variables
	private Vector3 velocity;
	private Vector3 newPos;
	private Vector3 oldPos;	
	
	void Start () {
		// Set the new position to the current position of the transform
		newPos = transform.position;
		// Set the old position to the same value
		oldPos = newPos;			
		// Set the velocity vector 3 to the specified velocity and set the direction to face forward of the transform
		velocity = Velocity * transform.forward;
		// Set the gameobject to destroy after period "life"
		Destroy(gameObject, Life);
	}

	int tmpTweenDescr0;
	int tmpTweenDescr1;

	void Update () {
		// Change new position by the velocity magnitude (in the direction of transform.forward) and since
		// we are in the update function we need to multiply by deltatime.
		newPos += transform.forward * velocity.magnitude * Time.deltaTime;
		// SDet direction to the difference between new position and old position
		Vector3 direction = newPos - oldPos;
		// Get the distance which is the magnitude of the direction vector
		float distance = direction.magnitude;
				
		// If distance is greater than nothing...
		if (distance > 0) {
			// Define a RayCast
			RaycastHit hit;
			// If the raycast from previous position in the specified direction at (or before) the distance...
			if (Physics.Raycast(oldPos, direction, out hit, distance)) {
				// and if the transform we hit isn't a the ship that fired the weapon and the collider isn't just a trigger...
				if (hit.transform != FiredBy && !hit.collider.isTrigger) {		
					// Set the rotation of the impact effect to the normal of the impact surface (we wan't the impact effect to
					// throw particles out from the object we just hit...
					Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
					// Instantiate the imapct effect at impact position
					Instantiate(ImpactEffect, hit.point, rotation);
					// If random number is a small value...
					if (Random.Range(0, 10) < 2) {
						// Instantiate the explosion effect at the point of impact and autodestroy it
						Destroy(Instantiate(ExplosionEffect.gameObject, hit.transform.position, rotation), AutoDestroyTime);
						// Destroy the game object that we just hit
						if(!LeanTween.isTweening(tmpTweenDescr0) && !LeanTween.isTweening(tmpTweenDescr1)){
							Vector3 tmpScale = hit.transform.localScale;
							tmpTweenDescr0 = LeanTween.scale(hit.transform.gameObject, Vector3.zero, 0.33f).id;
							tmpTweenDescr1 = LeanTween.delayedCall(15.33f, () => { LeanTween.scale(hit.transform.gameObject, tmpScale, 0.33f); }).id;
						}
					}
					// Destroy the laser shot game object
					Destroy(gameObject);
				}
			}
		}
		// Set the old position tho the current position
		oldPos = transform.position;
		// Set the actual position to the new position
		transform.position = newPos;		
	}
}
