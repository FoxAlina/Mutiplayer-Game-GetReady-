using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public int playerId;

    public TouchManager TouchManager;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float speed = 10f;

    [SerializeField] public Slider healthBar;
    private int _health;
    public int health { get => _health; set => _health = value; }

    [SerializeField] float playerHeight = -15;

    bool isGameOver;

    [SerializeField] ObjectPool bulletPool;
    public float bulletFireRate = 0.25f;
    float timeCount = 0f;
    float shootTime = 0f;

    public ScoreAndHealthManager scoreAndHealthManager;

    [SerializeField] SpriteRenderer playerIcon;

    public override void OnNetworkSpawn()
    {
        scoreAndHealthManager = FindObjectOfType<ScoreAndHealthManager>();

        playerId = (int)NetworkObjectId;
        playerIcon.sprite = FindObjectOfType<PlayerIconsList>().getIcon((int)NetworkObjectId);

        if (!IsOwner) enabled = false;

        healthBar.maxValue = scoreAndHealthManager.maxHealth;
        healthBar.value = scoreAndHealthManager.maxHealth;
    }

    private void OnEnable()
    {
        PlayerShooting.OnPlayerFire += Fire;
    }

    private void OnDisable()
    {
        PlayerShooting.OnPlayerFire -= Fire;
    }

    void Start()
    {
        TouchManager = FindObjectOfType<TouchManager>();

        isGameOver = false;
    }

    void Update()
    {
        if (!isGameOver)
        {
            Vector3 v = TouchManager.getTargetVector();
            if (v != Vector3.zero)
            {
                Vector3 scaledMovement = rotationSpeed * Time.deltaTime * new Vector3(v.x, v.y, playerHeight);
                //transform.LookAt(transform.position + scaledMovement, Vector3.forward);
                transform.rotation = Quaternion.LookRotation(scaledMovement, Vector3.forward);

                transform.Translate(speed * v.magnitude * Vector2.up * Time.deltaTime);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, playerHeight);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOwner)
            if (collision.collider.tag == "Coin")
            {
                scoreAndHealthManager.collectCoin();
            }
    }

    public void setBulletIds()
    {
        for (int i = 0; i < bulletPool.SharedInstance.amountToPool; i++)
        {
            bulletPool.SharedInstance.pooledObjects[i].GetComponent<Bullet>().playerId = playerId;
        }
    }

    #region GetDamage
    public void BulletHit()
    {
        if (IsOwner)
        {
            scoreAndHealthManager.getDamage();
            healthBar.value = scoreAndHealthManager.health;
            //RequestGetDamageServerRpc();
        }        
    }

    public void showHealth()
    {
        healthBar.value = _health;
    }

    //[ServerRpc]
    //private void RequestGetDamageServerRpc()
    //{
    //    GetDamageClientRpc();
    //}

    //[ClientRpc]
    //private void GetDamageClientRpc()
    //{
    //    if (!IsOwner)
    //    {
    //        healthBar.value = scoreAndHealthManager.health;
    //    }
    //}
    #endregion

    #region Shooting
    public void Fire()
    {
        if (!isGameOver)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= shootTime)
            {
                shootTime = timeCount + bulletFireRate;

                RequestFireServerRpc(transform.position, transform.rotation.eulerAngles);

                ExecuteShoot(transform.position, transform.rotation.eulerAngles);

                //GameObject bullet = bulletPool.SharedInstance.GetPooledObject();
                //if (bullet != null)
                //{
                //    bullet.transform.position = transform.position;
                //    bullet.transform.rotation = transform.rotation;
                //    bullet.transform.Rotate(new Vector3(0f, 0f, 90f));
                //    bullet.SetActive(true);
                //}
            }
        }
    }

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 pos, Vector3 rot)
    {
        FireClientRpc(pos, rot);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 pos, Vector3 rot)
    {
        if (!IsOwner) ExecuteShoot(pos, rot);
    }

    private void ExecuteShoot(Vector3 pos, Vector3 rot)
    {
        GameObject bullet = bulletPool.SharedInstance.GetPooledObject();
        if (bullet != null)
        {
            bullet.GetComponent<Bullet>().playerId = playerId;
            bullet.transform.position = pos;
            bullet.transform.rotation = Quaternion.Euler(rot);
            bullet.transform.Rotate(new Vector3(0f, 0f, 90f));
            bullet.SetActive(true);
        }
    }

    #endregion
}
