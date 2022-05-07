using UnityEngine;

public class MoveCommand : Command
{
    public Vector2 direction;
    public bool undoable { get; set; }

    public Character user { get; set; }

    public MoveCommand(Character client)
    {
        undoable = true;
        user = client;
    }
    
    public MoveCommand(Vector2 moveTarget, Character client)
    {
        undoable = true;
        Vector2 clientPosition = client.GetPosition();
        direction = new Vector2(moveTarget.x - clientPosition.x, moveTarget.y - clientPosition.y);
        user = client;
    }
    
    public void SetMoveTarget(Vector2 moveTarget)
    {
        Vector2 clientPosition = user.GetPosition();
        direction = new Vector2(moveTarget.x - clientPosition.x, moveTarget.y - clientPosition.y);
    }

    public bool Execute() {
        if (user.Move(direction)) {
            return true;
        }
        return false;
    }

    public void Undo()
    {
        user.UndoMove((int)direction.x, (int)direction.y);
    }
}