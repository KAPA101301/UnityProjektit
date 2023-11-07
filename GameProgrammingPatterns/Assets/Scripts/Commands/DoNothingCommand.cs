using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingCommand : Command
{
    public override void Execute(Rigidbody rb)
    {
        throw new System.NotImplementedException();
    }

    public override void Undo(Rigidbody rb)
    {
        throw new System.NotImplementedException();
    }

    public override void Redo(Rigidbody rb)
    {
        throw new System.NotImplementedException();
    }
}
