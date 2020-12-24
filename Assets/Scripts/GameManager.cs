using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public float levelStartDelay = 2f;                        //Time to wait before starting level, in seconds.
   public float turnDelay = 0.1f;                            //Delay between each Player turn.
   public int playerFoodPoints = 100;                        //Starting value for Player food points.
   public static GameManager instance = null;                //Static instance of GameManager which allows it to be accessed by any other script.
   [HideInInspector] public bool playersTurn = true;        //Boolean to check if it's players turn, hidden in inspector but public.

   private Text levelText;
   private GameObject levelImage;
   private GameObject restartButton;
   private GameObject quitButton;
   
   private BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.
   private int level = 0;                                    //Current level number, expressed in game as "Day 1".
   private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
   private bool enemiesMoving;                                //Boolean to check if enemies are moving.
   private bool doingSetup = true;

   // Start is called before the first frame update
   void Awake()
   {
		if (instance == null)
		{
         instance = this;
		}
		else if (instance != this)
		{
         Destroy(gameObject);
		}

      DontDestroyOnLoad(gameObject);

      enemies = new List<Enemy>();

      boardScript = GetComponent<BoardManager>();

      InitGame();

   }
	private void Start()
	{
		
	}

	private void OnLevelWasLoaded(int index)
	{
		if (index == 1)
		{
         level++;

         InitGame();
      }
	}

	void InitGame()
	{
      

      doingSetup = true;

      levelImage = GameObject.Find("LevelImage");
      restartButton = GameObject.Find("RestartButton");
      quitButton = GameObject.Find("QuitButton");

      levelText = GameObject.Find("LevelText").GetComponent<Text>();

      levelText.text = "Day " + level;

      levelImage.SetActive(true);
      restartButton.SetActive(false);
      quitButton.SetActive(false);
      

      Invoke("HideLevelImage", levelStartDelay);

      enemies.Clear();
      boardScript.SetupScene(level);
	} 

   void HideLevelImage()
	{
      levelImage.SetActive(false);

      doingSetup = false;
	}
    // Update is called once per frame
    void Update()
    {
		if (playersTurn || enemiesMoving || doingSetup)
		{
         return;
		}
      StartCoroutine(MoveEnemies());
    }

   public void AddEnemyToList(Enemy script)
	{
      enemies.Add(script);
	}

   public void GameOver()
   {
      levelText.text = "After " + level + " days, you starved.";
      levelImage.SetActive(true);
      restartButton.SetActive(true);
      quitButton.SetActive(true);

      Destroy(instance);
      Destroy(SoundManager.instance);
      Destroy(boardScript);
      enemies.Clear();
      
      
   }

   

   IEnumerator MoveEnemies()
	{
      enemiesMoving = true;

      yield return new WaitForSeconds(turnDelay);

		if (enemies.Count == 0)
		{
         yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++)
		{
         enemies[i].MoveEnemy();
         yield return new WaitForSeconds(enemies[i].moveTime);
		}

      playersTurn = true;

      enemiesMoving = false;
	}
}
