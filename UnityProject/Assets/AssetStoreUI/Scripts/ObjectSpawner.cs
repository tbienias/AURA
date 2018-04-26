using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ObjectSpawner : MonoBehaviour
{
	private static ObjectSpawner _instance;

    private static float distance = 2;

	[SerializeField]
	private GameObject _cubePrefab;

	public static ObjectSpawner Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			ObjectSpawner spawner = FindObjectOfType<ObjectSpawner>() ??
			                        new GameObject(typeof(ObjectSpawner) + "(Singleton)").AddComponent<ObjectSpawner>();
			return _instance ?? (_instance = spawner);
		}
	}

	public void Spawn(Vector3 offset)
	{
		GameObject objToSpawn = _cubePrefab;
		GameObject cam = GameObject.Find("MixedRealityCamera");

		Instantiate(objToSpawn, (cam.transform.position + offset), cam.transform.rotation);
		Debug.Log("Spawned cube at: " + cam.transform.position);
	}

    public void spawnCube()
    {
        Spawn(Camera.main.transform.forward * distance);
    }

    public void testCube()
    {
		Spawn(Vector3.zero);
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance == this)
			_instance = null;
	}
}
