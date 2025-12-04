using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float suavizadoRotacion = 10f;

    private Rigidbody rb;
    private Vector3 direccionMovimiento;

    private UdpClient udpClient;
    private Thread hiloUDP;
    private string direccion = "neutral";
    private bool hiloActivo = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        udpClient = new UdpClient(5005); // python port
        hiloUDP = new Thread(RecibirUDP);
        hiloUDP.IsBackground = true;
        hiloUDP.Start();
    }

    void RecibirUDP()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (hiloActivo)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                direccion = Encoding.UTF8.GetString(data).Trim();
            }
            catch { }
        }
    }

    void Update()
    {
        Vector3 input = Vector3.zero;
        if (direccion == "arriba") input.z -= 1;
        else if (direccion == "abajo") input.z += 1;
        else if (direccion == "izquierda") input.x += 1;
        else if (direccion == "derecha") input.x -= 1;

        direccionMovimiento = input.normalized;
    }

    void FixedUpdate()
    {
        if (direccionMovimiento.magnitude >= 0.1f)
        {
            Vector3 desplazamiento = direccionMovimiento * velocidad * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + desplazamiento);

            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            rb.rotation = Quaternion.Lerp(rb.rotation, rotacionObjetivo, suavizadoRotacion * Time.fixedDeltaTime);
        }
    }

    void OnApplicationQuit()
    {
        hiloActivo = false;
        udpClient.Close();
    }
}
