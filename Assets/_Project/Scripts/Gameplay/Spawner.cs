using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    public enum RingType
    {
        Additive,
        Multiplier,
        Reducer
    }

    [Serializable]
    public class Ring
    {
        [FormerlySerializedAs("ringType")] public RingType RingType;
        [FormerlySerializedAs("effect")] public int Effect;
    }

    [Serializable]
    public class UpperRing
    {
        [FormerlySerializedAs("insideRings")] public Ring[] InsideRings = new Ring[2];
    }

    public static Spawner Instance;
    public static List<GameObject> Enemies;

    [FormerlySerializedAs("Platform")] [SerializeField] private GameObject _platform;
    [FormerlySerializedAs("EnemyPrefab")] [SerializeField] private GameObject _enemyPrefab;
    [FormerlySerializedAs("rings")] [SerializeField] private UpperRing[] _rings;
    [FormerlySerializedAs("ringPrefab")] [SerializeField] private GameObject _ringPrefab;
    [FormerlySerializedAs("playerTransform")] [SerializeField] private Transform _playerTransform;
    [FormerlySerializedAs("finish")] [SerializeField] private GameObject _finish;
    [FormerlySerializedAs("enemyCount")] [SerializeField] private float _enemyCount;
    [FormerlySerializedAs("timeDif")] [SerializeField] private float _timeDif = 0.5f;

    private readonly float _g = -Physics.gravity.y;
    private float _averageTime;
    private float _velY;
    private float _velZ;
    private float _xPos;
    private float _yPos;
    private float _zPos;

    private int _ringCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Enemies = new List<GameObject>();
    }

    private void Start()
    {
        _platform.GetComponent<Renderer>().sharedMaterial.color =
            GameManager.Instance.ColorArray[SceneManager.GetActiveScene().buildIndex].PlatformColor;

        _ringPrefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial.color =
            GameManager.Instance.ColorArray[SceneManager.GetActiveScene().buildIndex].RingColor;

        _ringPrefab.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Renderer>().sharedMaterial
            .color = GameManager.Instance.ColorArray[SceneManager.GetActiveScene().buildIndex].RingTransColor;

        _ringPrefab.transform.GetChild(1).gameObject.GetComponent<Renderer>().sharedMaterial.color =
            GameManager.Instance.ColorArray[SceneManager.GetActiveScene().buildIndex].RingColor;

        _ringPrefab.transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<Renderer>().sharedMaterial
            .color = GameManager.Instance.ColorArray[SceneManager.GetActiveScene().buildIndex].RingTransColor;
        
        _ringCount = _rings.Length;
    }

    public void SpawnObjects(Vector3 velocity)
    {
        _velY = velocity.y;
        _velZ = velocity.z;
        _averageTime = _velY / _g;
        _yPos = _playerTransform.position.y + (_velY * _averageTime) - (0.5f * _g * _averageTime * _averageTime);
        _zPos = _playerTransform.position.z + _velZ * _averageTime;

        float finishTime = _averageTime * 2;
        float finishZPos = _playerTransform.position.z + _velZ * finishTime;
        GameObject finishGo = Instantiate(_finish, new Vector3(0, 0, finishZPos), Quaternion.identity);

        float angle = 360 / _enemyCount;

        for (int i = 0; i < _enemyCount; i++)
        {
            float rotation = angle * i;
            GameObject enemy = Instantiate(_enemyPrefab, finishGo.transform.position, Quaternion.Euler(0f, 180f, 0f));
            enemy.transform.Rotate(0, rotation, 0);
            enemy.transform.Translate(new Vector3(0, 0, -16f));
            Enemies.Add(enemy);
        }

        float t;

        if (_ringCount % 2 == 0)
        {
            t = _averageTime - _timeDif / 2 - _timeDif * (_ringCount - 2) / 2;
        }
        else
        {
            t = _averageTime - _timeDif * (_ringCount / 2);
        }

        for (int i = 0; i < _ringCount; i++)
        {
            GameObject currentRing = Instantiate(_ringPrefab, CalculateRingPosition(t, i), Quaternion.identity);

            for (int j = 0; j < 2; j++)
            {
                Ring currentInsideRing = _rings[i].InsideRings[j];
                GameObject currentGo = currentRing.transform.GetChild(j).gameObject;

                DetermineRingType(currentInsideRing, currentGo);
            }

            t += _timeDif;
        }
    }

    private void DetermineRingType(Ring insideRing, GameObject currentChildGo)
    {
        switch (insideRing.RingType)
        {
            case RingType.Additive:
                currentChildGo.AddComponent<AdditiveRing>();
                currentChildGo.GetComponent<AdditiveRing>().addition = insideRing.Effect;
                currentChildGo.GetComponentInChildren<TextMeshPro>().text = "+" + insideRing.Effect;
                break;
            case RingType.Multiplier:
                currentChildGo.AddComponent<MultiplierRing>();
                currentChildGo.GetComponent<MultiplierRing>().multiplier = insideRing.Effect;
                currentChildGo.GetComponentInChildren<TextMeshPro>().text = "x" + insideRing.Effect;
                break;
            case RingType.Reducer:
                currentChildGo.AddComponent<ReducerRing>();
                currentChildGo.GetComponent<ReducerRing>().ReductionFactor = insideRing.Effect;
                currentChildGo.GetComponentInChildren<TextMeshPro>().text = "-" + insideRing.Effect;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector3 CalculateRingPosition(float t, int i)
    {
        _xPos = i == 0 ? Random.Range(-10f, 10f) : Mathf.Clamp(_xPos + 2f, -20f, 20f);

        _yPos = _playerTransform.position.y + (_velY * t) - (0.5f * _g * t * t);
        _zPos = _playerTransform.position.z + _velZ * t;

        return new Vector3(_xPos, _yPos, _zPos);
    }
}