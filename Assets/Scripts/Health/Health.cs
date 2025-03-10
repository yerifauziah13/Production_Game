using System.Collections; // <-- Tambahkan ini
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iframesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer SpriteRend;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        SpriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            //iframes
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            SpriteRend.color = new Color(1, 0, 0, 0.5f); //merah transparan 
            yield return new WaitForSeconds(iframesDuration / (numberOfFlashes * 2));
            SpriteRend.color = Color.white; //warna putih
            yield return new WaitForSeconds(iframesDuration / (numberOfFlashes * 2));
        }
        //invulnerabity duration
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}