using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static bool paused;

    static Text narratorText = null;

    static string pauseText;


    Text scoreText = null;

    static Text timerText = null;
    static float timer = 120;

    float oldCameraPosition = 0f;
    float dstBeforeNextWave = 100f;

    bool introductionOver = false;
    bool firstWave = true;

    string[] messageEnemyApproching = { "Enemy coming bot !", "Enemy coming middle !", "Enemy coming top !", "Oups I was wrong" };

    DrawLevels drawLevel;

    Transform cameraPosition;

    private void Start()
    {
        timer = 120;

        drawLevel = GetComponent<DrawLevels>();

        Score.ResetScore();

        cameraPosition = transform.Find("Camera Follow");
        scoreText = transform.Find("Score").GetComponentInChildren<Text>();
        narratorText = transform.Find("Narrator").GetComponentInChildren<Text>();
        timerText = transform.Find("Timer").GetComponentInChildren<Text>();

        oldCameraPosition = cameraPosition.position.x;
        pauseText = "Pause";
        Pause();

        Audio.AudioManager.Play("Intro");
        StartCoroutine(StartingText());
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            End();
        }

        timerText.text = Mathf.RoundToInt(timer).ToString();

        if (dstBeforeNextWave <= 0 && introductionOver)
        {
            SentWave();
            dstBeforeNextWave = 100f;
        } else
        {
            dstBeforeNextWave -= cameraPosition.position.x - oldCameraPosition;
        }

        Score.AddScore(cameraPosition.position.x - oldCameraPosition);
        scoreText.text = "Score : " + Mathf.RoundToInt(Score.GetScore()).ToString();

        oldCameraPosition = cameraPosition.position.x;
    }

    private void SentWave()
    {
        bool lying;

        if(firstWave)
        {
            lying = false;
            firstWave = false;
        } else
        {
            if(timer < 60)
            {
                lying = (Random.Range(1, 3) == 3);
            } else
            {
                lying = Random.Range(1, 4) == 3;
            } 
        }

        narratorText.text = "Enemies are coming from the ";

        int path = Random.Range(0, 3);
        drawLevel.LoadEnemy(path, lying);
        if (path == 0)
        {
            if (timer < 60)
            {
                Audio.AudioManager.Play("RightMad");
            } else
            {
                Audio.AudioManager.Play("Right");
            }

            narratorText.text += "right !";
        } else if (path == 1)
        {
            Audio.AudioManager.Play("Middle");
            narratorText.text += "middle !";
        } else
        {
            if (timer < 60)
            {
                Audio.AudioManager.Play("LeftMad");
            }
            else
            {
                Audio.AudioManager.Play("Left");
            }

            narratorText.text += "left !";
        }
        if (lying)
        {
            Invoke("LyingMessage", 3f);
        } else
        {
            Invoke("ResetNarratorText", 3f);
        }
    }

    private void LyingMessage()
    {
        if (timer < 60)
        {
            Audio.AudioManager.Play("WrongMad");
        }
        else
        {
            Audio.AudioManager.Play("Wrong");
        }

        narratorText.text = "Oupsy, I was wrong";
        Invoke("ResetNarratorText", 1f);
    }

    private void ResetNarratorText()
    {
        narratorText.text = "";
    }

    IEnumerator StartingText()
    {
        string[] message = { "Go as far as possible before the timer ends ! Use ZQSD to move, space to jump, left click to attack (hold) and escape to start !",
                                "Monsters have invaded the realm !", "You have to defeat them ! But sometimes you won't be able to stop them all", "Just do your best, the people counts on you"};
        for (int i = 0; i < message.Length; i++)
        {
            narratorText.text = message[i];

        if (i == message.Length - 1) 
            {
                Invoke("IntroductionOver", 7f);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private void IntroductionOver()
    {
        introductionOver = true;
        ResetNarratorText();
    }

    public static void Pause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            narratorText.text = pauseText;
        } else
        {
            Time.timeScale = 1;
            narratorText.text = "";
        }
    }



    public static void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    private void End()
    {
        SceneManager.LoadScene("EndCinematic");
    }
}
