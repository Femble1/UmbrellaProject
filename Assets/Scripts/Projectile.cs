using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Animated
{
    public float hspeed;
    public float vspeed;
    public float movespeed;
    public Vector3 player_position;
    Vector3 hunt_position;
    float hunt_calculated;
    // Start is called before the first frame update
    void Start()
    {
        transform.up = player_position - transform.position;
        transform.Rotate(new Vector3(0, 0, 180));
        
        state_target();
    }

    void FixedUpdate()
    {
        move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void move()
    {
        transform.position += new Vector3(hspeed, vspeed);
    }

    void state_target()
    {
        hunt_position = player_position - transform.position;

        hunt_calculated = Mathf.Sqrt((hunt_position.x * hunt_position.x) + (hunt_position.y * hunt_position.y));

        hspeed = (hunt_position.x * (movespeed/hunt_calculated));
        vspeed = (hunt_position.y * (movespeed/hunt_calculated));

        transform.position += new Vector3(hunt_position.x * (1/hunt_calculated), hunt_position.y * (1/hunt_calculated));
    }

    public void hit(Vector3 eff_rotation, Player hitter)
    {
        SpawnAnimation("SimpleEffect", "Sprites/dash_eff_animation", true, new Vector3(transform.position.x, transform.position.y, -1), new Vector3(1,1,1), new Vector3(eff_rotation.x, eff_rotation.y, eff_rotation.z - 45), 1.5f);
        SpawnAnimation("SimpleEffect", "Sprites/dash_eff_animation3", true, new Vector3(transform.position.x,transform.position.y,-1), new Vector3(1,1,1), eff_rotation, 2f);

        transform.DetachChildren();

        Destroy(gameObject);
    }

    public void hit_player()
    {
        GetComponent<Animator>().SetBool("hit", true);

        Destroy(gameObject, Resources.Load<AnimationClip>("Sprites/Enemies/Rosa/Animations/RosaShotEnd").length/GetComponent<Animator>().speed);
    }
}
