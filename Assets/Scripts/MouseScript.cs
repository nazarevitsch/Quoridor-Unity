using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    private Game Game;
    private bool isPaused;

    private void Awake()
    {
        Game = GameObject.FindObjectOfType<Game>();
    }

    void Update()
    {
        if (!isPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.CompareTag("Unblocked") ||
                        hit.collider.gameObject.CompareTag("PossibleToBlock"))
                    {
                        Game.PutBlock(hit.collider.gameObject);
                    }

                    if (hit.collider.gameObject.CompareTag("Possible"))
                    {
                        Game.DoStep(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    public void ChangePauseState()
    {
        isPaused = !isPaused;
    }
    
    public void ChangePauseStateToFalse()
    {
        isPaused = false;
    }
}
