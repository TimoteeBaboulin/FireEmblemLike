using UnityEngine;

namespace DefaultNamespace
{
    public class ExchangePlacesCommand : Command
    {
        public Character user { get; set; }
        public Character otherUser;

        public ExchangePlacesCommand(Character firstUser, Character secondUser)
        {
            user = firstUser;
            otherUser = secondUser;
        }
        public bool Execute()
        {
            Vector2 firstUser = user.GetPosition();
            Vector2 secondUser = otherUser.GetPosition();

            if (!user.ExchangePositions(secondUser))
                return false;
            otherUser.ExchangePositions(firstUser);
            return true;
        }

        public void Undo()
        {
            Vector2 firstUser = user.GetPosition();
            Vector2 secondUser = otherUser.GetPosition();

            user.ExchangePositions(secondUser);
            otherUser.ExchangePositions(firstUser);
        }
    }
}