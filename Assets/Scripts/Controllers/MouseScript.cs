using models;
using Networking;
using store;
using UnityEngine;

namespace Controllers
{
    public class MouseScript : MonoBehaviour
    {
        private Game _game;
        private UnityObjStore _objStore;
        private NetworkController _networkController;
        private bool isPaused;

        private void Awake()
        {
            _objStore = FindObjectOfType<UnityObjStore>();
            _game = FindObjectOfType<Game>();
            _networkController = FindObjectOfType<NetworkController>();
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
                        switch (point.Tag)
                        {
                            case "Unblocked":
                                _game.PutBlock(point);
                                _networkController.SendCommand(new Command
                                {
                                    Code = "PutBlock",
                                    Position = point,
                                    PlayColor = _game.CurrentPlayer.Name == "Player 1" ? PlayColor.White : PlayColor.Black
                                });
                                break;
                            case "PossibleToBlock":
                                _game.PutBlock(point);
                                break;
                            case "Possible":
                            {
                                var destination = gameObj.transform.position;
                                _objStore.PlayerObjects[_game.CurrentPlayer.Name].transform.position = destination;
                                _game.DoStep(point);
                                _networkController.SendCommand(new Command
                                {
                                    Code = "Move",
                                    Position = point,
                                    PlayColor = _game.CurrentPlayer.Name == "Player 1" ? PlayColor.White : PlayColor.Black
                                });
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
