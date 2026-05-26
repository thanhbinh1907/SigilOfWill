using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Spell Actions/Melee Spell Action")]
	public class MeleeSpellAction : SpellAction
	{
		public override void SpawnSpell(PlayerManager player)
		{
			Transform spawnLocation = player.playerEquipmentManager.rightHandSlot.transform;
			Vector3 spawnPosition = spawnLocation.position;
			Quaternion spawnRotation = spawnLocation.rotation;

			if (spellPrefab == null)
			{
				Debug.LogError($"Melee Spell '{name}' lacks spellPrefab!");
				return;
			}

			GameObject spellObj = Instantiate(spellPrefab, spawnPosition, spawnRotation);

			InitializeHitbox(spellObj, player);

			Debug.Log($"Melee Spell '{name}' spawned successfully.");
		}
	}
}
