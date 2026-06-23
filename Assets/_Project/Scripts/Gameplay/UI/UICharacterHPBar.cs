using UnityEngine;
using TMPro;

namespace SG
{
    public class UICharacterHPBar : UI_StatBar
    {
        private CharacterManager character;

        [Header("Name Settings")]
        [SerializeField] bool displayCharacterNameOnDamage = false;
        [SerializeField] TextMeshProUGUI characterNameText;

        [Header("Damage Settings")]
        [SerializeField] TextMeshProUGUI characterDamageText;
        [SerializeField] int currentDamageTaken = 0;
        public int oldHealthValue = 0;

        [Header("Visibility Timer")]
        [SerializeField] float defaultTimeBeforeBarHides = 3f;
        [SerializeField] float hideTimer = 0f;

        protected override void Awake()
        {
            base.Awake();

            character = GetComponentInParent<CharacterManager>();
        }

        protected void Start()
        {

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {

            currentDamageTaken = 0;
        }

        private void Update()
        {

            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }


            if (hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }


        public void SetCharacterStat(int newValue)
        {
            if (character == null) return;


            CheckSlider();

            if (slider != null)
            {

                slider.maxValue = character.maxHealth;

                slider.value = newValue;
            }


            int damageDelta = oldHealthValue - newValue;
            currentDamageTaken += damageDelta;


            if (characterDamageText != null)
            {
                if (currentDamageTaken < 0)
                {
                    characterDamageText.color = Color.green;
                    characterDamageText.text = "+" + Mathf.Abs(currentDamageTaken).ToString();
                }
                else
                {
                    characterDamageText.color = Color.red;
                    characterDamageText.text = "-" + currentDamageTaken.ToString();
                }
            }


            if (displayCharacterNameOnDamage && characterNameText != null)
            {
                characterNameText.gameObject.SetActive(true);
                characterNameText.text = character.characterName;
            }


            hideTimer = defaultTimeBeforeBarHides;
            gameObject.SetActive(true);
        }
    }
}
