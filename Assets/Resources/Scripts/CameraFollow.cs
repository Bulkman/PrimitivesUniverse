using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	// Using UpdateMode you can select when the camera should be updated
	// Depending on your design camera jitter may occur which may be reduced if you change
	// the update mode. In the included demo the best result is achieved when camera is
	// updated in the FixedUpdate() function.
	
	public enum UpdateMode { FIXED_UPDATE, UPDATE, LATE_UPDATE }
	public UpdateMode CameraUpdateMode = UpdateMode.FIXED_UPDATE;
	
	// Select the chase mode (chase behind target or moving spectator)
	// CHASE = smooth chase behind target at distance and height
	// SPECTATOR = smooth look at target from a chasing spectator position
	public enum FollowMode { CHASE, SPECTATOR }	
	public FollowMode CameraFollowMode = FollowMode.SPECTATOR;
	
	// Target to follow
	public Transform Target;
		
	// Distance to follow from (this is the minimum distance, 
	// depending on damping the distance will increase at speed)
	public float Distance = 60.0f;	
	// Height for chase mode camera
	public float ChaseHeight = 15.0f;
	
	// Follow (movement) damping. Lower value = smoother
	public float FollowDamping = 0.3f;
	// Look at (rotational) damping. Lower value = smoother
	public float LookAtDamping = 4.0f;	
	// Optional hotkey for freezing camera movement
	public KeyCode FreezeKey = KeyCode.None;
		
	private Transform cacheTransform;
	
	void Start () {
		// Cache reference to transform to increase performance
		cacheTransform = transform;
	}
	
	void FixedUpdate () {
		if (CameraUpdateMode == UpdateMode.FIXED_UPDATE) DoCamera();
	}	
	void Update () {
		if (CameraUpdateMode == UpdateMode.UPDATE) DoCamera();
	}
	void LateUpdate () {
		if (CameraUpdateMode == UpdateMode.LATE_UPDATE) DoCamera();
	}
	
	void DoCamera () {
		// Return if no target is set
		if (Target == null) return;
		
		Quaternion lookAt;
		
		switch (CameraFollowMode) {
		case FollowMode.SPECTATOR:
			// Smooth lookat interpolation
			lookAt = Quaternion.LookRotation(Target.position - cacheTransform.position);
			cacheTransform.rotation = Quaternion.Lerp(cacheTransform.rotation, lookAt, Time.deltaTime * LookAtDamping);
			// Smooth follow interpolation
			if (!Input.GetKey(FreezeKey)) {
				if (Vector3.Distance(cacheTransform.position, Target.position) > Distance) {					
					cacheTransform.position = Vector3.Lerp(cacheTransform.position, Target.position, Time.deltaTime * FollowDamping);
				}
			}
			break;			
		case FollowMode.CHASE:			
			if (!Input.GetKey(FreezeKey)) {	
				// Smooth lookat interpolation
				lookAt = Target.rotation;
				cacheTransform.rotation = Quaternion.Lerp(cacheTransform.rotation, lookAt, Time.deltaTime * LookAtDamping);						
				// Smooth follow interpolation
				cacheTransform.position = Vector3.Lerp(cacheTransform.position, Target.position - Target.forward * Distance + Target.up * ChaseHeight, Time.deltaTime * FollowDamping * 10) ;
			}
			break;
		}						
	}	
}
