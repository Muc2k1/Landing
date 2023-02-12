using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Player;
namespace Managers
{
    public enum GameMode
    {
        Undefine = 0,
        Playing = 1,
        Pausing = 2,
    }
    public enum GamePhase
    {
        Normal = 0,
        Bossing = 1,
        Ending = 2,
    }

    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance;

        [SerializeField] private SpawnLine spawnLine;
        private const float NORMAL_SPAWN_COOLDOWN = 2f;
        private float normalSpawnCountDown;
        private GamePhase gamePhase = GamePhase.Normal;

        public GameObject Boss;
        [SerializeField] private GameObject gameOverMenu;

        private PlayerController cachePlayer;

        private void Awake() 
        {
            if (Instance != null) 
            {
                Debug.LogError("We are have 2 GameplayerManager!!!");
                return;
            }
            Instance = this;
        }

        private void Start() 
        {
            Init();
        }
        private void Init()
        {
            normalSpawnCountDown = NORMAL_SPAWN_COOLDOWN;
            cachePlayer = GameInstanceHolder.Instance.Player;
            AllEvents.OnBossingPhase += OnBossingPhase;
            AllEvents.OnPlayerDead += OnPlayerDead;
        }
        private void OnDestroy() 
        {
            AllEvents.OnBossingPhase -= OnBossingPhase;
            AllEvents.OnPlayerDead -= OnPlayerDead;
        }
        private void Update()
        {
            NormalDevilSpawn();
        }
        private void NormalDevilSpawn()
        {
            if (gamePhase != GamePhase.Normal) return;
            if (normalSpawnCountDown >= 0)
            {
                normalSpawnCountDown -= Time.deltaTime;
                if (normalSpawnCountDown < 0)
                {
                    spawnLine.Spawn();
                    ResetNormalSpawnCount();
                }
            }
        }
        private void ResetNormalSpawnCount()
        {
            normalSpawnCountDown = NORMAL_SPAWN_COOLDOWN;
        }
        private void OnBossingPhase(bool bossing)
        {
            if (!bossing)
            {
                gamePhase = GamePhase.Normal;
            }
            else
            {
                gamePhase = GamePhase.Bossing;
                Debug.Log("Spawn boss");
                SpawnBoss();
                ResetNormalSpawnCount();
            }
        }
        private void SpawnBoss()
        {
            Instantiate(Boss, Vector3.zero, Quaternion.identity);
        }
        private void OnPlayerDead()
        {
            //gameOverMenu.SetActive(true);
            gamePhase = GamePhase.Ending;
            AllEvents.OnTimeScale?.Invoke(0.1f, 1f);
            StartCoroutine(TriggerEndGameScene());

        }
        private IEnumerator TriggerEndGameScene()
        {
            yield return new WaitUntil(()=>cachePlayer.IsAlreadyDead);
            gameOverMenu.SetActive(true);
        }
        private IEnumerator TriggerEndGameScene()
        {
            yield return new WaitUntil(()=>cachePlayer.IsAlreadyDead);
            gameOverMenu.SetActive(true);
        }
    }
}
