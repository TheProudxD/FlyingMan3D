using System;
using System.Collections;
using _Project.Scripts.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using Reflex.Attributes;

public class Spawner : MonoBehaviour, IInitializable
{
    [Inject] private GameFactory _gameFactory;
    [Inject] private AssetProvider _assetProvider;
    [Inject] private LevelResourceService _levelResourceService;

    [SerializeField] private Colors[] _colorArray;

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

    public IEnumerator Initialize()
    {
        int level = _levelResourceService.Current.Value;
        _index = level % _colorArray.Length;

        _gameFactory.GetPlatform().GetComponent<Renderer>().sharedMaterial.color =
            _colorArray[_index].PlatformColor;

        _playerTransform = _gameFactory.GetPlayer().transform;

        yield break;
    }

    public void SpawnObjects(Vector3 velocity)
    {
        _velY = velocity.y;
        _velZ = velocity.z;
        _averageTime = _velY / _g;
        _yPos = _playerTransform.position.y + _velY * _averageTime - 0.5f * _g * _averageTime * _averageTime;
        _zPos = _playerTransform.position.z + _velZ * _averageTime;

        float finishTime = _averageTime * 2;
        float finishZPos = _playerTransform.position.z + _velZ * finishTime;
        Finish finishGo = _assetProvider.GetFinish(new Vector3(0, 0, finishZPos));

        int enemyCount = _gameFactory.GetCurrentLevel().EnemyCount;

        float angle = 360f / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            float rotation = angle * i;
            EnemyFinish enemy = _assetProvider.GetEnemy(finishGo.transform.position, Quaternion.Euler(0f, 180f, 0f));
            enemy.transform.Rotate(0, rotation, 0);
            enemy.transform.Translate(new Vector3(0, 0, -16f));
            _gameFactory.Enemies.Add(enemy);
        }

        int ringCount = _gameFactory.GetCurrentLevel().Rings.Length;
        float timeDif = _gameFactory.GetCurrentLevel().TimeDif;

        float t = ringCount % 2 == 0
            ? _averageTime - timeDif / 2 - timeDif * (ringCount - 2) / 2
            : _averageTime - timeDif * (ringCount / 2f);

        for (int i = 0; i < ringCount; i++)
        {
            RingHolder currentRingHolder = _assetProvider.GetRing(CalculateRingPosition(t, i), _colorArray, _index);

            for (int j = 0; j < currentRingHolder.transform.childCount; j++)
            {
                RingData ringData = _gameFactory.GetCurrentLevel().Rings[i].InsideRings[j];
                GameObject currentGo = currentRingHolder.transform.GetChild(j).gameObject;

                DetermineRingType(ringData, currentGo);
            }

            t += timeDif;
        }
    }

    private void DetermineRingType(RingData ringData, GameObject currentChildGo)
    {
        switch (ringData.RingType)
        {
            case RingType.Additive:
                var a = currentChildGo.AddComponent<AdditiveRing>();
                a.Addition = ringData.Effect;
                a.Text.SetText("+" + ringData.Effect);
                break;
            case RingType.Multiplier:
                var m = currentChildGo.AddComponent<MultiplierRing>();
                m.Multiplier = ringData.Effect;
                m.Text.SetText("x" + ringData.Effect);
                break;
            case RingType.Reducer:
                var r = currentChildGo.AddComponent<ReducerRing>();
                r.ReductionFactor = ringData.Effect;
                r.Text.SetText("-" + ringData.Effect);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector3 CalculateRingPosition(float t, int i)
    {
        _xPos = i == 0 ? Random.Range(-10f, 10f) : Mathf.Clamp(_xPos + 2f, -20f, 20f);

        _yPos = _playerTransform.position.y + _velY * t - 0.5f * _g * t * t;
        _zPos = _playerTransform.position.z + _velZ * t;

        return new Vector3(_xPos, _yPos, _zPos);
    }
}