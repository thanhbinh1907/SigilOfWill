using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class WorldObjectManager : MonoBehaviour
	{
		public static WorldObjectManager instance;

		[Header("List Fog Walls In World")]
		public List<FogWallInteractable> fogWalls = new List<FogWallInteractable>();

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void AddFogWallToList(FogWallInteractable fogWall)
		{
			if (!fogWalls.Contains(fogWall))
			{
				fogWalls.Add(fogWall);
			}
		}

		public void RemoveFogWallFromList(FogWallInteractable fogWall)
		{
			if (fogWalls.Contains(fogWall))
			{
				fogWalls.Remove(fogWall);
			}
		}
	}
}