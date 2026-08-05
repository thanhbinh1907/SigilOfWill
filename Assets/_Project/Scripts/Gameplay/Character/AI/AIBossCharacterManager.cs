using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
	public class AIBossCharacterManager : AICharacterManager
	{
		[Header("Boss Settings")]
		public int bossID = 0;
		[SerializeField] bool hasBeenAwakened = false;
		[SerializeField] bool hasBeenDefeated = false;
		public bool bossFightIsActive = false;

		[Header("Sleep & Awake Animations")]
		public string sleepAnimation = "Sleep_01";
		public string awakeAnimation = "Awaken_01";

		[Header("Sleep State")]
		public BossSleepState sleepState;

		[Header("Boss Fog Wall")]
		[SerializeField] private List<FogWallInteractable> myFogWalls = new List<FogWallInteractable>();

		[Header("Boss Music")]
		[SerializeField] AudioClip bossIntroMusic;
		[SerializeField] AudioClip bossLoopMusic;

		[Header("Phase Shift")]
		[SerializeField] float minimumHealthPercentageToShift = 0.5f;
		[SerializeField] bool hasShiftedPhase = false;
		[SerializeField] string phaseShiftDownAnimation = "Phase_Shift_Down_01";
		[SerializeField] float phaseShiftDownCrossfade = 0.25f;
		[SerializeField] string phaseShiftStunAnimation = "Phase_Shift_Stun_01";
		[SerializeField] float phaseShiftStunCrossfade = 0.25f;
		[SerializeField] float phaseShiftStunDuration = 3f;
		[SerializeField] string phaseShiftAnimation = "Phase_Shift_01";
		[SerializeField] float phaseShiftRecoverCrossfade = 0.25f;
		[SerializeField] CombatStanceState phase2CombatStance;
		[SerializeField] float phase2AttackSpeed = 1.5f;

		[Header("Test Debug")]
		[SerializeField] bool wakeBossUpDebug = false;

		protected override void Start()
		{
			base.Start();


			if (animator != null)
			{
				animator.speed = 1f;
			}

			if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
			{
				WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
				WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
				Debug.Log($"[HỆ THỐNG] Đã nhận diện Boss ID {bossID} thành công trên RAM!");
			}
			else
			{
				hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
				hasBeenAwakened = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
			}

			Debug.Log($"[HỆ THỐNG BOSS] Khởi tạo Boss ID {bossID}. hasBeenDefeated: {hasBeenDefeated}, hasBeenAwakened: {hasBeenAwakened}, sleepState có được gán không: {sleepState != null}");


			myFogWalls.Clear();
			FogWallInteractable[] allWalls = FindObjectsByType<FogWallInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			foreach (var fogWall in allWalls)
			{
				if (fogWall != null && fogWall.fogWallID == bossID)
				{
					myFogWalls.Add(fogWall);
				}
			}

			LoadBossAndFogWallStates();

			if (WorldAIManager.instance != null)
			{
				WorldAIManager.instance.RegisterBoss(this);
			}


			if (!hasBeenDefeated)
			{
				if (sleepState != null)
				{
					SetCurrentState(Instantiate(sleepState));
				}


				if (!hasBeenAwakened)
				{
					characterAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
				}
			}


			OnHealthChanged += CheckPhaseShift;
		}

		protected virtual void OnDestroy()
		{
			if (WorldAIManager.instance != null)
			{
				WorldAIManager.instance.UnregisterBoss(this);
			}
			OnHealthChanged -= CheckPhaseShift;
		}

		private void CheckPhaseShift(int oldValue, int newValue)
		{
			if (hasBeenDefeated || hasShiftedPhase)
				return;

			if (maxHealth <= 0)
				return;

			float healthPercent = (float)newValue / maxHealth;
			if (healthPercent <= minimumHealthPercentageToShift)
			{
				ShiftPhase();
			}
		}

		private void ShiftPhase()
		{
			StartCoroutine(ProcessPhaseShift());
		}

		private void PlayBossAnimation(string targetAnimation, float crossfadeTime)
		{
			if (characterAnimatorManager == null || string.IsNullOrEmpty(targetAnimation))
				return;

			applyRootMotion = true;
			isPerformingAction = true;
			canRotate = false;
			canMove = false;

			if (animator != null)
			{
				animator.CrossFade(targetAnimation, crossfadeTime);
			}
		}

		private IEnumerator PlayAnimationAndWait(string animationName, float crossfadeTime)
		{
			if (string.IsNullOrEmpty(animationName))
				yield break;

			PlayBossAnimation(animationName, crossfadeTime);

			if (animator != null)
			{

				float waitTransition = crossfadeTime + 0.05f;
				yield return new WaitForSeconds(waitTransition);


				var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				float duration = stateInfo.length;
				float remainingDuration = Mathf.Max(0f, duration - waitTransition);
				yield return new WaitForSeconds(remainingDuration);
			}
		}

		private IEnumerator ProcessPhaseShift()
		{
			hasShiftedPhase = true;


			isPerformingAction = true;
			canMove = false;
			canRotate = false;

			if (navMeshAgent != null && navMeshAgent.enabled)
			{
				navMeshAgent.isStopped = true;
			}


			if (!string.IsNullOrEmpty(phaseShiftDownAnimation))
			{
				Debug.Log($"[PHASE SHIFT] Boss ID {bossID} bắt đầu hoạt ảnh ngã xuống với Crossfade: {phaseShiftDownCrossfade}s.");
				yield return StartCoroutine(PlayAnimationAndWait(phaseShiftDownAnimation, phaseShiftDownCrossfade));
			}


			if (!string.IsNullOrEmpty(phaseShiftStunAnimation))
			{
				PlayBossAnimation(phaseShiftStunAnimation, phaseShiftStunCrossfade);
				Debug.Log($"[PHASE SHIFT] Boss ID {bossID} vào trạng thái Stun nằm đất trong {phaseShiftStunDuration} giây với Crossfade: {phaseShiftStunCrossfade}s.");
				yield return new WaitForSeconds(phaseShiftStunDuration);
			}


			if (!string.IsNullOrEmpty(phaseShiftAnimation))
			{
				Debug.Log($"[PHASE SHIFT] Boss ID {bossID} bắt đầu hoạt ảnh đứng dậy với Crossfade: {phaseShiftRecoverCrossfade}s.");
				yield return StartCoroutine(PlayAnimationAndWait(phaseShiftAnimation, phaseShiftRecoverCrossfade));
			}


			if (phase2CombatStance != null)
			{
				combatStance = Instantiate(phase2CombatStance);
				Debug.Log($"[PHASE SHIFT] Boss ID {bossID} đã đổi sang Combat Stance Phase 2 mới thành công.");
			}
			else
			{
				Debug.LogWarning($"[PHASE SHIFT] Boss ID {bossID} đổi phase nhưng phase2CombatStance bị NULL!");
			}


			if (animator != null)
			{
				animator.speed = phase2AttackSpeed;
				Debug.Log($"[PHASE SHIFT] Đã nâng tốc độ Animator của Boss ID {bossID} lên {phase2AttackSpeed} lần.");
			}


			isPerformingAction = false;
			canMove = true;
			canRotate = true;

			if (navMeshAgent != null && navMeshAgent.enabled)
			{
				navMeshAgent.isStopped = false;
			}
			Debug.Log($"[PHASE SHIFT] Boss ID {bossID} chính thức hoạt động lại ở Phase 2!");
		}

		protected override void Update()
		{
			base.Update();

			if (wakeBossUpDebug)
			{
				wakeBossUpDebug = false;
				WakeBoss();
			}
		}



		private void LoadBossAndFogWallStates()
		{
			if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
			{
				var saveData = WorldSaveGameManager.instance.currentCharacterData;

				if (!saveData.bossesAwakened.ContainsKey(bossID))
				{
					saveData.bossesAwakened[bossID] = false;
					saveData.bossesDefeated[bossID] = false;
				}
				else
				{
					hasBeenAwakened = saveData.bossesAwakened[bossID];
					hasBeenDefeated = saveData.bossesDefeated[bossID];
				}

				if (hasBeenDefeated)
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = false;
						}
					}
					gameObject.SetActive(false);
					return;
				}

				if (hasBeenAwakened)
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = true;
						}
					}
				}
				else
				{
					foreach (var fogWall in myFogWalls)
					{
						if (fogWall != null)
						{
							fogWall.IsActive = false;
						}
					}
				}
			}
		}

		public void WakeBoss()
		{
			Debug.Log($"[HỆ THỐNG BOSS] WakeBoss() được gọi cho Boss ID {bossID}. Trạng thái hiện tại - hasBeenDefeated: {hasBeenDefeated}, bossFightIsActive: {bossFightIsActive}");
			if (hasBeenDefeated || bossFightIsActive)
				return;


			bossFightIsActive = true;


			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
			{
				PlayerUIManager.instance.playerUIHudManager.AddBossHPBar(this);
			}
			else
			{
				Debug.LogError($"[HỆ THỐNG BOSS] Không thể hiển thị thanh máu vì PlayerUIManager ({PlayerUIManager.instance}) hoặc HUD Manager ({PlayerUIManager.instance?.playerUIHudManager}) đang bị NULL!");
			}


			if (WorldSoundFXManager.instance != null && (bossIntroMusic != null || bossLoopMusic != null))
			{
				WorldSoundFXManager.instance.PlayBossTrack(bossIntroMusic, bossLoopMusic);
			}


			SetCurrentState(idle);


			if (!hasBeenAwakened)
			{
				hasBeenAwakened = true;
				if (WorldSaveGameManager.instance != null && WorldSaveGameManager.instance.currentCharacterData != null)
				{
					WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID] = true;
					WorldSaveGameManager.instance.SaveGame();
				}

				if (characterAnimatorManager != null)
				{
					characterAnimatorManager.PlayTargetAnimation(awakeAnimation, true);
				}
			}


			foreach (var fogWall in myFogWalls)
			{
				if (fogWall != null)
				{
					fogWall.IsActive = true;
				}
			}

			Debug.Log($"[CHIẾN ĐẤU] Boss ID {bossID} chính thức vào trạng thái chiến đấu! Thanh máu UI đã hiển thị.");
		}

		public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
		{

			if (WorldSoundFXManager.instance != null)
			{
				WorldSoundFXManager.instance.StopBossMusic();
			}


			foreach (var fogWall in myFogWalls)
			{
				if (fogWall != null)
				{
					fogWall.IsActive = false;
				}
			}

			PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("GREAT FOE FELLED");

			_currentHealth = 0;
			isDead = true;


			bossFightIsActive = false;

			if (!manuallySelectDeathAnimation)
			{
				characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
			}

			hasBeenDefeated = true;

			WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID] = true;
			WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID] = true;

			WorldSaveGameManager.instance.SaveGame();


			yield return new WaitForSeconds(2.5f);

			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIHudManager != null)
			{
				PlayerUIManager.instance.playerUIHudManager.RemoveBossHPBar(this);
			}


			yield return new WaitForSeconds(2.5f);

			gameObject.SetActive(false);

			if (PlayerUIManager.instance != null && PlayerUIManager.instance.playerUIPopUpManager != null)
			{
				PlayerUIManager.instance.playerUIPopUpManager.DisplayDemoCompletionPopup();
			}
		}
	}
}
