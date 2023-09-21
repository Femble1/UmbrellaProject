using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Animated
{
    public CharacterStat stats;
    public int max_hp;
    public int hp;
    public float attack_damage;
    public float gravity = -0.0001f;
    public float jump_force = 0.1f;
    public float umbrella_fallspeed = 0.02f;
    public float move_speed = 0.1f;
    public float accel_force = 0.001f;
    public float accel_limit = 0.02f;
    public int dash_charges;
    public int dash_charge_total = 3;
    public int dash_combo = 0;
    public float dash_combo_time = 1;
    public float dash_combo_actual_time = 0;
    public float dash_cd = 4;
    public float dash_actual_cd = 0;
    public float dash_duration = 0f;
    public float dash_speed = 1f;
    public float dash_max_distance = 2f;
    public float dash_init_delay = 0.1f;
    public float dash_end_delay = 0.1f;
    public float dash_buffer_time = 1f;
    public float jump_buffer_time = 1f;
    public float sprite_height = 0.1f;
    public float sprite_width = 0;
    public float dashh_accel = 0;
    public float dashy_accel = 0;
    public float h_accel = 0;
    public float knockback_time = 0;
    float clamped_accel = 0;
    public float vspeed = 0;
    public float hspeed = 0;
    public bool on_ground = false;
    public bool dash_check = false;
    public bool dash_clicked = false;
    public bool enemy_hit;
    public bool knockback;
    bool invulnerability;
    bool direction_changed;
    bool fix_vposition;
    float dash_calculated;
    float jump_buffer;
    float dash_buffer;
    Transform last_combo_enemy;
    Vector2 last_unbugged_pos;
    private int ground_layer_mask;
    Animator player_animator;
    Rigidbody2D rigid_body;
    // Start is called before the first frame update
    void Start()
    {
        download_stats();
        ground_layer_mask = (1 << 3);
        rigid_body = GetComponent<Rigidbody2D>();
        player_animator = GetComponent<Animator>();
        InvokeRepeating("point_one_sec", 0, 0.1f);
        dash_charges = dash_charge_total;
        dash_actual_cd = dash_cd;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp > 0)
        {
            if (!knockback)
            {
                if (!dash_clicked)
                {
                    walking();
                    falling();
                    jump();
                }
                dash();
            }
            else 
            {
                falling();
            }
        }
    }

    void FixedUpdate()
    {
        dash_move();

        collision();

        move();

        dash_buffer -= 0.2f;
        if (dash_buffer <= 0) dash_buffer = 0;

        jump_buffer -= 0.2f;
        if (jump_buffer <= 0) jump_buffer = 0;
    }

    void falling()
    {
        if (on_ground)
        {
            if (knockback_time <= 0) knockback = false;
            return;
        }
        else if (enemy_hit)
        {
            if (knockback) enemy_hit = false;
            vspeed = umbrella_fallspeed;
        }
        else
        {
            vspeed += gravity * Time.smoothDeltaTime;
        }
    }

    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space)) jump_buffer = jump_buffer_time;

        if (jump_buffer > 0 && on_ground)
        {
            on_ground = false;
            player_animator.SetBool("jumping", true);
            vspeed -= jump_force;
            jump_buffer = 0;
        }
    }

    void walking()
    {
        bool left = Input.GetKey(KeyCode.A);
        int left_sign = left ? 1 : 0;
        bool right = Input.GetKey(KeyCode.D);
        int right_sign = right ? 1 : 0;

        if (left_sign == 1 && h_accel < 0)
        {
            h_accel += accel_force*4;
            player_animator.SetBool("running", true);
            transform.localScale = new Vector3(-3, transform.localScale.y, transform.localScale.z);
        }
        else if (right_sign == 1 && h_accel > 0)
        {
            h_accel -= accel_force*4;
            player_animator.SetBool("running", true);
            transform.localScale = new Vector3(3, transform.localScale.y, transform.localScale.z);
        }
        else if (left_sign == 1)
        {
            h_accel += accel_force;
            player_animator.SetBool("running", true);
            transform.localScale = new Vector3(-3, transform.localScale.y, transform.localScale.z);
            if (h_accel >= accel_limit) h_accel = accel_limit;
        }
        else if (right_sign == 1)
        {
            h_accel -= accel_force;
            player_animator.SetBool("running", true);
            transform.localScale = new Vector3(3, transform.localScale.y, transform.localScale.z);
            if (h_accel <= -accel_limit) h_accel = -accel_limit;
        }

        if (right_sign == 0 && left_sign == 0 && h_accel > 0)
        {
            h_accel -= accel_force*2;
        }
        else if (right_sign == 0 && left_sign == 0 && h_accel < 0)
        {
            h_accel += accel_force*2;
        }
        
        if (h_accel < 0.005f && h_accel > -0.005f && right_sign == 0 && left_sign == 0 || right_sign == 1 && left_sign == 1)
        {
            player_animator.SetBool("running", false);
            h_accel = 0;
        }

        clamped_accel = Mathf.Clamp(h_accel, -accel_limit, accel_limit);
        
        hspeed = (((left_sign - right_sign) * move_speed) + clamped_accel);
    }

    void dash()
    {
        if(Input.GetMouseButtonDown(0) && dash_charges > 0)
        {
            dash_buffer = dash_buffer_time;
        }

        if(!dash_clicked && dash_buffer > 0)
        {
            dash_clicked = true;
            StartCoroutine(dashing());
        }
    }
    IEnumerator dashing()
    {
        FindObjectOfType<SoundManager>().play("dash_sound", UnityEngine.Random.Range(0.9f, 1.25f));

        dash_charges --;

        vspeed = 0;
        hspeed = 0;

        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dash_pos = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - pos;

        player_animator.SetBool("dash_start", true);

        yield return new WaitForSeconds(dash_init_delay);

        invulnerability = true;

        player_animator.SetBool("dash_start", false);

        dash_calculated = Mathf.Sqrt((dash_pos.x * dash_pos.x) + (dash_pos.y * dash_pos.y));

        dashh_accel = dash_pos.x * (dash_max_distance/dash_calculated);
        dashy_accel = dash_pos.y * (dash_max_distance/dash_calculated);

        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (mouse_pos.x < transform.position.x)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
        }

        if (mouse_pos.x > transform.position.x && transform.localScale.x < 0 || mouse_pos.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direction_changed = true;
        }

        dash_check = true;
        dash_duration = 1f - (dash_speed/100);

        yield return new WaitForFixedUpdate();

        GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.42f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);

        yield return new WaitForFixedUpdate();

        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        
        SpawnAnimation("SimpleEffect", "Sprites/dash_eff_animation2", false, new Vector3(transform.position.x, transform.position.y, -1), new Vector3(3,3,3), new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z));

        //CHECKING FOR WALLBUG EVERY FRAME
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);
        yield return new WaitForFixedUpdate();
        hits = Physics2D.RaycastAll(transform.position, dash_pos, (float) Math.Sqrt((hspeed * hspeed) + (vspeed * vspeed)) * 2);

        hit_test(hits);

        player_animator.SetBool("dash_end", true);

        GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.42f);

        yield return new WaitForSeconds(dash_end_delay);

        invulnerability = false;

        player_animator.SetBool("dash_end", false);
        dash_check = false;
        dash_clicked = false;
        transform.rotation = new Quaternion(0,0,0,0);
        if (transform.localScale.y < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
        }
        if (direction_changed)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            direction_changed = false;
        }

        yield return new WaitForFixedUpdate();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<Rizarudo>().single_hit = false;
        }
    }

    void move()
    {
        transform.position -= new Vector3 (hspeed, vspeed, 0);
    }

    void dash_move()
    {
        if (dash_check)
        {
            hspeed -= dashh_accel/(10 - (dash_speed/10));
            vspeed -= dashy_accel/(10 - (dash_speed/10));
            dash_duration -= 0.1f;

            if (dash_duration <= 0)
            {
                if (dashh_accel != 0) dashh_accel = 0;
                if (dashy_accel != 0) dashy_accel = 0;
                dash_duration = 0;
                vspeed = 0;
                hspeed = 0;
                h_accel = 0;
                dash_check = false;
            }
        }
    }

    void collision()
    {
        RaycastHit2D hit_left = Physics2D.Raycast(transform.position, Vector2.left, sprite_width + hspeed, ground_layer_mask);
        RaycastHit2D hit_right = Physics2D.Raycast(transform.position, Vector2.right, sprite_width - hspeed, ground_layer_mask);

        

        if (hit_right || hit_left)
        {
            if (dash_check && hit_right && hspeed < 0)
            {
                transform.position = hit_right.point - new Vector2(sprite_width, 0);
            }
            if (dash_check && hit_left && hspeed > 0)
            {
                transform.position = hit_left.point + new Vector2(sprite_width, 0);
            }
            hspeed = 0;
        }

        if (hit_right && hit_left)
        {
            //transform.position = last_unbugged_pos;
        }

        RaycastHit2D hit_ground = Physics2D.Raycast(transform.position, Vector3.down, sprite_height + vspeed, ground_layer_mask);
        RaycastHit2D hit_roof = Physics2D.Raycast(transform.position, Vector3.up, sprite_height - vspeed, ground_layer_mask);

        if (hit_ground && hit_ground.collider.tag == "Ground" && vspeed >= 0)
        {
            on_ground = true;
            vspeed = 0;
            enemy_hit = false;
            player_animator.SetBool("jumping", false);
            transform.position = hit_ground.point + new Vector2(0, sprite_height);
        }
        else
        {
            on_ground = false;
            player_animator.SetBool("jumping", true);
        }

        if (hit_roof && vspeed < 0)
        {
            vspeed = 0;
            transform.position = hit_roof.point - new Vector2(0, sprite_height/2);
        }

        if (hit_ground && hit_roof)
        {
            //transform.position = last_unbugged_pos;
        }
    }

    void hit_test(RaycastHit2D[] hits)
    {
        bool hit_enemy = false;
        bool hit_wall = false;
        List<RaycastHit2D> enemies = new List<RaycastHit2D>();
        List<RaycastHit2D> walls = new List<RaycastHit2D>();
        float min_dist = 10000;

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Enemy")
            {
                hit_enemy = true;
                enemies.Add(hit); 
            }
            if (hit.collider.tag == "Ground")
            {
                hit_wall = true;
                walls.Add(hit);
            }
        }

        if (hit_enemy && hit_wall)
        {
            float dist = 0;
            Vector2 point_diff;
            Vector2 point_to_go = new Vector2(0, 0);
            foreach(RaycastHit2D enemy in enemies)
            {
                foreach(RaycastHit2D wall in walls)
                {
                    point_diff = enemy.point - wall.point;
                    dist = Mathf.Sqrt((point_diff.x * point_diff.x) + (point_diff.y * point_diff.y));

                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        point_to_go = enemy.point;
                    } 
                }
            }

            if (min_dist <= 2f)
            {
                dash_check = false;

                hspeed = 0;

                vspeed = 0;

                transform.position = point_to_go;

                foreach(RaycastHit2D enemy in enemies)
                {
                    if (enemy.transform.gameObject.GetComponent<Rizarudo>().single_hit == false)
                    {
                        enemy_hit = true;
                        dash_combo_actual_time = dash_combo_time;
                        if (last_combo_enemy != enemy.transform.gameObject)
                        {
                            dash_combo ++;
                            last_combo_enemy = enemy.transform;
                        }
                        enemy.transform.gameObject.GetComponent<Rizarudo>().single_hit = true;
                        enemy.transform.gameObject.GetComponent<Rizarudo>().hit(transform.rotation.eulerAngles, attack_damage, dash_combo, GetComponent<Player>());
                    }
                    
                }
            }
        }
    }

    void take_knockback(float damage_dir)
    {
        knockback = true;

        StopCoroutine(dashing());

        enemy_hit = false;

        if (damage_dir >= 180) hspeed = 0.1f;
        else hspeed = -0.1f;

        vspeed -= jump_force/5;

        knockback_time = 0.2f;
    }
    
    void point_one_sec()
    {
        if (dash_charges < dash_charge_total) dash_actual_cd -= 0.1f;
        if (dash_actual_cd <= 0)
        {
            dash_actual_cd = dash_cd;
            dash_charges ++;
        }

        dash_combo_actual_time -= 0.1f;
        if (dash_combo_actual_time <= 0) 
        {
            dash_combo_actual_time = 0;
            dash_combo = 0;
            last_combo_enemy = transform;
        }

        knockback_time -= 0.1f;
        if (knockback_time <= 0) knockback_time = 0;
    }

    void download_stats()
    {
        max_hp = stats.max_hp;
        attack_damage = stats.attack_damage;
        hp = max_hp;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Enemy" && invulnerability && !col.GetComponent<Rizarudo>().single_hit)
        {
            enemy_hit = true;
            dash_combo_actual_time = dash_combo_time;
            if (last_combo_enemy != col.transform)
            {
                dash_combo ++;
                last_combo_enemy = col.transform;
            }
            col.GetComponent<Rizarudo>().single_hit = true;
            col.GetComponent<Rizarudo>().hit(transform.rotation.eulerAngles, attack_damage, dash_combo, GetComponent<Player>());
        }

        if (col.transform.tag == "Projectile" && invulnerability)
        {
            col.GetComponent<Projectile>().hit(transform.rotation.eulerAngles, GetComponent<Player>());

            if (last_combo_enemy != col.transform)
            {
                dash_combo ++;
                last_combo_enemy = col.transform;
            }
        }
        else if (col.transform.tag == "Projectile")
        {
            col.GetComponent<Projectile>().hit_player();

            hp --;

            take_knockback(col.transform.eulerAngles.z);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.transform.tag == "Enemy" && invulnerability && !col.GetComponent<Rizarudo>().single_hit)
        {
            enemy_hit = true;
            dash_combo_actual_time = dash_combo_time;
            if (last_combo_enemy != col.transform.gameObject)
            {
                dash_combo ++;
                last_combo_enemy = col.transform;
            }
            col.GetComponent<Rizarudo>().single_hit = true;
            col.GetComponent<Rizarudo>().hit(transform.rotation.eulerAngles, attack_damage, dash_combo, GetComponent<Player>());
        }

        if (col.transform.tag == "Projectile" && invulnerability)
        {
            col.GetComponent<Projectile>().hit(transform.rotation.eulerAngles, GetComponent<Player>());

            if (last_combo_enemy != col.transform)
            {
                dash_combo ++;
                last_combo_enemy = col.transform;
            }
        }
    }
}
