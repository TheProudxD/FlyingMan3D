using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static readonly int Win = Animator.StringToHash("Win");

    [HideInInspector] public bool isPlayerEntered;
    [HideInInspector] public bool isGameStarted;
    public bool CanSmoke = true;

    [SerializeField] private GameObject GameSuccessPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject tapToThrow;
    [SerializeField] private TextMeshProUGUI levelText;
    public Colors[] ColorArray;

    [Serializable]
    public class Colors
    {
        public Color RingColor;
        public Color RingTransColor;
        public Color PlatformColor;
    }

    private int _currentLevel;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
        
        Application.targetFrameRate = 60;
    }
    
    private void Start()
    {
        _currentLevel = PlayerPrefs.GetInt("level", 0);
        levelText.text = "LEVEL " + (_currentLevel + 1);
    }

    private void Update()
    {
        if (Instance.isPlayerEntered)
        {
            if (PlayerController.players.Count == 0 && Spawner.Enemies.Count >= 0)
            {
                foreach (GameObject item in Spawner.Enemies)
                {
                    item.GetComponent<Animator>().SetBool("Win", true);
                    Destroy(item.GetComponent<Rigidbody>());
                }

                GameOver();
                isPlayerEntered = false;
            }
            else if (Spawner.Enemies.Count == 0 && PlayerController.players.Count > 0)
            {
                foreach (PlayerController item in PlayerController.players)
                {
                    item.Animator.SetBool(Win, true);
                    Destroy(item.GetComponent<Rigidbody>());
                }

                GameSuccess();
                isPlayerEntered = false;
            }
        }
    }

    public void CloseTapText()
    {
        tapToThrow.SetActive(false);
    }

    public void GameOver()
    {
        PlayerController.players = null;
        GameOverPanel.SetActive(true);
    }

    public void GameSuccess()
    {
        PlayerController.players = null;
        GameSuccessPanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        _currentLevel += 1;
        PlayerPrefs.SetInt("level", _currentLevel);

        if (_currentLevel % SceneManager.sceneCountInBuildSettings == 0)
        {
            SceneManager.LoadScene(0);
        }
        else
            SceneManager.LoadScene(_currentLevel % SceneManager.sceneCountInBuildSettings + 1);
    }

    public void LoadAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}