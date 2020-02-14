using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float zVelocity = 5f;
    [SerializeField]
    float timeToApex = 0.6f;
    [SerializeField]
    float minJumpHeight = 2;
    [SerializeField]
    float maxJumpHeight = 10;
    [SerializeField]
    CameraController cameraController = null;

    public static float widthLevel = 10;
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float range = 2f;


    int depthLevel = 0;
    int changingDepth = 0; // 0 ne bouge pas, 1 va vers le fond, -1 se rapproche de la caméra


    bool canAttack = false;

    bool dead = false;

    Vector3 velocity;
    float directionZ;
    float directionX;

    bool canChangeSide = false;
    float maxJumpVelocity;
    float minJumpVelocity;
    float gravity;

    float smoothX = 0;
    float smoothXtime = 0.05f;

    BoxCollider2D hitbox;
    CollisionController controller;
    Animator anim;
    SpriteRenderer sprite;

    public ParticleSystem score50;
    public ParticleSystem score75;
    public ParticleSystem score100;
    public ParticleSystem score125;
    public ParticleSystem score150;
    public ParticleSystem dust;

    public GameObject lightEffect;

    private void Start()
    {
        dead = false;

        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        controller = GetComponent<CollisionController>();

        gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        cameraController.MoveCamera(transform.position);
    }

    void Update()
    {

        if (controller.collisions.below)
        {
            anim.SetBool("jump", false);
            velocity.y = 0;
        }

        Debug.DrawRay(new Vector3(hitbox.bounds.center.x + hitbox.bounds.size.x/2, hitbox.bounds.center.y + hitbox.bounds.size.y / 2, 0), Vector3.right * (sprite.flipX ? -1 : 1) * range/2, Color.green);
        Debug.DrawRay(new Vector3(hitbox.bounds.center.x + hitbox.bounds.size.x/2, hitbox.bounds.center.y - hitbox.bounds.size.y / 2, 0), Vector3.right * (sprite.flipX ? -1 : 1) * range/2, Color.green);

        Inputs();

        if (!GameState.paused)
        {

            if (canChangeSide && directionZ != 0 || changingDepth != 0) // Si le joueur veut changer de niveau et le peut, ou bien qu'il est entrain de changer de niveau
            {
                DepthChanged(directionZ);
            }

            velocity.y -= gravity * Time.deltaTime;

            velocity.x = Mathf.SmoothDamp(velocity.x, directionX * (speed - (1.5f * depthLevel)), ref smoothX, smoothXtime);

            anim.SetFloat("speed", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z));

            LayerMask path;

            switch (depthLevel)
            {
                case 0:
                    path = LayerMask.GetMask("FirstPath");
                    break;
                case 1:
                    path = LayerMask.GetMask("SecondPath");
                    break;
                case 2:
                    path = LayerMask.GetMask("ThirdPath");
                    break;
                default:
                    Debug.Log("Pas de chemin assigné " + depthLevel);
                    path = LayerMask.GetMask("FirstPath");
                    break;
            }

            controller.Move(velocity * Time.deltaTime, path);

            cameraController.MoveCamera(transform.position);

        }
    }

    private void Inputs()
    {
        if (!GameState.paused && !dead)
        {
            if (!anim.GetBool("charging"))
            {
                directionX = Input.GetAxisRaw("Horizontal");
                directionZ = Input.GetAxisRaw("Vertical");
            } else
            {
                directionX = 0;
                directionZ = 0;
            }

            if (directionX != 0)
            {
                sprite.flipX = (directionX == -1);
            }

            if (directionZ != 0)
            {
                CanChangeSide(directionZ);
            }

            if (Input.GetButtonDown("Jump"))
            {
                OnJumpInputDown();
            }

            if (Input.GetButtonUp("Jump"))
            {
                OnJumpInputUp();
            }

            if (Input.GetButtonDown("Attack"))
            {
                OnAttackButtonDown();
            }

            if (Input.GetButtonUp("Attack"))
            {
                OnAttackButtonUp();
            }
        } else if (dead)
        {
            directionX = 0;
            directionZ = 0;
        }

        if (Input.GetButtonDown("Pause"))
        {
            GameState.Pause();
        }
    }

    private void OnAttackButtonDown()
    {
        if (controller.collisions.below)
        {
            Debug.Log("TRY ATTACK");
            anim.SetBool("charging", true);
        }
    }

    private void OnAttackButtonUp()
    {
        if (canAttack) // Si on attend assez jusqu'au ResetCanAttack on peut attaquer, à faire à partir de Animator quand on aura les sprites
        {
            canAttack = false;
            anim.SetBool("attack", true);
        }
        anim.SetBool("charging", false);
    }

    private void OnJumpInputDown()
    {
        if (controller.collisions.below) // On ne peut sauter que si l'on touche le sol
        {
            velocity.y = maxJumpVelocity;

            if (canAttack) // Si on attend assez jusqu'au ResetCanAttack on peut attaquer, à faire à partir de Animator quand on aura les sprites
            {
                canAttack = false;
                anim.SetBool("attack", true);
            } else
            {
                anim.SetBool("jump", true);
            }

            anim.SetBool("charging", false);
        }
    }

    private void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity) // Si on relache le bouton trop tôt on précipite le diminution de vitesse du saut
        {
            velocity.y = minJumpVelocity;
        }  
    }

    private void OnAttackCharged()
    {
        canAttack = true;
    }

    private void OnAttackFinished()
    {
        anim.SetBool("attack", false);
    }

    private void Attack()
    {
        Collider2D enemy;
        if ((enemy = Physics2D.OverlapBox(new Vector3(hitbox.bounds.center.x + (hitbox.bounds.size.x * (sprite.flipX ? -1 : 1)), hitbox.bounds.center.y, 0), new Vector2(range, hitbox.bounds.size.y), 0, LayerMask.GetMask("Enemy"), depthLevel*widthLevel, depthLevel*widthLevel)) != null)
        {
            CameraShake.ShakeCamera(true);
            Debug.Log("I hit " + enemy.name);

            float scoreValue = enemy.GetComponent<Enemy>().scoreValue;
            
            switch(scoreValue)
            {
                case 50:
                    score50.Emit(1);
                    break;
                case 75:
                    score75.Emit(1);
                    break;
                case 100:
                    score100.Emit(1);
                    break;
                case 125:
                    score125.Emit(1);
                    break;
                case 150:
                    score150.Emit(1);
                    break;
                default:
                    Debug.Log("Unknown score " + scoreValue);
                    break;
            }

            enemy.transform.GetComponent<Enemy>().Kill();
            lightEffect.SetActive(true);
        } else
        {
            CameraShake.ShakeCamera(false);
        }
        Debug.Log("ATTACK");
    }

    public void GetHit()
    {
        if (!dead)
        {
            anim.SetTrigger("death");
            dead = true;
        }
    }

    public void Dust()
    {
        dust.GetComponent<ParticleSystemRenderer>().sortingLayerID = sprite.sortingLayerID;
        dust.Emit(100);
    }

    public void PlaySound(string sound)
    {
        Audio.AudioManager.Play(sound);
    }

    public void StopSound(string sound)
    {
        Audio.AudioManager.Stop(sound);
    }

    private void GameOver()
    {
        GameState.GameOver();
    }

    private void CanChangeSide(float directionZ)
    {
        if (directionZ < 0) // Si on veut descendre de chemin
        {
            if (depthLevel == 1) // On ne peut le faire que depuis le dernier et le deuxième chemin
            {
                if (Physics2D.OverlapCircle(transform.position, hitbox.bounds.size.x / 2, LayerMask.GetMask("FirstBridge")) != null)
                {
                    canChangeSide = true;
                }
                else
                {
                    canChangeSide = false;
                }
            } else if (depthLevel == 2)
            {
                if (Physics2D.OverlapCircle(transform.position, hitbox.bounds.size.x / 2, LayerMask.GetMask("SecondBridge")) != null)
                {
                    canChangeSide = true;
                }
                else
                {
                    canChangeSide = false;
                }
            } else
            {
                canChangeSide = false;
            }
        } else // Sinon on veut monter
        {
            if (depthLevel == 0) // On ne peut le faire que depuis le premier et le deuxième chemin
            {
                if (Physics2D.OverlapCircle(transform.position, hitbox.bounds.size.x / 2, LayerMask.GetMask("FirstBridge")) != null)
                {
                    canChangeSide = true;
                }
                else
                {
                    canChangeSide = false;
                }

            } else if (depthLevel == 1)
            {
                if (Physics2D.OverlapCircle(transform.position, hitbox.bounds.size.x / 2, LayerMask.GetMask("SecondBridge")) != null)
                {
                    canChangeSide = true;
                }
                else
                {
                    canChangeSide = false;
                }
            }
            else
            {
                canChangeSide = false;
            }
        }
    }

    void DepthChanged(float directionZ)
    {
        if (directionZ < 0 && depthLevel > 0 && changingDepth != -1) // On vérifie de ne pas dépasser le niveau de profondeur min et max, et de ne pas sauter un niveau en changeant pendant une transition
         {
            depthLevel--;
            changingDepth = -1;

        } else if (directionZ > 0 && depthLevel < 2 && changingDepth != 1)
        {
            depthLevel++;
            changingDepth = 1;
        }

        if (transform.position.z >= depthLevel * widthLevel && changingDepth == 1 || //Sinon on vérifie où en est la transition 
            transform.position.z <= depthLevel * widthLevel && changingDepth == -1) // Si on est arrivé à la bonne profondeur on s'arrête
        {
            changingDepth = 0;
            transform.position = new Vector3(transform.position.x, transform.position.y, depthLevel * widthLevel);
        }

        if (depthLevel == 0)
        {
            sprite.sortingLayerName = "FirstPath";
        } else if (depthLevel == 1)
        {
            sprite.sortingLayerName = "SecondPath";
        } else
        {
            sprite.sortingLayerName = "ThirdPath";
        }

        velocity.z = changingDepth * zVelocity;
    }
}
