using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Spell Actions/Sky Spell Action")]
	public class SkySpellAction : SpellAction
	{
		[Header("Sky Spell Settings")]
		public float spawnForwardOffset = 5f;

		public override void SpawnSpell(PlayerManager player)
		{
			Vector3 spawnPosition;

			if (player.isLockOn && player.playerCombatManager.currentTarget != null)
			{
				spawnPosition = player.playerCombatManager.currentTarget.transform.position;
			}
			else
			{
				spawnPosition = player.transform.position + player.transform.forward * spawnForwardOffset;
				
				RaycastHit hit;
				if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, 
					WorldUtilityManager.instance.GetEnvironmentLayers()))
				{
					spawnPosition.y = hit.point.y;
				}
				else
				{
					spawnPosition.y = player.transform.position.y;
				}
			}
			Quaternion spawnRotation = Quaternion.identity;

			if (spellPrefab == null)
			{
				return;
			}

			GameObject spellObj = Instantiate(spellPrefab, spawnPosition, spawnRotation);

			InitializeHitbox(spellObj, player);
		}
	}
}
