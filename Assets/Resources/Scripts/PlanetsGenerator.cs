using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetsGenerator : MonoBehaviour {

	public TextMesh DebugTextPrefab;

	public SpaceGridObject Grid;

	public bool IsRecursive;

	public bool GenerateInnerUniverseFromTree;

	public Transform Parent;

	public byte PlanetsNum;

	int recursionDepth = 0;

	// Use this for initialization
	void Start () {
		if(GlobalData.FirstGeneration){
			UniverseData udata = new UniverseData();

			if(GlobalData.CurrentUniverse == null){
				udata.Name = "RootUniverse";
				GlobalData.CurrentUniverse = new TreeNode<UniverseData>(udata);
			} else {
				udata.Name = gameObject.name;
				GlobalData.CurrentUniverse.AddChild(udata);
			}

			int cellXCoord = Grid.NumCellsX / 2;
			int cellYCoord = Grid.NumCellsY / 2;
			int cellZCoord = Grid.NumCellsZ / 2;
			int initialCellXCoord = cellXCoord;
			int initialCellYCoord = cellYCoord;
			int initialCellZCoord = cellZCoord;
			// 1, 3, 5, 7, 9 multipillers are possible
			float planetSize = Grid.CellSize + ((Grid.CellSize * 2) * Random.Range(0, 5));
			udata.UniverseSize = Parent.transform.localScale;
			bool isPortalGenerated = false;
			
			for(int i = 0; i < PlanetsNum; i++){
				GameObject cubePlanet = new GameObject();
				cubePlanet.transform.parent = Parent;
				cubePlanet.transform.position = Grid.GetCellPositionAtIndex(cellXCoord, cellYCoord, cellZCoord);
				cubePlanet.transform.localScale = new Vector3(planetSize / Parent.transform.localScale.x, 
				                                              planetSize / Parent.transform.localScale.y, 
				                                              planetSize / Parent.transform.localScale.z);
				//check rotation
				Vector3 rotationAxis = (Random.Range(0, 2) == 1) ? Vector3.forward : Vector3.left;
//				cubePlanet.transform.Rotate(rotationAxis, 90 * Random.Range(0, 4));
				MeshFilter meshFilter = (MeshFilter)cubePlanet.AddComponent(typeof(MeshFilter));

				Color cubeColor = new Color(Random.value, Random.value, Random.value);
				MeshRenderer renderer = cubePlanet.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
				renderer.material.shader = Shader.Find ("Grid");
				renderer.material.color = cubeColor;
				renderer.castShadows = true;
				renderer.receiveShadows = true;

				//enshure we have at least one portal
				if(Random.Range(0, 11) == 10 || (i == PlanetsNum - 1 && !isPortalGenerated)){
					isPortalGenerated = true;
					cubePlanet.name = "PortalCube_" + System.Guid.NewGuid().ToString();
					float botLeftX = Random.Range(0.1f, 0.49f);
					float botLeftY = Random.Range(0.1f, 0.49f);
					float topRightX = Random.Range(0.1f, 0.49f);
					float topRightY = Random.Range(0.1f, 0.49f);
					meshFilter.mesh = Utils.GenerateBoxMesh(false);
					Utils.GenerateHoleOnTriangle(meshFilter, Random.Range(0, (meshFilter.mesh.triangles.Length - 1) / 3), DebugTextPrefab);
					Utils.MakeDoubleSidedMesh(meshFilter.mesh);
					//MeshHelper.Subdivide(meshFilter.mesh, 32);
					//Utils.MakeSomeNoise(meshFilter);
					
					//generate small universe inside
					//but first create new grid
					if(IsRecursive){
						moveRadom(cubePlanet);

						SpaceGridObject innerGrid = cubePlanet.AddComponent(typeof(SpaceGridObject)) as SpaceGridObject;
						innerGrid.NumCellsX = 64;
						innerGrid.NumCellsY = 64;
						innerGrid.NumCellsZ = 64;
						innerGrid.Margin = 3f / 64f;
						innerGrid.CellSize = (float)(planetSize / 64f);
						
						PlanetsGenerator innerGenerator = cubePlanet.AddComponent(typeof(PlanetsGenerator)) as PlanetsGenerator;
						innerGenerator.Grid = innerGrid;
						innerGenerator.IsRecursive = false;
						innerGenerator.Parent = cubePlanet.transform;
						innerGenerator.PlanetsNum = 32;
						
						//add door
						GameObject entrance = new GameObject();
						entrance.transform.parent = cubePlanet.transform;
						Vector3 entrancePos = new Vector3(cubePlanet.transform.position.x, cubePlanet.transform.position.y, cubePlanet.transform.position.z);
	//					entrance.transform.localScale = new Vector3((botLeftX + topRightX) * cubePlanet.transform.localScale.x,
	//					                                            2f,
	//					                                            (botLeftY + topRightY) * cubePlanet.transform.localScale.y);
						
	//					entrance.transform.localScale = new Vector3(cubePlanet.transform.localScale.x - 1f,
	//					                                            cubePlanet.transform.localScale.z - 1f,
	//					                                            cubePlanet.transform.localScale.y - 1f);

						entrance.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
						entrance.name = "Entrance";
						BoxCollider ecollider = entrance.AddComponent(typeof(BoxCollider)) as BoxCollider;
						ecollider.center = Vector3.zero;
						ecollider.size = Vector3.one;
						ecollider.isTrigger = true;
						entrance.transform.localPosition = Vector3.zero;
						//entrance.transform.Rotate(90f, 0f, 0f);
	//					entrance.transform.Rotate(cubePlanet.transform.rotation.eulerAngles);

						Destroy(cubePlanet.GetComponent<MeshCollider>());
						MeshCollider collider = cubePlanet.AddComponent(typeof(MeshCollider)) as MeshCollider;
						collider.convex = false;

						Rigidbody rb = (Rigidbody)cubePlanet.AddComponent(typeof(Rigidbody));
						rb.useGravity = false;
						rb.mass = planetSize * 1000f;
						rb.freezeRotation = true;
					}

					UniverseData.Planet planetData = new UniverseData.Planet();
					planetData.Position = new Vector3(cellXCoord, cellYCoord, cellZCoord);
					planetData.Rotation = cubePlanet.transform.rotation;
					planetData.Size = planetSize / Grid.CellSize;
					planetData.HoleBotLeftX = botLeftX;
					planetData.HoleBotLeftY = botLeftY;
					planetData.HoleTopRightX = topRightX;
					planetData.HoleTopRightY = topRightY;
					planetData.Name = cubePlanet.name;
					planetData.Color = cubeColor;
					udata.Planets.Add(planetData);
					
				} else {
					cubePlanet.name = "SateliteCube";
//					meshFilter.mesh = Utils.GenerateBoxMesh(false);
					
					if(IsRecursive){
						moveRadom(cubePlanet);

						BoxCollider collider = cubePlanet.AddComponent(typeof(BoxCollider)) as BoxCollider;
						collider.center = Vector3.zero;
						collider.size = Vector3.one;

						Rigidbody rb = (Rigidbody)cubePlanet.AddComponent(typeof(Rigidbody));
						rb.useGravity = false;
						rb.mass = planetSize * 1000f;
						rb.freezeRotation = true;
					}

					UniverseData.Satelite sateliteData = new UniverseData.Satelite();
					sateliteData.Name = cubePlanet.name;
					sateliteData.Position = new Vector3(cellXCoord, cellYCoord, cellZCoord);
					sateliteData.Rotation = cubePlanet.transform.rotation;
					sateliteData.Size = planetSize / Grid.CellSize;
					sateliteData.Color = cubeColor;
					udata.Satelites.Add(sateliteData);
				}
				
				//block cells
				Grid.GetCellAtIndex(cellXCoord, cellYCoord, cellZCoord).IsBlocked = true;
				int halfSizeIndexStep = (int)((planetSize - Grid.CellSize) / Grid.CellSize) / 2;
				for (int x = cellXCoord - halfSizeIndexStep; x <= cellXCoord + halfSizeIndexStep; x++) {
					for (int y = cellYCoord - halfSizeIndexStep; y <= cellYCoord + halfSizeIndexStep; y++) {
						for (int z = cellZCoord - halfSizeIndexStep; z <= cellZCoord + halfSizeIndexStep; z++) {
							try {
								Grid.GetCellAtIndex(x, y, z).IsBlocked = true;
							} catch {
								Debug.Log("X: " + x + " --- Y: " + y + " --- Z: " + z);
								return;
							}
						}
					}
				}
				
				//generate new planet size
				int oldSizeMeasuredInCells = (int)(planetSize / Grid.CellSize);
				planetSize = Grid.CellSize + ((Grid.CellSize * 2) * Random.Range(0, 5));
				
				//now we need to find free cells for it
				int newHalfSizeIndexStep = (int)(((planetSize - Grid.CellSize) / Grid.CellSize) / 2);
				int startX = initialCellXCoord - halfSizeIndexStep - newHalfSizeIndexStep - 1;
				int startY = initialCellYCoord - halfSizeIndexStep - newHalfSizeIndexStep - 1;
				int startZ = initialCellZCoord - halfSizeIndexStep - newHalfSizeIndexStep - 1;
				int lineSize = oldSizeMeasuredInCells + (int)(planetSize / Grid.CellSize) + 1;
				int cellXCoordCopy = cellXCoord;
				int cellYCoordCopy = cellYCoord;
				int cellZCoordCopy = cellZCoord;
				recursionDepth = 0;
				findPositionForNextPlanet(planetSize, lineSize, startX, startY, startZ, out cellXCoord, out cellYCoord, out cellZCoord);
				
				//if we fail then try to find position around current planet
				if(Grid.GetCellAtIndex(cellXCoord, cellYCoord, cellZCoord) == null){
					startX = cellXCoordCopy - halfSizeIndexStep - newHalfSizeIndexStep - 1;
					startY = cellYCoordCopy - halfSizeIndexStep - newHalfSizeIndexStep - 1;
					startZ = cellZCoordCopy - halfSizeIndexStep - newHalfSizeIndexStep - 1;
					initialCellXCoord = cellXCoordCopy;
					initialCellYCoord = cellYCoordCopy;
					initialCellZCoord = cellZCoordCopy;
					findPositionForNextPlanet(planetSize, lineSize, startX, startY, startZ, out cellXCoord, out cellYCoord, out cellZCoord);
					
					//if still nothing then skip it
					if(Grid.GetCellAtIndex(cellXCoord, cellYCoord, cellZCoord) == null){
						return;
					}
				}
			}
		} else if(!GlobalData.FirstGeneration || GenerateInnerUniverseFromTree){
			GlobalData.FirstGeneration = true;

			foreach(UniverseData.Planet planet in GlobalData.CurrentUniverse.Value.Planets){
				GameObject cubePlanet = new GameObject();
				cubePlanet.name = planet.Name;
				cubePlanet.transform.parent = Parent;
				cubePlanet.transform.position = Grid.GetCellPositionAtIndex((int)planet.Position.x, (int)planet.Position.y, (int)planet.Position.z);
				float planetSize = planet.Size * Grid.CellSize;
				cubePlanet.transform.localScale = new Vector3(planetSize / Parent.transform.localScale.x, 
				                                              planetSize / Parent.transform.localScale.y, 
				                                              planetSize / Parent.transform.localScale.z);
				cubePlanet.transform.rotation = planet.Rotation;
				MeshFilter meshFilter = (MeshFilter)cubePlanet.AddComponent(typeof(MeshFilter));
				//MeshFilter meshFilter = (MeshFilter)cubePlanet.AddComponent(typeof(MeshFilter));
				meshFilter.mesh = Utils.GenerateBoxMesh(false);
				Utils.GenerateHoleOnTriangle(meshFilter, Random.Range(0, (meshFilter.mesh.triangles.Length - 1) / 3), DebugTextPrefab);
				Utils.MakeDoubleSidedMesh(meshFilter.mesh);

				if(!GenerateInnerUniverseFromTree){
					MeshCollider collider = cubePlanet.AddComponent(typeof(MeshCollider)) as MeshCollider;
					collider.convex = false;

					Rigidbody rb = (Rigidbody)cubePlanet.AddComponent(typeof(Rigidbody));
					rb.useGravity = false;
					rb.mass = planetSize * 1000f;
					rb.freezeRotation = true;
				}

				MeshRenderer renderer = cubePlanet.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
				renderer.material.shader = Shader.Find ("Diffuse");
				renderer.material.color = planet.Color;
				renderer.castShadows = true;
				renderer.receiveShadows = true;

				if(!GenerateInnerUniverseFromTree){
					//generate new Universe?
					//check child nodes and compare names
					bool generateNewUniverse = true;
					for(int i = 0; i < GlobalData.CurrentUniverse.Children.Count; i++){
						if(GlobalData.CurrentUniverse[i].Value.Name.Equals(planet.Name)){
							// we found it
							generateNewUniverse = false;
							break;
						}
					}

					SpaceGridObject innerGrid = cubePlanet.AddComponent(typeof(SpaceGridObject)) as SpaceGridObject;
					innerGrid.NumCellsX = 64;
					innerGrid.NumCellsY = 64;
					innerGrid.NumCellsZ = 64;
					innerGrid.Margin = 3f / 64f;
					innerGrid.CellSize = (float)(planetSize / 64f);
					
					PlanetsGenerator innerGenerator = cubePlanet.AddComponent(typeof(PlanetsGenerator)) as PlanetsGenerator;
					innerGenerator.Grid = innerGrid;
					innerGenerator.IsRecursive = false;
					innerGenerator.GenerateInnerUniverseFromTree = !generateNewUniverse;
					innerGenerator.Parent = cubePlanet.transform;
					innerGenerator.PlanetsNum = 16;
					
					//add door
					GameObject entrance = new GameObject();
					entrance.transform.parent = cubePlanet.transform;
					Vector3 entrancePos = new Vector3(cubePlanet.transform.position.x, cubePlanet.transform.position.y, cubePlanet.transform.position.z);
					//					entrance.transform.localScale = new Vector3((botLeftX + topRightX) * cubePlanet.transform.localScale.x,
					//					                                            2f,
					//					                                            (botLeftY + topRightY) * cubePlanet.transform.localScale.y);
					
					//					entrance.transform.localScale = new Vector3(cubePlanet.transform.localScale.x - 1f,
					//					                                            cubePlanet.transform.localScale.z - 1f,
					//					                                            cubePlanet.transform.localScale.y - 1f);
					
					entrance.transform.localScale = new Vector3(0.97f, 0.97f, 0.97f);
					entrance.name = "Entrance";

					BoxCollider ecollider = entrance.AddComponent(typeof(BoxCollider)) as BoxCollider;
					ecollider.center = Vector3.zero;
					ecollider.size = Vector3.one;
					ecollider.isTrigger = true;
					entrance.transform.localPosition = Vector3.zero;

					moveRadom(cubePlanet);
				}
			}
			
			foreach(UniverseData.Satelite satelite in GlobalData.CurrentUniverse.Value.Satelites){
				GameObject cubePlanet = new GameObject();
				cubePlanet.name = satelite.Name;
				cubePlanet.transform.parent = Parent;
				cubePlanet.transform.position = Grid.GetCellPositionAtIndex((int)satelite.Position.x, (int)satelite.Position.y, (int)satelite.Position.z);
				float planetSize = satelite.Size * Grid.CellSize;
				cubePlanet.transform.localScale = new Vector3(planetSize / Parent.transform.localScale.x, 
				                                              planetSize / Parent.transform.localScale.y, 
				                                              planetSize / Parent.transform.localScale.z);
				cubePlanet.transform.rotation = satelite.Rotation;
				
				//MeshFilter meshFilter = (MeshFilter)cubePlanet.GetComponent(typeof(MeshFilter));
				//meshFilter.mesh = Utils.GenerateBoxMesh(false);

				if(!GenerateInnerUniverseFromTree){
					MeshCollider collider = cubePlanet.AddComponent(typeof(MeshCollider)) as MeshCollider;
					collider.convex = false;
					
					Rigidbody rb = (Rigidbody)cubePlanet.AddComponent(typeof(Rigidbody));
					rb.useGravity = false;
					rb.mass = planetSize * 100000f;
					rb.freezeRotation = true;
				}

				MeshRenderer renderer = cubePlanet.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
				renderer.material.shader = Shader.Find ("Diffuse");
				renderer.material.color = satelite.Color;
				renderer.castShadows = true;
				renderer.receiveShadows = true;

				moveRadom(cubePlanet);
			}
		}
	}

	void moveRadom(GameObject cubePlanet)
	{
		int axis = Random.Range(0, 3);
		float ammountDelta = Random.Range(0.5f, 2f);
		float time = Random.Range(2f, 7f);
		switch(axis){
		case 0: {
			LeanTween.moveLocalX(cubePlanet, cubePlanet.transform.localPosition.x + Grid.Margin * ammountDelta, time).setLoopOnce().setOnComplete(() => {moveRadom(cubePlanet);});
			break;
		}
		case 1: {
			LeanTween.moveLocalY(cubePlanet, cubePlanet.transform.localPosition.y + Grid.Margin * ammountDelta, time).setLoopOnce().setOnComplete(() => {moveRadom(cubePlanet);});
			break;
		}
		case 2: {
			LeanTween.moveLocalZ(cubePlanet, cubePlanet.transform.localPosition.z + Grid.Margin * ammountDelta, time).setLoopOnce().setOnComplete(() => {moveRadom(cubePlanet);});
			break;
		}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void findPositionForNextPlanet(float planetSize, int lineSize, int startX, int startY, int startZ, out int xCoord, out int yCoord, out int zCoord){
		List<SpaceCell> freeCells = new List<SpaceCell>(64);

		int side = Random.Range(0, 6);
		switch(side){
			case 0: {
				for (int x = startX; x < startX + lineSize; x++) {
					for (int z = startZ; z < startZ + lineSize; z++) {
						SpaceCell cell = Grid.GetCellAtIndex(x, startY, z);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
			case 1: {
				for (int x = startX; x < startX + lineSize; x++) {
					for (int z = startZ; z < startZ + lineSize; z++) {
						SpaceCell cell = Grid.GetCellAtIndex(x, startY + lineSize - 1, z);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
			case 2: {
				for (int y = startY; y < startY + lineSize; y++) {
					for (int z = startZ; z < startZ + lineSize; z++) {
						SpaceCell cell = Grid.GetCellAtIndex(startX, y, z);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
			case 3: {
				for (int y = startY; y < startY + lineSize; y++) {
					for (int z = startZ; z < startZ + lineSize; z++) {
						SpaceCell cell = Grid.GetCellAtIndex(startX + lineSize - 1, y, z);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
			case 4: {
				for (int x = startX; x < startX + lineSize; x++) {
					for (int y = startY; y < startY + lineSize; y++) {
						SpaceCell cell = Grid.GetCellAtIndex(x, y, startZ);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
			case 5: {
				for (int x = startX; x < startX + lineSize; x++) {
					for (int y = startY; y < startY + lineSize; y++) {
						SpaceCell cell = Grid.GetCellAtIndex(x, y, startZ + lineSize - 1);
						if(cell != null && !cell.IsBlocked)
							freeCells.Add(cell);
					}
				}
				break;
			}
		}

		freeCells.Shuffle();

		int newXPos = -1, newYPos = -1, newZPos = -1;
		//check cells around center
		bool spaceForNewCubeFound = true;
		foreach(SpaceCell spaceCell in freeCells){
			spaceForNewCubeFound = true;
			int halfSizeIndexStep = (int)(((planetSize - Grid.CellSize) / Grid.CellSize) / 2);
			for (int x = spaceCell.XIndex - halfSizeIndexStep; x <= spaceCell.XIndex + halfSizeIndexStep; x++) {
				for (int y = spaceCell.YIndex - halfSizeIndexStep; y <= spaceCell.YIndex + halfSizeIndexStep; y++) {
					for (int z = spaceCell.ZIndex - halfSizeIndexStep; z <= spaceCell.ZIndex + halfSizeIndexStep; z++) {
						SpaceCell tmpCellRef = Grid.GetCellAtIndex(x, y, z);
						if(tmpCellRef == null || tmpCellRef.IsBlocked){
							spaceForNewCubeFound = false;
							goto BreakLoops; 
						}
					}
				}
			}

		BreakLoops:
			
			if(spaceForNewCubeFound){
				newXPos = spaceCell.XIndex;
				newYPos = spaceCell.YIndex;
				newZPos = spaceCell.ZIndex;
//				Debug.Log("Space for new cube is found");
				break;
			} else {
				continue;
			}
			
		}

		if(freeCells.Count == 0)
			spaceForNewCubeFound = false;

		if(!spaceForNewCubeFound){
			findPositionForNextPlanet(planetSize, lineSize + 2, startX - 1, startY - 1, startZ - 1, out newXPos, out newYPos, out newZPos);
		}
		 
		xCoord = newXPos;
		yCoord = newYPos;
		zCoord = newZPos;

//		Debug.Log("X: " + xCoord + " --- Y: " + yCoord + " --- Z: " + zCoord);
//		Debug.Log("Recursion depth: " + recursionDepth);

		recursionDepth++;
	}


}
