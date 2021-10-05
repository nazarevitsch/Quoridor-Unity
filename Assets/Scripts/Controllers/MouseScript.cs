using models;
using store;
using UnityEngine;

namespace Controllers
{
    public class MouseScript : MonoBehaviour
    {
        private Game _game;
        private UnityObjStore _objStore;
        private bool isPaused;

        private void Awake()
        {
            _objStore = FindObjectOfType<UnityObjStore>();
            _game = FindObjectOfType<Game>();
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
                        
                        var gameObj = hit.collider.gameObject;
                        var cs = gameObj.GetComponent<CoordinateScript>();
                        var point = _game.Points[cs.Y][cs.X];
                        Debug.Log("Click: " + point.Tag);
                        switch (point.Tag)
                        {
                            case "Unblocked":
                                Debug.Log("Mouse 2");
                                _game.PutBlock(point);
                                break;
                            case "PossibleToBlock":
                                Debug.Log("Mouse 1");
                                _game.PutBlock(point);
                                break;
                            case "Possible":
                            {
                                var destination = gameObj.transform.position;
                                _objStore.PlayerObjects[_game.CurrentPlayer.Name].transform.position = destination;
                                _game.DoStep(point);
                                break;
                            }
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
}
