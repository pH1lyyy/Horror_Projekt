using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform player;
    public float speed;
    public float x;
    public float z;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        player.Translate(x * speed * Time.deltaTime, 0, z * speed * Time.deltaTime);
    }
}
