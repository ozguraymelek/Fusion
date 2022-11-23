using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    [SerializeField] private Ball prefabBall;
    private Vector3 _forward;
    
    [Networked] private TickTimer Delay { get; set; }
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.Direction.Normalize(); // to prevent cheating!

            _cc.Move(5 * data.Direction * Runner.DeltaTime);

            if (data.Direction.sqrMagnitude > 0)
                _forward = data.Direction;

            if (Delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.Buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    Delay = TickTimer.CreateFromSeconds(Runner, .5f);
                    
                    Runner.Spawn(prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward),
                        Object.InputAuthority, (Runner, o) =>
                        {
                            o.GetComponent<Ball>().Init();
                        });
                }
            }
            
            
        }
    }
    
    
}
