using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FSM : NetworkBehaviour
{
    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }
    protected virtual void FSMLateUpdate() { }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        FSMUpdate();
    }

    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }

    private void LateUpdate()
    {
        FSMLateUpdate();
    }
}
