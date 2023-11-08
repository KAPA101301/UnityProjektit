using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody _rigidbody;
    Vector3 _start_pos;
    bool bReplaying = false;

    //Commands
    Command cmd_W = new MoveForwardCommand();
    Command cmd_A = new MoveLeftCommand();
    Command cmd_S = new MoveBackwardCommand();
    Command cmd_D = new MoveRightCommand();

    //Command _last_cmd = null;

    Stack<Command> _undo_commands = new Stack<Command>();
    Stack<Command> _redo_commands = new Stack<Command>();
    Stack<Command> _replay_commands = new Stack<Command>();

    

    void SwapCommands(ref Command A, ref Command B, ref Command C, ref Command D)
    {
        Command tmp = A;
        A = B;
        B = tmp;
        Command tmp2 = C;
        C = D;
        D = tmp2;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _start_pos = transform.position;
    }
    IEnumerator Replay()
    {
        //Go through all the replay commands
        while (_replay_commands.Count > 0)
        {
            _replay_commands.Pop().Execute(_rigidbody);
            yield return new WaitForSeconds(.5f);
        }

        bReplaying = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (bReplaying)
        {
            
        }
        else
        {
        if (Input.GetKeyUp(KeyCode.F))
        {
            bReplaying = true;
            //Get the Undo-stack and "reverse" it
            while(_undo_commands.Count > 0)
            {
                _replay_commands.Push(_undo_commands.Pop());    
            }
            //Move the player to the start position
            transform.position = _start_pos;

            //Start coroutine
            StartCoroutine(Replay());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            cmd_W.Execute(_rigidbody);
           // _last_cmd = cmd_W;
            _undo_commands.Push(cmd_W);
            _redo_commands.Clear();
            //transform.position += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            cmd_A.Execute(_rigidbody);
            //_last_cmd = cmd_A;
            _undo_commands.Push(cmd_A);
            _redo_commands.Clear();
            //transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            cmd_S.Execute(_rigidbody);
            //_last_cmd = cmd_S;
            _undo_commands.Push(cmd_S);
            _redo_commands.Clear();
            //transform.position += Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            cmd_D.Execute(_rigidbody);
            //_last_cmd = cmd_D;
            _undo_commands.Push(cmd_D);
            _redo_commands.Clear();
            //transform.position += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(10.0f*transform.up , ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwapCommands(ref cmd_A, ref cmd_D, ref cmd_W, ref cmd_S);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //if there are commands in the stack...
            if(_undo_commands.Count > 0)
            {
                //...pop one command out and execute it.
                Command cmd = _undo_commands.Pop();
                _redo_commands.Push(cmd);
                cmd.Undo(_rigidbody);   
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if(_redo_commands.Count > 0)
            {
                Command cmd = _redo_commands.Pop();
                _undo_commands.Push(cmd);
                cmd.Execute(_rigidbody);
            }    
        }

        }
    }
}
