using UnityEngine;
using UnityEngine.InputSystem;
using R3;
//using R3.Triggers;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

/*
 ・ジャンプ制限　二回まで 済
 ・ジャンプできる回数をUI表示　済
 ・ステージ制作
 ・ダメージ表現　ノックバック 済
 ・ダメージ時点滅 済
 ・始まった時のカウントダウンとタイムのカウントダウンをunitaskでやる
*/
public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] Image JumpCountImage1;
    [SerializeField] Image JumpCountImage2;
    [SerializeField]
    private Color DarkColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    [Header("ダメージ演出数値")]
    [SerializeField] private float knockbackForceX = 5f;
    [SerializeField] private float knockbackForceY = 3f;

    [Header("点滅設定")]
    [SerializeField] private int blinkCount = 5;           // 点滅回数
    [SerializeField] private float blinkInterval = 0.1f;  // 点滅間隔（秒）
    [SerializeField] private float invincibleDuration = 1.0f; // 無敵時間（秒）

    float step = 5.0f;
    public float MaxLife => 100f;
    public ReactiveProperty<float> life { get; private set; } = new();

    PlayerInput playerInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer; // 点滅用
    int jumpcount = 2;
    private Color orig_Coler1;
    private Color orig_Coler2;

    bool Isdamage = false;
    private bool isInvincible = false; // 無敵フラグ
    private bool isend = false; 
    private CancellationTokenSource blinkCts; // 点滅キャンセル用

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        life.Value = MaxLife;

        if (JumpCountImage1 != null)
            orig_Coler1 = JumpCountImage1.color;

        if (JumpCountImage2 != null)
            orig_Coler2 = JumpCountImage2.color;
    }
    void Update()
    {
        if(isend)
        {
            return;
        }
        if(Isdamage == false)
        {
            this.transform.position += new Vector3(step * Time.deltaTime, 0, 0);
        }
        

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            if (jumpcount > 0)
            {
                rb.linearVelocityY = jumpSpeed;
                jumpcount--;
            }
            switch (jumpcount)
            {
                case 0:
                    JumpCountImage2.color = DarkColor;
                    break;
                case 1:
                    JumpCountImage1.color = DarkColor;
                    break;
                case 2:
                    JumpCountImage1.color = orig_Coler1;
                    JumpCountImage2.color = orig_Coler2;
                    break;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            // ジャンプ回数リセット
            jumpcount = 2;
            JumpCountImage1.color = orig_Coler1;
            JumpCountImage2.color = orig_Coler2;
        }
        else if (collision.gameObject.tag == "Damageobj")
        {
            if (!isInvincible)
            {
                ApplyDamage(collision);
            }
        }
    }
    /// <summary>
    /// ダメージ処理：ノックバック ＋ 点滅 ＋ 無敵時間
    /// </summary>
    private void ApplyDamage(Collision2D collision)
    {
        // --- ノックバック ---
        // 衝突してきたオブジェクトの方向を判定してX方向を決める
        float dirX = transform.position.x - collision.transform.position.x;
        dirX = dirX >= 0 ? 1f : -1f;

        rb.linearVelocity = Vector2.zero; // 現在の速度をリセット
        rb.AddForce(new Vector2(dirX * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);

        // --- HP減少（例：10ダメージ） ---
        life.Value = Mathf.Max(0f, life.Value - 10f);

        // --- 点滅 ＋ 無敵時間 ---
        // 既存の点滅をキャンセルして新しく開始
        blinkCts?.Cancel();
        blinkCts?.Dispose();
        blinkCts = new CancellationTokenSource();
        BlinkAsync(blinkCts.Token).Forget();
    }

    /// <summary>
    /// 点滅しながら無敵時間を付与するAsync処理
    /// </summary>
    private async UniTaskVoid BlinkAsync(CancellationToken ct)
    {
        Isdamage = true;
        isInvincible = true;

        try
        {
            float elapsed = 0f;
            bool visible = true;

            while (elapsed < invincibleDuration)
            {
                ct.ThrowIfCancellationRequested();

                // 表示トグル
                visible = !visible;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = visible;

                await UniTask.WaitForSeconds(blinkInterval, cancellationToken: ct);
                elapsed += blinkInterval;
            }
        }
        catch (System.OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
        finally
        {
            // 必ずスプライトを表示状態に戻す
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            Isdamage = false;   
            isInvincible = false;
        }
    }

    private void OnDestroy()
    {
        blinkCts?.Cancel();
        blinkCts?.Dispose();
    }
    public void IsEnd(bool i)
    {
        if(i)
        {
            isend = true;
        }
    }
}