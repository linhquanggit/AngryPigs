using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;

namespace AngryPigs
{
    public class PlayerController : MonoBehaviour
    {

        public Action<int, int> onCurrentHpChange;
        [SerializeField] private Rigidbody2D m_RigidBody;
        [SerializeField] private Animator m_Animator;

        //walk
        [SerializeField] private float m_WalkingSpeed;

        //jump
        [SerializeField] private float JumpForce;
        [SerializeField] private LayerMask m_GroundLayerMask;
        [SerializeField] private Transform m_GroundCastPoint;
        [SerializeField] private Vector2 m_GroundCastSize;

        //climb
        [SerializeField] private LayerMask m_ClimbableLayerMask;
        [SerializeField] private float m_ClimbSpeed;
        [SerializeField] private float m_KnockBackForce;

        //hp player
        [SerializeField] private int m_MaxHP;
        [SerializeField] private int m_GetDamage;
        private int m_CurrentHP;
        private bool m_GetHit;
        private float m_GetHitTime;
        private bool m_Dead;

        private int m_AttackHash;
        private int m_IdleHash;
        private int m_WalkingHash;
        private int m_DyingHash;

        private PlayerInputActions m_PlayerInput;
        private Vector2 m_MovementInput;
        private bool m_OnGround;
        private int m_JumpCount;
        private bool m_AttackInput;
        private Collider2D m_Collider2D;

        private void OnEnable()
        {
            if(m_PlayerInput == null)
            {
                m_PlayerInput = new PlayerInputActions();
                m_PlayerInput.Player.Movement.started += OnMovement;
                m_PlayerInput.Player.Movement.performed += OnMovement;
                m_PlayerInput.Player.Movement.canceled += OnMovement;

                m_PlayerInput.Player.Jump.started += OnJump;
                m_PlayerInput.Player.Jump.performed += OnJump;
                m_PlayerInput.Player.Jump.canceled += OnJump;

                m_PlayerInput.Player.Attack.started += OnAttack;
                m_PlayerInput.Player.Attack.performed += OnAttack;
                m_PlayerInput.Player.Attack.canceled += OnAttack;
            }
            m_PlayerInput.Enable();
        }

       
        private void OnDisable()
        {
            if(m_PlayerInput != null)
            {
                m_PlayerInput.Disable();
            }
        }
        void Start()
        {
            TryGetComponent(out m_Collider2D);
            m_AttackHash = Animator.StringToHash("Attack");
            m_IdleHash = Animator.StringToHash("Idle");
            m_WalkingHash = Animator.StringToHash("Walking");
            m_DyingHash = Animator.StringToHash("Dying");
            m_CurrentHP = m_MaxHP;
            if(onCurrentHpChange !=null)
            {
                onCurrentHpChange(m_CurrentHP, m_MaxHP);
            }
        }
        private void FixedUpdate()
        {
            if (m_GetHit || m_Dead)
                return;
            m_OnGround = Physics2D.BoxCast(m_GroundCastPoint.position, m_GroundCastSize, 0, Vector3.forward, 0, m_GroundLayerMask);
            if(m_OnGround)
                m_JumpCount = 0;
            CheckMovement();
            CheckClimb();
            CheckAnimations();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireCube(m_GroundCastPoint.position, m_GroundCastSize);
        }
        //code di chuyen
        private void CheckMovement()
        {
            if (m_AttackInput)
                return;
            m_RigidBody.velocity = new Vector2(m_MovementInput.x * m_WalkingSpeed, m_RigidBody.velocity.y);
            
        }
        private void CheckAnimations()
        {
            m_Animator.SetBool(m_AttackHash, m_AttackInput);
            m_Animator.SetBool(m_IdleHash, m_RigidBody.velocity.x == 0 && !m_AttackInput);
            m_Animator.SetBool(m_WalkingHash, m_RigidBody.velocity.x != 0 && !m_AttackInput);
        }    
        
        private void CheckClimb()
        {
            if (m_Collider2D.IsTouchingLayers(m_ClimbableLayerMask))
            {
                Vector2 velocity = m_RigidBody.velocity;
                velocity.y = m_ClimbSpeed * m_MovementInput.y;
                m_RigidBody.velocity = velocity;
                m_RigidBody.gravityScale = 0;
            }
            else
                m_RigidBody.gravityScale = 2f;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                m_AttackInput = true;
            else
                m_AttackInput = false;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (m_AttackInput || m_GetHit || m_Dead)
                return;
            if (context.started || context.performed)
            {
                if (m_OnGround || m_JumpCount < 2)
                {
                    m_JumpCount++;
                    m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, JumpForce);
                }
            }
        }

