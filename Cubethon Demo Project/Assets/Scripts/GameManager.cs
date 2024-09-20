using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    public GameObject completeLevelUI;
    bool instantReplay = false;
    GameObject player;
    float replayStartTime;

    private void OnEnable()
    {
        PlayerCollision.OnHitObstacle += EndGame;
    }

    private void OnDisable()
    {
        PlayerCollision.OnHitObstacle -= EndGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        player = playerMovement.gameObject;

        if (CommandLog.commands.Count > 0)
        {
            instantReplay = true;
            replayStartTime = Time.timeSinceLevelLoad;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (instantReplay)
        {
            RunInstantReplay();
        }
    }

    public void CompleteLevel()
    {
        completeLevelUI.SetActive(true);
    }

    public void EndGame(Collision collisionInfo)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        PlayerCollision.OnHitObstacle -= EndGame;

        if (collisionInfo != null)
        {
            Debug.Log("Hit: " + collisionInfo.collider.name);
        }

        // this flag prevents responding to multiple hit events:
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Invoke("Restart", 2f);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void RunInstantReplay()
    {
        if (CommandLog.commands.Count == 0)
        {
            return;
        }

        Command command = CommandLog.commands.Peek();
        if (Time.timeSinceLevelLoad >= command.timestamp) // + replayStartTime)
        {
            command = CommandLog.commands.Dequeue();
            command._player = player.GetComponent<Rigidbody>();
            Invoker invoker = new Invoker();
            invoker.disableLog = true;
            invoker.SetCommand(command);
            invoker.ExecuteCommand();
        }
    }
}
