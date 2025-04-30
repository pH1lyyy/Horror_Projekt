using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public Transform playerCam;
    public Transform player;
    public float MouseX;
    public float MouseY;
    public float Sens;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseX += Input.GetAxis("Mouse X") * Sens * Time.deltaTime;
        MouseY += Input.GetAxis("Mouse Y") * Sens * Time.deltaTime;

        MouseY = Mathf.Clamp(MouseY, -90f, 90f);

        player.rotation = Quaternion.Euler(0, MouseX, 0);
        playerCam.rotation = Quaternion.Euler(-MouseY,MouseX,0);
    }
}
