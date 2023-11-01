using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody _rigidbody;

    //Commands
    Command cmd_W = new MoveForwardCommand();
    Command cmd_A = new MoveLeftCommand();
    Command cmd_S = new MoveBackwardCommand();
    Command cmd_D = new MoveRightCommand();

    Command _last_cmd = null;

    Stack<Command> _undo_commands = new Stack<Command>();

    
    void SwapCommands(ref Command A, ref Command B)
    {
        Command tmp = A;
        A = B;
        B = tmp;

        _undo_commands.Push(cmd_W);
        _undo_commands.Pop();
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            cmd_W.Execute(_rigidbody);
            _last_cmd = cmd_W;
            //transform.position += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cmd_A.Execute(_rigidbody);
            _last_cmd = cmd_A;
            //transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cmd_S.Execute(_rigidbody);
            _last_cmd = cmd_S;
            //transform.position += Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cmd_D.Execute(_rigidbody);

            //transform.position += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(10.0f*transform.up , ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwapCommands(ref cmd_A, ref cmd_D);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _last_cmd.Undo(_rigidbody);
            _last_cmd = new DoNothingCommand();
        }
    }
}
