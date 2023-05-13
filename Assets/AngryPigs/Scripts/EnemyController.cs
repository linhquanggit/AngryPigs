using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
namespace AngryPigs
{

    public class EnemyController : MonoBehaviour
    {
        //ð?nh ngh?a tr?ng thái
        public enum State
        {
            Idle,
            Walk,
            Run,
            Hit,
        }    
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Rigidbody2D m_Rigidbody2D;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_WalkDistance;
        [SerializeField] private float m_EnemyRunSpeed;
        [SerializeField] private float m_EnemyHP;
        [SerializeField] private float m_GetDamage;
        private int m_ChangeParamHash;
        private int m_StateparamHash;

        private State m_CurrentState;
        private int m_Direction = 1;
        private Vector3 m_StartPosition;
        private bool m_GetHit;

        // g?c t?a ð? khi cast wall;
        [SerializeField] private Transform m_CastWallPoint;
        // g?c t?a ð? khi cast ground;
        [SerializeField] private Transform m_CastGroundPoint;

        //layermask c?a platform
        [SerializeField] LayerMask m_PlatformLayerMask;
        // Start is called before the first frame update
        void Start()
        {
            m_StartPosition = transform.position;
            m_ChangeParamHash = Animator.StringToHash("Change");
            m_StateparamHash = Animator.StringToHash("State");

            SetState(State.Idle);
            SetDirection(1);
            StartCoroutine(UpdatAI());
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (!Application.isPlaying)
                m_StartPosition = transform.position;
            Gizmos.DrawLine(new Vector2(m_StartPosition.x - m_WalkDistance, m_StartPosition.y), new Vector2(m_StartPosition.x + m_WalkDistance, m_StartPosition.y));

            //castwall line
            Gizmos.color = Color.cyan;
            Vector3 fromPos = m_CastWallPoint.position;
            Vector3 toPos = fromPos;
            toPos.x += m_Direction * 0.5f;
            Gizmos.DrawLine(fromPos, toPos);

            //castground line
            Gizmos.color = Color.magenta;
            fromPos = m_CastGroundPoint.position;
            toPos = fromPos;
            toPos.y -= 0.5f;
            Gizmos.DrawLine(fromPos, toPos);


        }
          
        private bool CheckWallAndGround()
        {
            bool hitWall = false;
            Vector3 fromPos = m_CastWallPoint.position;
            Vector3 toPos = fromPos;
            toPos.x += m_Direction * 0.5f;
            hitWall = Physics2D.Linecast(fromPos,toPos,m_PlatformLayerMask);


            bool noGround = true;
            fromPos = m_CastGroundPoint.position;
            toPos = fromPos;
            toPos.y -= 0.5f;
            noGround = !Physics2D.Linecast(fromPos, toPos, m_PlatformLayerMask);

            return hitWall || noGround;
        }

        private IEnumerator UpdatAI()
        {
            while(true)
            {
                if(m_CurrentState == State.Idle)
                {
                    //yield return new  WaitForSeconds(3f);
                    float time = 0;
                    while (time < 3 && !m_GetHit)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }
                    if(!m_GetHit)
                    SetState(State.Walk);
                }
                else if (m_CurrentState == State.Walk)
                {
                    float distance = Vector2.Distance(m_StartPosition, transform.position);
                    if(distance > m_WalkDistance || CheckWallAndGround())
                    {
                        if (transform.position.x > m_StartPosition.x && m_Direction == 1)
                        {
                            PlayIdleAnimation();
                            float time = 0;
                            while (time < 1 && !m_GetHit)
                            {
                                time += Time.deltaTime;
                                yield return null;
                            }
                            if (!m_GetHit)
                                //yield return new WaitForSeconds(1f);
                                PlayWalkAnimation();
                            SetDirection(-1);
                        }    
                        else if(transform.position.x < m_StartPosition.x && m_Direction == -1)
                        {
                            PlayIdleAnimation();
                            float time = 0;
                            while (time < 1 && !m_GetHit)
                            {
                                time += Time.deltaTime;
                                yield return null;
                            }
                            if (!m_GetHit)
                                //yield return new WaitForSeconds(1f);
                                PlayWalkAnimation();
                            SetDirection(1);
                        }
                    }
                    m_Rigidbody2D.velocity = new Vector2(m_WalkSpeed*m_Direction, m_Rigidbody2D.velocity.y);
                }
                else if(m_CurrentState == State.Hit)
                {
                    yield return new WaitForSeconds(0.5f);
                    m_GetHit = false;
                    SetState(State.Run);
                }
                else if (m_CurrentState == State.Run)
                {
                    float distance = Vector2.Distance(m_StartPosition, transform.position);
                    if (distance > m_WalkDistance || CheckWallAndGround())
                    {
                        if (transform.position.x > m_StartPosition.x && m_Direction == 1)
                        {
                            SetDirection(-1);
                        }
                        else if (transform.position.x < m_StartPosition.x && m_Direction == -1)
                        {
                            SetDirection(1);
                        }
                    }
                    m_Rigidbody2D.velocity = new Vector2(m_EnemyRunSpeed * m_Direction, m_Rigidbody2D.velocity.y);
                }
                yield return null; 
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (m_GetHit)
                return;
            if(collision.CompareTag("Attack"))
            {
                AudioManager.Instance.PlaySFX_EnemyGetHit();
                m_EnemyHP -= m_GetDamage;
                if (m_EnemyHP <= 0)
                {
                    Destroy(gameObject);
                    GamePlayManager.Instance.EnemyDied();
                    return;
                }
                m_GetHit = true;
                SetState(State.Hit);

                Vector2 knockbackDirection = transform.position - collision.transform.position;
                knockbackDirection = knockbackDirection.normalized;
                m_Rigidbody2D.AddForce(knockbackDirection * 10, ForceMode2D.Impulse);
            }
  

        }
        private void SetDirection(int direction)
        {
            m_Direction = direction;
            transform.localScale = new Vector3(-m_Direction, 1, 1);
        }
        private void SetState(State state)
        {
            m_CurrentState = state;
            switch(state)
            {
                case State.Idle:
                    PlayIdleAnimation();
                    break;
                case State.Walk:
                    PlayWalkAnimation();
                    break;
                case State.Run:
                    PlayRunAnimation();
                    break;
                case State.Hit:
                    PlayHitAnimation();
                    break;

                default:
                    break;
            }
        }    
        [ContextMenu ("Play Idle Animation")]
        private void PlayIdleAnimation()
        {
            m_Animator.SetTrigger(m_ChangeParamHash);
            m_Animator.SetInteger(m_StateparamHash,1);
        }
        [ContextMenu("Play Walk Animation")]
        private void PlayWalkAnimation()
        {
            m_Animator.SetTrigger(m_ChangeParamHash);
            m_Animator.SetInteger(m_StateparamHash,2);
        }
        [ContextMenu("Play Run Animation")]
        private void PlayRunAnimation()
        {
            m_Animator.SetTrigger(m_ChangeParamHash);
            m_Animator.SetInteger(m_StateparamHash, 3);
        }

        [ContextMenu("Play Hit Animation")]
        private void PlayHitAnimation()
        {
            m_Animator.SetTrigger(m_ChangeParamHash);
            m_Animator.SetInteger(m_StateparamHash, 4);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
