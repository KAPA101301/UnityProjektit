using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command 
{
    //All movement commands use this speed
    protected float _Speed = 10.0f;
    //EXECUTE MUST BE IMPLEMENTED BY EACH CHILD-CLASS
    public abstract void Execute(Rigidbody rb);

    public abstract void Undo(Rigidbody rb);

    public abstract void Redo(Rigidbody rb);


}
