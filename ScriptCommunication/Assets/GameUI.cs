using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject playerObject = GameObject.Find("Player");
        //GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); //IF TAG IS GIVEN
        //player = playerObject.GetComponent<Player>();

        //OR
        player = FindObjectOfType<Player>();

        player.OnPlayerDeath += GameOver; //Subcribing the GameOver method to OnPlayerDeath event
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawHealthBar(player.health);
    }

    void DrawHealthBar(float playerHealth)
    {
        //Implementation
    }

    void GameOver()
    {
        //Implementation
    }
}
