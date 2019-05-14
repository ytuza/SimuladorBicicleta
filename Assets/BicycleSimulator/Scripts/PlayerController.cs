using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO.Ports; //incluimos el namespace Sustem.IO.Ports


[NetworkSettings(channel =0,sendInterval =0.0f)]
public class PlayerController : NetworkBehaviour
{
    public float speed;
    public float angle;
    public float distance;
    public GameObject fire;
    public GameObject mando;
    //public GameObject hudText;

    private Rigidbody rb;
    //private ArduinoIO io;
    //private Text txt;
    private AudioSource bike;

    SerialPort serialPort = new SerialPort("COM4", 9600); //Inicializamos el puerto serie
    public int x = 1;
    public int freno = 0;
    public int velocidad = 0;
    public int var_freno=0;
  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -98F, 0);
        bike = GetComponent<AudioSource>();

        serialPort.Open(); //Abrimos una nueva conexión de puerto serie
        serialPort.ReadTimeout = 25; //Establecemos el tiempo de espera cuando una operación de lectura no finaliza

        if (!isServer)
        {
            CmdDestroy();
        }
    }

    bool verificar(String[] vec)
    {
        String a = vec[0];
        for (int i = 0; i < vec.Length - 2; i++)
        {
            if (a != vec[i])
            {
                return true;
            }
        }
        return false;
    }


    void FixedUpdate()
    {
        
        if (!isLocalPlayer)  //isLocalPlayer
        {
            return;
        }
        CmdDoMove(serialPort.IsOpen);
    }

    [Command]
    void CmdDestroy()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    void CmdDoMove(bool opend_)
    {
        if (opend_) //comprobamos que el puerto esta abierto
        {
            int seguir = velocidad;
            string value = serialPort.ReadLine(); //leemos una linea del puerto serie y la almacenamos en un string
            string[] vec6 = value.Split(','); //Separamos el String leido valiendonos
            print(value);
            var_freno = freno;
            velocidad = int.Parse(vec6[vec6.Length - 1]);
            freno = int.Parse(vec6[0]);
            bool frenar = false;
            if (freno < var_freno)
            {
                frenar = true;
            }
            else if (freno > var_freno) {
                frenar = false;
            }
           
            if (frenar==false && velocidad == 0 && seguir > 30) {
                velocidad = seguir;
            }

            print("velocidad: " + velocidad);
            

            try //utilizamos el bloque try/catch para detectar una posible excepción.
            {
                
                if (velocidad != 0 && freno == var_freno)
                {
                    velocidad = velocidad / 30;
                    speed = velocidad / 2; //2
                }
                else if (frenar || velocidad == 0)
                {
                    speed = 0;
                    frenar = false;
                }
            }

            catch
            {

            }

            Vector3 mousepos = Input.mousePosition;
            if (mousepos.x < 650 && mousepos.x > 600)
            {
                angle = -10;

            }
            else if (mousepos.x < 600 && mousepos.x > 550)
            {
                angle = -20;
            }
            else if (mousepos.x < 550)
            {
                angle = -30;
            }
            else if (mousepos.x > 650 && mousepos.x < 710)
            {
                angle = 0;
            }
            else if (mousepos.x > 710 && mousepos.x < 760)
            {
                angle = 10;
            }
            else if (mousepos.x > 760 && mousepos.x < 810)
            {
                angle = 20;
            }
            else if (mousepos.x > 810)
            {
                angle = 30;
            }

            Vector3 movement = rb.transform.forward * Mathf.Min(speed, 5.0f) * 3.5f;
            Vector3 rotate = new Vector3(0.0f, Mathf.Pow(speed, 0.5f) * Mathf.Sin(Mathf.Deg2Rad * angle) * 0.8f, 0.0f);

            rb.velocity = movement;
            distance += Mathf.Min(speed, 5.0f) / 34;
            rb.transform.Rotate(rotate);
            //mando.transform.Rotate(rotate);

            if (speed <= 0)
            {
                bike.mute = true;
            }
            else
            {
                bike.mute = false;
            }
        }
    }
}