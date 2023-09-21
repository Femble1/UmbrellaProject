using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rizarudo : Animated
{
    public CharacterStat stats;
    public int max_hp;
    public int armor;
    public int hp;
    public float movespeed = 0.01f;
    public float shot_cd = 4f;
    public float actual_shot_cd;
    public float safe_distance = 1;
    public float state_wait_time = 0.2f;
    public float sprite_height;
    public float sprite_width;
    float random_movement;
    float actual_movespeed = 0;
    float hspeed;
    float vspeed;
    bool stop;
    bool retreat;
    public bool single_hit;
    private int ground_layer_mask;
    Vector3 hunt_position;
    float hunt_calculated;
    GameObject player;
    public GameObject ring;
    public GameObject damage_display;


    // Start is called before the first frame update
    void Start()
    {
        ground_layer_mask = (1 << 3);
        download_stats();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(random_move_timer());
        actual_shot_cd = Random.Range(0.5f, 5f);
        InvokeRepeating("point_one_sec", 0, 0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        collision();

        move();
    }
    void Update()
    {
        calculate_player_position();

        if (!stop && !retreat)
        {
            hunt_player_state();
        }
        else if (stop && !retreat) stop_state();
        else if (retreat) retreat_state();

        if (single_hit)
        {
            actual_movespeed = 0;
        }
        else actual_movespeed = movespeed;
    }

    void calculate_player_position()
    {
        hunt_position = player.transform.position - transform.position;

        hunt_calculated = Mathf.Sqrt((hunt_position.x * hunt_position.x) + (hunt_position.y * hunt_position.y));    
    }

    void hunt_player_state()
    {
        if (hunt_calculated > safe_distance)
        {
            hspeed = (hunt_position.x * (actual_movespeed/hunt_calculated)) + (random_movement * actual_movespeed);
            vspeed = (hunt_position.y * (actual_movespeed/hunt_calculated)) + (random_movement * actual_movespeed);
        }
        else
        {
            hspeed = 0;
            vspeed = 0;

            StartCoroutine(stop_timer());
        }
    }

    void shot()
    {
        GameObject shot = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/RosaShot"));

        shot.GetComponent<Projectile>().player_position = player.transform.position;
        shot.transform.position = transform.position;
    }

    void stop_state()
    {
        if (hunt_calculated > safe_distance)
        {
            StartCoroutine(start_timer());
        }

        if (hunt_calculated < safe_distance - 0.5f)
        {
            StartCoroutine(retreat_timer());
        }
    }

    void retreat_state()
    {
        if (hunt_calculated > safe_distance)
        {
            StartCoroutine(start_timer());
        }
        else
        {
            hspeed = - (hunt_position.x * (actual_movespeed/hunt_calculated)) + (random_movement * actual_movespeed);
            vspeed = - (hunt_position.y * (actual_movespeed/hunt_calculated)) + (random_movement * actual_movespeed);
        }
    }

    void move()
    {
        transform.position += new Vector3(hspeed, vspeed, 0);
    }

    void collision()
    {
        RaycastHit2D hit_left = Physics2D.Raycast(transform.position, Vector2.left, sprite_width - hspeed, ground_layer_mask);
        RaycastHit2D hit_right = Physics2D.Raycast(transform.position, Vector2.right, sprite_width + hspeed, ground_layer_mask);

        if (hit_right || hit_left)
        {
            hspeed = 0;
        }

        RaycastHit2D hit_ground = Physics2D.Raycast(transform.position, Vector3.down, sprite_height - vspeed, ground_layer_mask);
        RaycastHit2D hit_roof = Physics2D.Raycast(transform.position, Vector3.up, sprite_height + vspeed, ground_layer_mask);

        if (hit_ground || hit_roof)
        {
            vspeed = 0;
        }
    }

    IEnumerator random_move_timer()
    {
        random_movement = Random.Range(-0.6f, 0.6f);

        yield return new WaitForSeconds(2f);

        StartCoroutine(random_move_timer());
    }

    IEnumerator stop_timer()
    {
        yield return new WaitForSeconds(state_wait_time);

        stop = true;

        retreat = false;
    }

    IEnumerator start_timer()
    {
        yield return new WaitForSeconds(state_wait_time);

        stop = false;

        retreat = false;
    }

    IEnumerator retreat_timer()
    {
        yield return new WaitForSeconds(state_wait_time);

        retreat = true;

        stop = false;
    }

    public void hit(Vector3 eff_rotation, float dmg, int combo, Player hitter)
    {
        //Play Animations and Sound
        SpawnAnimation("SimpleEffect", "Sprites/dash_eff_animation", true, new Vector3(transform.position.x, transform.position.y, -1), new Vector3(1,1,1), new Vector3(eff_rotation.x, eff_rotation.y, eff_rotation.z - 45), 1.5f);
        SpawnAnimation("SimpleEffect", "Sprites/dash_eff_animation3", true, new Vector3(transform.position.x,transform.position.y,-1), new Vector3(1,1,1), eff_rotation, 2f);

        int random_hit_sound = Random.Range(1, 100);

        if (random_hit_sound > 50)
        {
            FindObjectOfType<SoundManager>().play("dash_hit1", Random.Range(0.9f, 1.25f));
        }
        else FindObjectOfType<SoundManager>().play("dash_hit2", Random.Range(0.9f, 1.25f));

        GameObject dmg_display = GameObject.Instantiate(damage_display);
        dmg_display.transform.SetParent(transform, false);

        //Receive Damage
        
        float combo_coeficient = 0.75f + (0.25f * combo);

        float damage = (((dmg * combo_coeficient) - armor) * Random.Range(1,1.2f));

        if (eff_rotation.z <= 120 && eff_rotation.z >= 60)
        {
            //Weak-hit
            hp -= Mathf.RoundToInt(damage * 0.5f);
            dmg_display.GetComponentInChildren<DamageDisplay>().display_damage("Weak-Hit", Mathf.RoundToInt(damage * 0.5f));
        }
        else if (eff_rotation.z > 120 && eff_rotation.z < 150 || eff_rotation.z < 60 && eff_rotation.z > 30)
        {
            //Half-Weak-Hit
            hp -= Mathf.RoundToInt(damage * 0.75f);
            dmg_display.GetComponentInChildren<DamageDisplay>().display_damage("Half-Weak-Hit", Mathf.RoundToInt(damage * 0.75f));
        }
        else if (eff_rotation.z > 240 && eff_rotation.z < 300)
        { 
            //Crit-Hit
            hp -= Mathf.RoundToInt(damage * 2f);
            dmg_display.GetComponentInChildren<DamageDisplay>().display_damage("Crit-Hit", Mathf.RoundToInt(damage * 2f));
        }
        else if (eff_rotation.z < 240 && eff_rotation.z > 210 || eff_rotation.z > 300 && eff_rotation.z < 330)
        {
            //Half-Crit-Hit
            hp -= Mathf.RoundToInt(damage * 1.5f);
            dmg_display.GetComponentInChildren<DamageDisplay>().display_damage("Half-Crit-Hit", Mathf.RoundToInt(damage * 1.5f));
        }
        else
        {
            //Normal Hit
            hp -= Mathf.RoundToInt(damage);
            dmg_display.GetComponentInChildren<DamageDisplay>().display_damage("Normal-Hit", Mathf.RoundToInt(damage));
        }

        if (hp <= 0)
        {
            StartCoroutine(destroy_proccess());
        }
    }

    IEnumerator destroy_proccess()
    {
        player.GetComponent<Player>().dash_charges ++;
        RectTransform[] damage_transforms = GetComponentsInChildren<RectTransform>();
        transform.DetachChildren();
        foreach(RectTransform damage_transform in damage_transforms)
        {
            damage_transform.anchoredPosition = new Vector2(transform.position.x, transform.position.y);
        }
        
        yield return new WaitForFixedUpdate();

        Destroy(ring);
        Destroy(gameObject);
    }

    void point_one_sec()
    {
        actual_shot_cd -= 0.1f * (1 + (player.GetComponent<Player>().dash_combo/5));
        if (actual_shot_cd <= 0.5f * (1 + (player.GetComponent<Player>().dash_combo/5)))
        {
            ring.GetComponent<Animator>().SetBool("attack", true);
            ring.transform.up = player.transform.position - ring.transform.position;
            ring.transform.Rotate(new Vector3(0, 0, 180));
        }
        if (actual_shot_cd <= 0.55f * (1 + (player.GetComponent<Player>().dash_combo/5)) && actual_shot_cd >= 0.45f * (1 + (player.GetComponent<Player>().dash_combo/5)))
        {
            FindObjectOfType<SoundManager>().play("rosa_shot");
        }
        if (actual_shot_cd <= 0)
        {
            actual_shot_cd = shot_cd;
            ring.GetComponent<Animator>().SetBool("attack", false);
            ring.transform.rotation = new Quaternion(0, 0, 0, 0);
            shot();
        }
    }

    void download_stats()
    {
        movespeed = stats.movespeed;
        max_hp = stats.max_hp;
        hp = max_hp;
    }
}
