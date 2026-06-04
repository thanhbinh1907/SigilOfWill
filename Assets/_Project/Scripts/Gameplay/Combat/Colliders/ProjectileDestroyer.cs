using UnityEngine;

public class ProjectileDestroyer : MonoBehaviour
{
	public float lifeTime = 5f; 

	void Start()
	{
		Destroy(gameObject, lifeTime);
	}
}