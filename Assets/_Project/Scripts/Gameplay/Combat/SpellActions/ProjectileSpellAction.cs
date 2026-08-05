using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "Character Actions/Spell Actions/Projectile Spell Action")]
	public class ProjectileSpellAction : SpellAction
	{
		[Header("Projectile Settings (Overrides Base)")]
		public float speed = 15f;

		public override void SpawnSpell(PlayerManager player)
		{
			Transform spawnLocation = null;

			if (player.playerEquipmentManager != null && player.playerEquipmentManager.rightWeaponManager != null)
			{
				spawnLocation = player.playerEquipmentManager.rightWeaponManager.spellSpawnPoint;
			}

			if (spawnLocation == null)
			{
				spawnLocation = player.playerEquipmentManager.rightHandSlot.transform;
			}

			Vector3 spawnPosition = spawnLocation.position;
			Vector3 shootDirection = player.transform.forward;

			if (player.isLockOn && player.playerCombatManager.currentTarget != null)
			{
				Transform targetTransform = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform;
				Vector3 targetPos = targetTransform != null ? targetTransform.position :
					player.playerCombatManager.currentTarget.transform.position;
				shootDirection = targetPos - spawnLocation.position;
			}

			shootDirection.Normalize();
			Quaternion spawnRotation = Quaternion.LookRotation(shootDirection);

			if (spellPrefab == null)
			{
				return;
			}

			GameObject spellObj = Instantiate(spellPrefab, spawnPosition, spawnRotation);

			InitializeHitbox(spellObj, player);

			Rigidbody rb = spellObj.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.linearVelocity = shootDirection * speed;
			}

		}
	}
}
