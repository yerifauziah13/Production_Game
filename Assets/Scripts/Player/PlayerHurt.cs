using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] private float hurtCooldown;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private void Awake()
    {
        anim = GetComponent<Animator>();

        // Coba ambil komponen dari objek yang sama
        playerMovement = GetComponent<PlayerMovement>();
        // Jika tidak ditemukan, cari di scene
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }
    }
    private void Update()
    {
        // Pastikan playerMovement tidak null sebelum memanggil metode
        if (playerMovement != null && Input.GetMouseButton(0) && cooldownTimer > hurtCooldown && playerMovement.CanHurt())
        {
            Hurt();
        }
        cooldownTimer += Time.deltaTime;
    }
    private void Hurt()
    {
        anim.SetTrigger("Hurt");
        cooldownTimer = 0;
    }
}
