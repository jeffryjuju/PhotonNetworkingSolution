using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    //float horizontalMove = 0f;
    //float verticalMove = 0f;
    float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
    }

    void playerMovement() // control the player movement
    {

        float posHorizontal = Input.GetAxis("Horizontal") * this.speed * Time.deltaTime;
        float posVertical = Input.GetAxis("Vertical") * this.speed * Time.deltaTime;

        if (posHorizontal > 0)
        {
            transform.Translate(posHorizontal, 0, 0);
        }
        else if (posHorizontal < 0)
        {
            transform.Translate(posHorizontal, 0, 0);
        }
        if (posVertical > 0)
        {
            transform.Translate(0, posVertical, 0);
        }
        else if (posVertical < 0)
        {
            transform.Translate(0, posVertical, 0);
        }
    }
}
