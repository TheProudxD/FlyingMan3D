using System;
using System.Collections;
using _Project.Scripts.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Resources;
using Reflex.Attributes;

public class Spawner : MonoBehaviour
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AssetProvider _assetProvider;
    [Inject] private LevelResourceService _levelResourceService;

    [SerializeField] private Colors[] _colorArray;
    [field: SerializeField] public ParticleSystem WinParticle { get; private set; }

    [Serializable]
    public class Colors
    {
        public Color RingColor;
        public Color RingTransColor;
        public Color PlatformColor;
    }

    private readonly float _g = -Physics.gravity.y;
    private Transform _playerTransform;
    private float _averageTime;
    private float _velY;
    private float _velZ;
    private float _xPos;
    private float _yPos;
    private float _zPos;
    private int _index;

    public void Initialize()
    {
        int level = _levelResourceService.Current.Value;
        _index = level % _colorArray.Length;

        _gameFactory.GetPlatform().GetComponent<Renderer>().sharedMaterial.color = _colorArray[_index].PlatformColor;

        _playerTransform = _gameFactory.GetMainPlayer().transform;
    }

    public void SpawnObjects(Vector3 velocity)
    {
        _velY = velocity.y;
        _velZ = velocity.z;
        _averageTime = _velY / _g;
        _yPos = _playerTransform.position.y + _velY * _averageTime - 0.5f * _g * _averageTime * _averageTime;
        _zPos = _playerTransform.position.z + _velZ * _averageTime;

        float finishTime = _averageTime * 2;
        float finishZPos = _playerTransform.position.z + (_velZ + Random.Range(2, 5)) * finishTime;
        Finish finishGo = _gameFactory.CreateFinish(new Vector3(0, -0.5f, finishZPos), Quaternion.identity);

        Level level = _gameFactory.GetCurrentLevel();
        int enemyCount = level.EnemyCount;

        float angle = 360f / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            float rotation = angle * i;

            _gameFactory.AddEnemy(finishGo.transform.position, rotation);
        }

        int ringCount = level.Rings.Length;
        float timeDif = level.TimeDif;

        float time = ringCount % 2 == 0
            ? _averageTime - timeDif / 2 - timeDif * (ringCount - 2) / 2
            : _averageTime - timeDif * (ringCount / 2f);

        for (int i = 0; i < ringCount; i++)
        {
            for (int j = 0; j < level.Rings[i].InsideRings.Length; j++)
            {
                RingHolder ring = _gameFactory.GetRing(CalculateRingPosition(time, i, j), _colorArray, _index);

                RingData ringData = level.Rings[i].InsideRings[j];
                _assetProvider.GetRingByType(ringData, ring.transform.GetChild(0).gameObject);
            }

            time += timeDif;
        }
    }

    private Vector3 CalculateRingPosition(float t, int i, int j)
    {
        float range = 20;
        float ringDistance = 12f;

        if (j == 0)
            _xPos = Random.Range(-range, range);
        else
            _xPos += ringDistance;

        _yPos = _playerTransform.position.y + _velY * t - 0.5f * _g * t * t;
        _zPos = _playerTransform.position.z + _velZ * t;

        return new Vector3(_xPos, _yPos, _zPos);
    }
}