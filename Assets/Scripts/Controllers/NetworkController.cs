using System;
using models;
using Networking;
using UnityEngine;

namespace Controllers
{
    public class NetworkController : MonoBehaviour
    {
        public Client Client;
        private Game _game;
        private bool Connected = false;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
            Client = new Client(_game);
        }

        public void SendCommand(Command cmd)
        {
            if (!Connected) return;
            Client.SendCommand(cmd);
        }

        public void Connect()
        {
            Client.Connect();
            Connected = true;
        }
    }
}