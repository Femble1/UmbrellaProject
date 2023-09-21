using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animated : MonoBehaviour
{
    [SerializeField]
    public List <GameObject> created_anims;

    void OnAwake()
    {
        created_anims = new List<GameObject>();
    }

    public void SpawnAnimation(string effect_type, string animation_path, bool set_transform_to_parent, Vector3 position = default(Vector3), Vector3 scale = default(Vector3), Vector3 rotation = default(Vector3), float anim_speed = 1f)
    {
        Debug.Log(rotation);
        switch (created_anims.Count)
        {
            case 0: 
            GameObject hit_effect = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect);
            if (set_transform_to_parent)
            {
                hit_effect.transform.SetParent(transform, false);
            }
            hit_effect.transform.position = position;
            hit_effect.transform.localScale = scale;
            hit_effect.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override = new AnimatorOverrideController(hit_effect.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect.GetComponent<Animator>().speed = anim_speed;
            hit_effect.GetComponent<Animator>().runtimeAnimatorController = animator_override;

            animator_override["default"] = Resources.Load<AnimationClip>(animation_path);
            
            Destroy(hit_effect, Resources.Load<AnimationClip>(animation_path).length/hit_effect.GetComponent<Animator>().speed);
            break;
            case 1:
            GameObject hit_effect2 = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect2);
            if (set_transform_to_parent)
            {
                hit_effect2.transform.SetParent(transform, false);
            }
            hit_effect2.transform.position = position;
            hit_effect2.transform.localScale = scale;
            hit_effect2.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect2.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override2 = new AnimatorOverrideController(hit_effect2.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect2.GetComponent<Animator>().speed = anim_speed;
            hit_effect2.GetComponent<Animator>().runtimeAnimatorController = animator_override2;

            animator_override2["default"] = Resources.Load<AnimationClip>(animation_path);

            Destroy(hit_effect2, Resources.Load<AnimationClip>(animation_path).length/hit_effect2.GetComponent<Animator>().speed);
            break;
            case 2:
            GameObject hit_effect3 = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect3);
            if (set_transform_to_parent)
            {
                hit_effect3.transform.SetParent(transform, false);
            }
            hit_effect3.transform.position = position;
            hit_effect3.transform.localScale = scale;
            hit_effect3.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect3.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override3 = new AnimatorOverrideController(hit_effect3.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect3.GetComponent<Animator>().speed = anim_speed;
            hit_effect3.GetComponent<Animator>().runtimeAnimatorController = animator_override3;

            animator_override3["default"] = Resources.Load<AnimationClip>(animation_path);
            
            Destroy(hit_effect3, Resources.Load<AnimationClip>(animation_path).length/hit_effect3.GetComponent<Animator>().speed);
            break;
            case 3:
            GameObject hit_effect4 = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect4);
            if (set_transform_to_parent)
            {
                hit_effect4.transform.SetParent(transform, false);
            }
            hit_effect4.transform.position = position;
            hit_effect4.transform.localScale = scale;
            hit_effect4.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect4.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override4 = new AnimatorOverrideController(hit_effect4.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect4.GetComponent<Animator>().speed = anim_speed;
            hit_effect4.GetComponent<Animator>().runtimeAnimatorController = animator_override4;

            animator_override4["default"] = Resources.Load<AnimationClip>(animation_path);

            Destroy(hit_effect4, Resources.Load<AnimationClip>(animation_path).length/hit_effect4.GetComponent<Animator>().speed);
            break;
            case 4:
            GameObject hit_effect5 = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect5);
            if (set_transform_to_parent)
            {
                hit_effect5.transform.SetParent(transform, false);
            }
            hit_effect5.transform.position = position;
            hit_effect5.transform.localScale = scale;
            hit_effect5.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect5.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override5 = new AnimatorOverrideController(hit_effect5.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect5.GetComponent<Animator>().speed = anim_speed;
            hit_effect5.GetComponent<Animator>().runtimeAnimatorController = animator_override5;

            animator_override5["default"] = Resources.Load<AnimationClip>(animation_path);
            
            Destroy(hit_effect5, Resources.Load<AnimationClip>(animation_path).length/hit_effect5.GetComponent<Animator>().speed);
            break;
            case 5:
            GameObject hit_effect6 = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + effect_type));
            created_anims.Add(hit_effect6);
            if (set_transform_to_parent)
            {
                hit_effect6.transform.SetParent(transform, false);
            }
            hit_effect6.transform.position = position;
            hit_effect6.transform.localScale = scale;
            hit_effect6.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            hit_effect6.GetComponent<SimpleEffect>().parent = GetComponent<Animated>();
            
            AnimatorOverrideController animator_override6 = new AnimatorOverrideController(hit_effect6.GetComponent<Animator>().runtimeAnimatorController);
            hit_effect6.GetComponent<Animator>().speed = anim_speed;
            hit_effect6.GetComponent<Animator>().runtimeAnimatorController = animator_override6;

            animator_override6["default"] = Resources.Load<AnimationClip>(animation_path);
            
            Destroy(hit_effect6, Resources.Load<AnimationClip>(animation_path).length/hit_effect6.GetComponent<Animator>().speed);
            break;
        }
    }
}
