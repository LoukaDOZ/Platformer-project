using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private InputSettings inputs;
    private bool jump = false;
    private bool dash = false;
    private PlayerController controller;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        inputs = new InputSettings();
        inputs.Player.Enable();
        inputs.Player.Jump.performed += ctx => Jump();
        inputs.Player.Dash.performed += ctx => Dash();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Jump()
    {
        if (Time.timeScale == 1)
            jump = true;
    }

    private void Dash()
    {
        dash = true;
    }

    private void FixedUpdate()
    {
        bool sprint = inputs.Player.Sprint.ReadValue<float>() == 1f;
        
        controller.Move(inputs.Player.Move.ReadValue<Vector2>(), jump, sprint, dash);
        
        jump = false;
        dash = false;
        
        bool jumpButton = inputs.Player.Jump.ReadValue<float>() != 0;
        controller.SetJumpButtonValue(jumpButton);
    }
}
