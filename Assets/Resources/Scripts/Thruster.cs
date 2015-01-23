using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {
	// Force of individual thrusters
	public float ThrusterForce = 10000;
	// Whether or not to add force at position which introduces torque, use with care...
	public bool AddForceAtPosition = false;
	// Sound effect volume of thruster
	public float SoundEffectVolume = 1.0f;
	
	// Private variables
	private bool isActive = false;	
	private Transform cacheTransform;
	private Rigidbody cacheParentRigidbody;
	private Light cacheLight;
	private ParticleSystem cacheParticleSystem;
	
	// Call StartThruster() function from other scripts to start the thruster
	public void StartThruster() {
		// Set the thruster active flag to true
		isActive = true; 
	}
	
	// Call StopThruster() function from other scripts to stop the thruster
	public void StopThruster() {
		// Set the thruster active flag to false		
		isActive = false; 
	}
	
	void Start () {
		// Cache the transform and parent rigidbody to improve performance
		cacheTransform = transform;
		
		// Ensure that parent object (e.g. spaceship) has a rigidbody component so it can apply force.
		if (transform.parent.rigidbody != null) {			
			cacheParentRigidbody = transform.parent.rigidbody;
		} else {
			Debug.LogError("Thruster has no parent with rigidbody that it can apply the force to.");
		}
		
		// Cache the light source to improve performance (also ensure that the light source in the prefab is intact.)
		cacheLight = transform.GetComponent<Light>().light;
		if (cacheLight == null) {
			Debug.LogError("Thruster prefab has lost its child light. Recreate the thruster using the original prefab.");
		}
		// Cache the particle system to improve performance (also ensure that the particle system in the rpefab is intact.)
		cacheParticleSystem = particleSystem;
		if (cacheParticleSystem == null) {
			Debug.LogError("Thruster has no particle system. Recreate the thruster using the original prefab.");
		}
		
		// Start the audio loop playing but mute it. This is to avoid play/stop clicks and clitches that Unity may produce.
		audio.loop = true;
		audio.volume = SoundEffectVolume;
		audio.mute = true;
		audio.Play();		
	}	
	
	void Update () {
		// If the light source of the thruster is intact...
		if (cacheLight != null) {
			// Set the intensity based on the number of particles
			cacheLight.intensity = cacheParticleSystem.particleCount / 6;
		}
		
		// If the thruster is active...
		if (isActive) {
			// ...and if audio is muted...
//			if (audio.mute) {
//				// Unmute the audio
//				audio.mute=false;
//			}
//			// If the audio volume is lower than the sound effect volume...
//			if (audio.volume < SoundEffectVolume) {
//				// ...fade in the sound (to avoid clicks if just played straight away)
//				audio.volume += 5f * Time.deltaTime;
//			}
			
			// If the particle system is intact...
			if (cacheParticleSystem != null) {	
				// Enable emission of thruster particles
				cacheParticleSystem.enableEmission = true;
			}		
		} else {
			// The thruster is not active
//			if (audio.volume > 0.01f) {
//				// ...fade out volume
//				audio.volume -= 5f * Time.deltaTime;	
//			} else {
//				// ...and mute it when it has faded out
//				audio.mute = true;
//			}
//			
			// If the particle system is intact...
			if (cacheParticleSystem != null) {				
				// Stop emission of thruster particles
				cacheParticleSystem.enableEmission = false;				
			}
			
		}
	
	}
	
	void FixedUpdate() {
		// If the thruster is active...
		if (isActive) {
			// ...add the relative thruster force to the parent object
			if (AddForceAtPosition) {
				// Add force relative to the position on the parent object which will also apply rotational torque
				cacheParentRigidbody.AddForceAtPosition (cacheTransform.up * ThrusterForce, cacheTransform.position);
			} else {
				// Add force without rotational torque
				cacheParentRigidbody.AddRelativeForce (Vector3.forward * ThrusterForce);				
			}
		}		
	}
}
