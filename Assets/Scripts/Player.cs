using UnityEngine;
using UnityEngine.InputSystem;
using R3;               // R3 core
using R3.Triggers;

/*
 ・ジャンプ制限　二回まで 済
 ・ジャンプできる回数をUI表示
 ・ステージ制作
 ・ダメージ表現　ノックバック
 ・始まった時のカウントダウンとタイムのカウントダウンをunitaskでやる
*/
public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    float step = 5.0f;
    public float MaxLife => 100f;
    public ReactiveProperty<float> life { get; private set; } = new();

    PlayerInput playerInput;
    Rigidbody2D rb;
    int jumpcount = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        life.Value = MaxLife;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(step * Time.deltaTime, 0, 0);
        // 移動
       

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            if(jumpcount > 0)
            {
               rb.linearVelocityY = jumpSpeed;
                jumpcount--;
            }
           
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {//ジャンプ回数リセット
            jumpcount = 2;
        }
    }
}
 /*
        var move = playerInput.actions["Move"].ReadValue<Vector2>();
        if (move.x != 0f)
        {
            rb.linearVelocityX = move.x * speed;

            // 向き
            var localScale = transform.localScale;
            if (move.x < 0)
            {
                localScale.x = 1f;
            }
            else
            {
                localScale.x = -1f;
            }
            transform.localScale = localScale;
        }*/