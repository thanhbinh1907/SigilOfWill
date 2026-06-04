using UnityEngine;

namespace SG
{
	[CreateAssetMenu(menuName = "A.I/States/Boss Sleep")]
	public class BossSleepState : AIState
	{
		public override AIState Tick(AICharacterManager aiCharacter)
		{
			// Đứng im, không tìm mục tiêu, không đuổi theo Player, trả về chính nó
			return this;
		}
	}
}
