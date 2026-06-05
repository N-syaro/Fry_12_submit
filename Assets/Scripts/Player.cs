using UnityEngine;
using UnityEngine.InputSystem;
using R3;               // R3 core
using R3.Triggers;
using UnityEngine.UI;

/*
 ・ジャンプ制限　二回まで 済
 ・ジャンプできる回数をUI表示　済
 ・ステージ制作
 ・ダメージ表現　ノックバック
 ・始まった時のカウントダウンとタイムのカウントダウンをunitaskでやる
*/
public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] Image JumpCountImage1;
    [SerializeField] Image JumpCountImage2;
    [SerializeField]
    private Color DarkColor = new Color(0.5f,0.5f,0.5f,1.0f);
    [Header("ダメージ演出数値")]
    [SerializeField] private float knockbackForceX = 5f;
    [SerializeField] private float knockbackForceY = 3f;
    float step = 5.0f;
    public float MaxLife => 100f;
    public ReactiveProperty<float> life { get; private set; } = new();

    PlayerInput playerInput;
    Rigidbody2D rb;
    int jumpcount = 2;
    private Color orig_Coler1;
    private Color orig_Coler2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        life.Value = MaxLife;
        if(JumpCountImage1 !=null)
        {
            orig_Coler1 = JumpCountImage1.color;
        }
        if (JumpCountImage2 != null)
        {
            orig_Coler2 = JumpCountImage2.color;
        }
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
            switch (jumpcount)
            {
                    case 0:
                       JumpCountImage2.color = DarkColor;
                    break;
                    //
                    case 1:
                       JumpCountImage1.color = DarkColor;
                    break;
                    //
                    case 2:
                      JumpCountImage1.color = orig_Coler1;
                      JumpCountImage2.color = orig_Coler2;
                    break;
            }


        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {//ジャンプ回数リセット
            jumpcount = 2;
            JumpCountImage1.color = orig_Coler1;
            JumpCountImage2.color = orig_Coler2;
        }
        else if(collision.gameObject.tag == "DamageObj")
        {

        }
    }
}
