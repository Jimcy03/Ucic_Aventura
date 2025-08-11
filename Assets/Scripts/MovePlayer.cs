using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour

{
    public float velocidad = 5f;
    public Rigidbody2D rg;
    public Vector2 entrada;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rg.linearVelocity = entrada * velocidad;
    }

    public void Moverse(InputAction.CallbackContext contexto)
    {
        animator.SetBool("estaCaminando", true);

        Vector2 valorEntrada = contexto.ReadValue<Vector2>();

        // Determinar el eje dominante
        if (Mathf.Abs(valorEntrada.x) > Mathf.Abs(valorEntrada.y))
        {
            // Movimiento horizontal
            entrada = new Vector2(Mathf.Sign(valorEntrada.x), 0);
        }
        else if (Mathf.Abs(valorEntrada.y) > 0)
        {
            // Movimiento vertical
            entrada = new Vector2(0, Mathf.Sign(valorEntrada.y));
        }
        else
        {
            entrada = Vector2.zero;

        }

        animator.SetFloat("entradaX", entrada.x);
        animator.SetFloat("entradaY", entrada.y);

        if (contexto.canceled)
        {
            animator.SetBool("estaCaminando", false);

        }

    }

}