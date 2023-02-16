using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Managers;
using Player;
using DG.Tweening;

namespace Devil
{
    public class DevilCube : MonoBehaviour
    {
        [SerializeField] private GameObject deadEffect;
        [SerializeField] private Transform display;
        private const float MIN_SPEED = 0.25f;
        private const float MAX_SPEED = 1f;
        private const float DODGE_DISTANCE = 0.3f;
        private const float DODGE_COOLDOWN = 1.5f;

        private bool isMutant = false;
        int randomDir = 0;
        private float dodgeCountDown = 0;
        private float dodgeTime = 1f;

        private float speed;
        public void Init()
        {
            isMutant = UnityEngine.Random.Range(0, 10) < 1 ? true : false;
            speed = UnityEngine.Random.Range(MIN_SPEED, MAX_SPEED);
        }
        void Start()
        {
            Init();
            AllEvents.OnPlayerAlreadyDead += Explode;
            AllEvents.OnBossingPhase += Explode;
            AllEvents.OnDevilReachBarrier += Explode;
            AllEvents.OnPlayerStartLanding += Dodge;
        }
        void OnDestroy()
        {
            AllEvents.OnPlayerAlreadyDead -= Explode;
            AllEvents.OnDevilReachBarrier -= Explode;
            AllEvents.OnBossingPhase -= Explode;
            AllEvents.OnPlayerStartLanding -= Dodge;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector2.up);
            if (dodgeCountDown >= 0) dodgeCountDown -= Time.deltaTime;
        }
        private void Deactive()
        {
            this.gameObject.SetActive(false);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Feet") && GameInstanceHolder.Instance.Player.MoveState == MoveState.Landing)
            {
                AllEvents.OnDevilDead?.Invoke();
                Explode();
            }
        }
        private void Explode()
        {
            Instantiate(deadEffect, transform.position, Quaternion.identity);
            Deactive();
        }
        private void Explode(bool onBossing)
        {
            Explode();
        }
        private void Dodge()
        {
            if (!isMutant) return;
            if (dodgeCountDown > 0) return;
            dodgeCountDown = DODGE_COOLDOWN;
            randomDir = UnityEngine.Random.Range(0,2) < 1 ? -1 : 1;
            transform.position += (Vector3.right * randomDir * DODGE_DISTANCE);
            display.DOPunchScale(new Vector3(0.1f,0.1f, 0.1f), 0.5f, 3);
            StartCoroutine(Back());
        }
        private IEnumerator Back()
        {
            yield return new WaitForSeconds(dodgeTime);
            transform.position -= (Vector3.right * randomDir * DODGE_DISTANCE);
            display.DOPunchScale(new Vector3(0.1f,0.1f, 0.1f), 0.5f, 3);
        }
    }
}

