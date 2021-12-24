using System;
using System.Collections.Generic;

namespace AStar.networking.Session
{
    public class SessionManager
    {
        public List<GameSession> Sessions = new();

        public GameSession CreateSession(string name)
        {
            var session = new GameSession(name);
            Sessions.Add(session);
            return session; 
        }

        public void AddPlayerToSession(GameSession session, Player player)
        {
            session.AddPlayer(player);
        }

        public GameSession FindSessionByName(string name)
        {
            return Sessions.Find(s => s.Name == name);
        }
        
        public GameSession FindSessionByGuid(Guid guid)
        {
            return Sessions.Find(s => s.Guid == guid);
        }
    }
}