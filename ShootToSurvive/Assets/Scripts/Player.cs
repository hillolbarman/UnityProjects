using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 7;
    PlayerController controller;
    GunController gunController;
    Camera viewCamera;
    public Crosshairs crosshairs;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        //Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 velocity = moveInput.normalized * moveSpeed;
        controller.Move(velocity);

        //Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up*gunController.GunHeight);
        float rayDistance;
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 pointOfIntersection = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, pointOfIntersection,Color.red);
            controller.LookAt(pointOfIntersection);
            crosshairs.transform.position = pointOfIntersection;
            crosshairs.DetectTargets(ray);
            if((new Vector2(pointOfIntersection.x, pointOfIntersection.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude>Mathf.Pow(1.5f,2)) 
                gunController.Aim(pointOfIntersection);
        }

        //Weapon Input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if (transform.position.y < -10)
        {
            TakeDamage(health);
        }
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}
