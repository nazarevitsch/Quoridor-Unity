using models;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    private Game Game;
    private bool isPaused;

    private void Awake()
    {
        Game = FindObjectOfType<Game>();
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
                        var gameObj = hit.collider.gameObject;
                        Vector3 destination = gameObj.transform.position;
                        Game.CurrentPlayer.player.transform.position = destination;
                        var cs = gameObj.gameObject.GetComponent<CoordinateScript>();
                        Game.DoStep(new Point(null, cs.X, cs.Y));
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