        private void OnMovement(InputAction.CallbackContext context)       
        {
            if (context.started || context.performed)
            {
                m_MovementInput = context.ReadValue<Vector2>();
                transform.localScale = new Vector3(m_MovementInput.x >= 0 ? 1 : -1, 1, 1);
            }
            else
                m_MovementInput = Vector2.zero;
        }
        [ContextMenu("Play Attack Anim")]
        private void PlayAttackAnim()
        {
            m_Animator.SetBool(m_AttackHash, true);
            m_Animator.SetBool(m_IdleHash, false);
            m_Animator.SetBool(m_WalkingHash, false);
        }

        [ContextMenu("Play Idle Anim")]
        private void PlayIdleAnim()
        {
            m_Animator.SetBool(m_IdleHash, true);
            m_Animator.SetBool(m_AttackHash, false);
            m_Animator.SetBool(m_WalkingHash, false);
        }

        [ContextMenu("Play Walking Anim")]
        private void PlayWalkingAnim()
        {
            m_Animator.SetBool(m_WalkingHash, true);
            m_Animator.SetBool(m_IdleHash, false);
            m_Animator.SetBool(m_AttackHash, false);
        }

        [ContextMenu("Play Dying Anim")]
        private void PlayDyingAnim()
        {
            m_Animator.SetBool(m_DyingHash, true);
        }

        [ContextMenu("Reset Anim")]
        private void ResetAnim()
        {
            m_Animator.SetBool(m_IdleHash, true);
            m_Animator.SetBool(m_WalkingHash, false);
            m_Animator.SetBool(m_AttackHash, false);
            m_Animator.SetBool(m_DyingHash, false);
        }
        // Start is called before the first frame update
       

        // Update is called once per frame
        void Update()
        {
            if(m_GetHit)
            {
                m_GetHitTime -= Time.deltaTime;
                if (m_GetHitTime <= 0)
                    m_GetHit = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(m_GetHit || m_Dead)
                return;
            if(collision.CompareTag("Enemy"))
            {
                //get hit
                m_CurrentHP -= m_GetDamage;
                m_GetHit = true;
                m_GetHitTime = 0.5f;
                if (onCurrentHpChange != null)
                {
                    onCurrentHpChange(m_CurrentHP, m_MaxHP);
                }
                if (m_CurrentHP<=0)
                {
                    m_Dead = true;
                    AudioManager.Instance.PlaySFX_PlayerDead();
                    GamePlayManager.Instance.Gameover(false);
                    GamePlayManager.Instance.IsNextLevelBtnDisable();
                    PlayDyingAnim();
                    return;
                }

                AudioManager.Instance.PlaySFX_PlayerGetHit();
                Vector2 knockbackDirection = transform.position - collision.transform.position;
                knockbackDirection = knockbackDirection.normalized;
                m_RigidBody.AddForce(knockbackDirection * m_KnockBackForce, ForceMode2D.Impulse);

                StartCoroutine(GetHitFx());
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("MoveablePlatform"))
            {
                transform.SetParent(collision.transform);
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MoveablePlatform"))
            {
                transform.SetParent(collision.transform);
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MoveablePlatform"))
            {
                transform.SetParent(null);
            }
        }
        private IEnumerator GetHitFx()
        {

            /* Cinemachine.CinemachineImpulseSource impulseSource;
             TryGetComponent(out impulseSource);
             impulseSource.GenerateImpulse();*/

            CameraShake.Instance.Shake(0.1f);
            SpriteRenderer spt;

            TryGetComponent(out spt);
            Color transparent = Color.white;
            /*transparent.a: ð? trong su?t c?a player*/
            transparent.a = 0.025f;
            int i = 0;
            while(m_GetHitTime>0)
            {
                if (i % 2 == 0)
                    spt.color = Color.white;
                else
                    spt.color = transparent;
                i++;
                yield return null;
            }
            spt.color = Color.white;
        }
        private void PlayAttackSFX()
        {
            AudioManager.Instance.PlaySFX_MeleeSplash();
        }
    }
}
