using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player, win;
    private Vector3 camFollow;
    public Camera cam;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>().transform;
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            cam.backgroundColor = Random.ColorHSV(0, 1, 0.5f, 1, 1, 1);
        }

        if (win == null)
            win = GameObject.Find("win(Clone)").GetComponent<Transform>();

        if (transform.position.y > player.transform.position.y && transform.position.y > win.transform.position.y + 4)
            camFollow = new Vector3(transform.position.x, player.position.y, transform.position.z);

        transform.position = new Vector3(transform.position.x, camFollow.y, -3);
    }
}
