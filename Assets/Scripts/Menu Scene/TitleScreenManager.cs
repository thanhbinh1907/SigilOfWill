using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
	public class TitleScreenManager : MonoBehaviour
	{
		public void StartNewGame()
		{
			WorldSaveGameManager.instance.StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
		}
	}
}