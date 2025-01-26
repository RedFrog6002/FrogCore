using UnityEngine;

namespace FrogCore.Unity
{
    public class UnityExtraDamageable : MonoBehaviour, IExtraDamageable
    {
        private UnitySpriteFlash spriteFlash;

        private bool isSpellVulnerable;

        private HealthManager healthManager;

        [SerializeField]
        private RandomAudioClipTable impactClipTable;

        [SerializeField]
        private AudioSource audioPlayerPrefab;

        private bool damagedThisFrame;

        protected void Awake()
        {
            healthManager = GetComponent<HealthManager>();
            spriteFlash = GetComponent<UnitySpriteFlash>();
            isSpellVulnerable = base.gameObject.CompareTag("Spell Vulnerable");
        }

        private void LateUpdate()
        {
            damagedThisFrame = false;
        }

        public void RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            if (damagedThisFrame)
                return;
            damagedThisFrame = true;
            if (!isSpellVulnerable && (healthManager && healthManager.IsInvincible))
                return;
            impactClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
            if (spriteFlash)
            {
                switch (extraDamageType)
                {
                    case ExtraDamageTypes.Spore:
                        spriteFlash.flashSporeQuick();
                        break;
                    case ExtraDamageTypes.Dung:
                    case ExtraDamageTypes.Dung2:
                        spriteFlash.flashDungQuick();
                        break;
                }
            }
            ApplyExtraDamageToHealthManager(GetDamageOfType(extraDamageType));
        }

        public static int GetDamageOfType(ExtraDamageTypes extraDamageTypes)
        {
            if ((uint)extraDamageTypes <= 1u || extraDamageTypes != ExtraDamageTypes.Dung2)
                return 1;
            return 2;
        }

        private void ApplyExtraDamageToHealthManager(int damageAmount)
        {
            if (healthManager)
                healthManager.ApplyExtraDamage(damageAmount);
        }
    }
}