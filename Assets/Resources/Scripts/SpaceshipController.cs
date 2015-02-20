using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class SpaceshipController : MonoBehaviour {
	//Joystick
	public CNAbstractController MovementJoystick;
	public CNButton FireBtn;
	public CNButton ThrustBtn;
	// Grid
	public SpaceGridObject Grid;
	// Array of thrusters attached to the spaceship
	public Thruster[] Thrusters;
	// Specify the roll rate (multiplier for rolling the ship when steering left/right)	
	public float RollRate = 100.0f;
	// Specify the yaw rate (multiplier for rudder/steering the ship when steering left/right)
	public float YawRate = 30.0f;
	// Specify the pitch rate (multiplier for pitch when steering up/down)
	public float PitchRate = 100.0f;
	// Weapon mount points on ship (this is where lasers will be fired from)
	public Vector3[] WeaponMountPoints;	
	// Laser shot prefab
	public Transform LaserShotPrefab;
	// Laser shot sound effect
	public AudioClip SoundEffectFire;
	
	// Private variables
	private Rigidbody cachedRigidbody;

	private Stopwatch shootingTimer;

	private Vector3 prevPos;

	private SpaceGridObject rootGrid;
	
	void Start () {		
		// Ensure that the thrusters in the array have been linked properly
		foreach (Thruster thruster in Thrusters) {
			if (thruster == null) {
				UnityEngine.Debug.LogError("Thruster array not properly configured. Attach thrusters to the game object and link them to the Thrusters array.");
			}			
		}
		
		// Cache reference to rigidbody to improve performance
		cachedRigidbody = rigidbody;
		if (cachedRigidbody == null) {
			UnityEngine.Debug.LogError("Spaceship has no rigidbody - the thruster scripts will fail. Add rigidbody component to the spaceship.");
		}

		shootingTimer = new Stopwatch();
		shootingTimer.Start();

		SpaceCell freeCell = Grid.GetRandomFreeCell();
		transform.position = Grid.GetCellPositionAtIndex(freeCell.XIndex, freeCell.YIndex, freeCell.ZIndex);
		transform.LookAt(Grid.Center);

		rootGrid = GameObject.FindGameObjectWithTag("UniverseRootObject").GetComponent<SpaceGridObject>();
	}

	void OnTriggerEnter(Collider other) {
		GlobalData.FirstGeneration = false;
		if(other.name.Equals("Entrance")){
			UnityEngine.Debug.Log ("Entering new universe...");
			for(int i = 0; i < GlobalData.CurrentUniverse.Children.Count; i++){
				if(GlobalData.CurrentUniverse[i].Value.Name.Equals(other.transform.parent.name)){
					GlobalData.CurrentUniverse = GlobalData.CurrentUniverse[i];
					break;
				}
			}
		}

		Application.LoadLevel(Application.loadedLevel);
	}

	void Update () {
		//GlobalData.CurrentUniverse indicates that we are not in the 1 node
		if(!rootGrid.GridBounds.Contains(transform.position)){
			if(GlobalData.CurrentUniverse.Parent != null){
				UnityEngine.Debug.Log ("Entering previous universe...");
				GlobalData.FirstGeneration = false;
				GlobalData.CurrentUniverse = GlobalData.CurrentUniverse.Parent;
				Application.LoadLevel(Application.loadedLevel);
			} else {
				//dont let ship leave bounds
				cachedRigidbody.position = prevPos;
			}
		}

		// Start all thrusters when pressing Fire 1
		bool thrustOn;
		bool thrustOff;
		bool fire;
#if UNITY_PSM
		thrustOn = Input.GetButtonDown("Fire1");
		thrustOff = Input.GetButtonUp("Fire1");
		fire = Input.GetButton("Fire2");
#else
		thrustOn = ThrustBtn.Pressed;
		thrustOff = !ThrustBtn.Pressed;
		fire = FireBtn.Pressed;
#endif

		if (thrustOn) {		
			foreach (Thruster thruster in Thrusters) {
				thruster.StartThruster();
			}
		}
		// Stop all thrusters when releasing Fire 1
		if (thrustOff) {		
			foreach (Thruster thruster in Thrusters) {
				thruster.StopThruster();
			}
		}
		
		if (fire && shootingTimer.ElapsedMilliseconds >= 200) {
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

		prevPos = cachedRigidbody.position;
	}

	void FixedUpdate () {
		// In the physics update...
		// Add relative rotational roll torque when steering left/right
		float roll = 0.0f;
		// Add rudder yaw torque when steering left/right
		float yaw = 0.0f;
		// Add pitch torque when steering up/down
		float pitch = 0.0f;



#if UNITY_PSM
		roll = -Input.GetAxis("Horizontal")*RollRate*cachedRigidbody.mass;
		yaw = Input.GetAxis("Horizontal")*YawRate*cachedRigidbody.mass;
		pitch = -Input.GetAxis("Vertical")*PitchRate*cachedRigidbody.mass;
#else
		roll = -MovementJoystick.GetAxis("Horizontal")*RollRate*cachedRigidbody.mass;
		yaw = MovementJoystick.GetAxis("Horizontal")*YawRate*cachedRigidbody.mass;
		pitch = -MovementJoystick.GetAxis("Vertical")*PitchRate*cachedRigidbody.mass;
#endif

		//cachedRigidbody.AddRelativeTorque(new Vector3(0,0,roll));
		cachedRigidbody.AddRelativeTorque(new Vector3(0,yaw,0));
		cachedRigidbody.AddRelativeTorque(new Vector3(pitch,0,0));	
	}
}
