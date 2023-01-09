using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

namespace Devil
{
    public class BasicBoss : MonoBehaviour
    {
        private const int ATT_DOWN_L = 0;
        private const int ATT_DOWN_R = 1;
        private const int ATT_SIDE_L = 2;
        private const int ATT_SIDE_R = 3;
        [SerializeField] private GameObject attackWave;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        private float attackCountdown;
        private const float ATTACK_COOLDOWN = 10f;
        private Animator anim;
        private const int MAX_ATTACK_STYLE = 4;
        private readonly string[] ATTACK_STYLE_STR = new string[] {
            "LeftHandAttack", "RightHandAttack", "LeftHandSideAttack", "RightHandSideAttack"};
        [SerializeField] private Transform[] spawnAttackPos;

        private Dictionary<string, Transform> attackPosDic;
        [SerializeField] private int cheatBossAttackStyle = -1;
        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();

            attackPosDic = new Dictionary<string, Transform>();
            for (int i = 0; i < ATTACK_STYLE_STR.Length; i ++)
            {
                attackPosDic.Add(ATTACK_STYLE_STR[i], spawnAttackPos[i]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (attackCountdown >= 0)
            {
                attackCountdown -= Time.deltaTime;
                if (attackCountdown <= 0)
                {
                    Attack();
                }
            }
        }
        private void Attack()
        {
            int attackStyle = Random.Range(0, MAX_ATTACK_STYLE);

            if (cheatBossAttackStyle != -1)
                attackStyle = cheatBossAttackStyle;

            anim.Play(ATTACK_STYLE_STR[attackStyle]);
            attackCountdown = ATTACK_COOLDOWN;
        }
        public void SpawnAttackLeftDown()
        {
            SpawnAttackWave(ATTACK_STYLE_STR[ATT_DOWN_L], true);
            AllEvents.OnWorldDeviated?.Invoke(3);
        }
        public void SpawnAttackRightDown()
        {
            SpawnAttackWave(ATTACK_STYLE_STR[ATT_DOWN_R], true);
            AllEvents.OnWorldDeviated?.Invoke(3);
        }
        public void SpawnAttackLeftSide()
        {
            SpawnAttackWave(ATTACK_STYLE_STR[ATT_SIDE_L], false);
            AllEvents.OnWorldDeviated?.Invoke(0);
        }
        public void SpawnAttackRightSide()
        {
            SpawnAttackWave(ATTACK_STYLE_STR[ATT_SIDE_R], false, true);
            AllEvents.OnWorldDeviated?.Invoke(2);
        }
        public void SpawnAttackWave(string attackName, bool isHorizonDir, bool flipY = false)
        {
            float horizonDir = isHorizonDir ? 0f : 90f;

            GameObject wave = Instantiate(attackWave, this.transform, true);
            wave.transform.localPosition = attackPosDic[attackName].localPosition;
            wave.transform.rotation = Quaternion.Euler(0f, 0f, horizonDir);

            if (flipY)
            {
                wave.transform.localScale = new Vector3(1f, -1f, 1f);
            }
        }
    }
}
