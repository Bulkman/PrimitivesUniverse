using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class EnemyShipController : MonoBehaviour {

	private GameObject player;
	
	public float MoveSpeedMultipiller = 250;

	public float MinAgroDistance = 50.0f;

	public float MaxAgroDistance = 100.0f;

	public float RotationDamping = 0.75f;

	public bool CanShoot = true;

	public float BrakeForce = 5f;

	public Transform LaserShotPrefab;

	public Vector3[] WeaponMountPoints;

	public AudioClip SoundEffectFire;


	private bool isShooting = false;

	private Vector3 direction;

	private float distance = 0.0f;

	private Rigidbody cachedRigidbody;

	private Stopwatch shootingTimer;
	
	public enum CurrentState 
	{ 	
		Idle, 
		Following, 
		Attacking
	};

	public CurrentState currentState;
	public bool debugGizmo = true;
	
	public float DistanceToPlayer { get { return distance; } }
	
	void Start()
	{  
		cachedRigidbody = rigidbody;
		if (cachedRigidbody == null) {
			UnityEngine.Debug.LogError("Enemy ship has no rigidbody");
		}

		player = GameObject.FindGameObjectWithTag("Player");
		currentState = CurrentState.Idle;
		isShooting = false;

		renderer.material.color = new Color(1f, 0.05f, 0.05f);

		float scale = Random.Range(0.11f, 0.22f);

		shootingTimer = new Stopwatch();
		shootingTimer.Start();
	}
	
	void Update() 
	{
		//Find the distance to the player
		distance = Vector3.Distance(player.transform.position, this.transform.position);
		
		//Face the drone to the player
		direction = (player.transform.position - this.transform.position);
		direction.Normalize();
	}
	
	void FixedUpdate()
	{  
		cachedRigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
		cachedRigidbody.angularDrag = RotationDamping;
		
		//If the player is in range move towards
		if(distance > MinAgroDistance && distance < MaxAgroDistance )
		{
			currentState = CurrentState.Following;
			DroneMovesToPlayer();
		}
		
		//If the player is close enough shoot
		else if(distance < MinAgroDistance)
		{
			
			DroneStopsMoving();
			
			if(CanShoot)
			{
				currentState = CurrentState.Attacking;
				ShootPlayer();
			}
		}
		
		//If the player is out of range stop moving
		else
		{
			currentState = CurrentState.Idle;
			DroneStopsMoving();
		}
	}
	
	void DroneStopsMoving()
	{ 
		isShooting = false;
		cachedRigidbody.drag = (BrakeForce);
	}
	
	void DroneMovesToPlayer()
	{
		isShooting = false;
		cachedRigidbody.AddRelativeForce(Vector3.forward * MoveSpeedMultipiller);
	}
	
	void ShootPlayer()
	{
		isShooting = true;

		if (CanShoot && shootingTimer.ElapsedMilliseconds >= 400) {
			shootingTimer.Reset();
			shootingTimer.Start();
			// Itereate through each weapon mount point Vector3 in array
			foreach (Vector3 wmp in WeaponMountPoints) {
				// Calculate where the position is in world space for the mount point
				Vector3 pos = transform.position + transform.right * wmp.x + transform.up * wmp.y + transform.forward * wmp.z;
				// Instantiate the laser prefab at position with the spaceships rotation
				Transform laserShot = (Transform) Instantiate(LaserShotPrefab, pos, transform.rotation);
				// Specify which transform it was that fired this round so we can ignore it for collision/hit
				laserShot.GetComponent<LaserShot>().FiredBy = transform;
			}
			// Play sound effect when firing
			if (SoundEffectFire != null) {
				audio.PlayOneShot(SoundEffectFire);
			}
		}		
	}
	
	void OnDrawGizmosSelected()
	{
		if (debugGizmo)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.transform.position, MaxAgroDistance);
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.transform.position, MinAgroDistance);
		}
	}
}
