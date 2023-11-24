﻿using System.Collections;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Script for the bullet projectile of the Bullet and MainBullet ability
/// </summary>
public class CruiserBulletScript : MonoBehaviour
{
    public bool missParticles = false;
    public GameObject missPrefab;
    private float damage; // damage of the spawned bullet
    private int faction;
    public Entity owner;
    private Entity.TerrainType terrain;
    private Entity.EntityCategory category;
    private float pierceFactor = 0;
    public Color particleColor;
    Vector2 vector;
    public bool disableDrones;
    public Transform target;
    Entity previousID;

    /// <summary>
    /// Sets the damage value of the spawned buller
    /// </summary>
    /// <param name="damage">The damage the bullet does</param>
    public void SetDamage(float damage)
    {
        this.damage = damage; // set damage
    }

    public void SetPierceFactor(float pierce)
    {
        pierceFactor = pierce;
    }

    public void SetShooterFaction(int faction)
    {
        this.faction = faction;
    }

    public void SetTerrain(Entity.TerrainType terrain)
    {
        this.terrain = terrain;
    }

    public void SetCategory(Entity.EntityCategory category)
    {
        this.category = category;
    }

    public bool CheckCategoryCompatibility(IDamageable entity)
    {
        return (category == Entity.EntityCategory.All || category == entity.GetCategory()) && (terrain == Entity.TerrainType.All || terrain == entity.GetTerrain());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (MasterNetworkAdapter.mode != MasterNetworkAdapter.NetworkMode.Off && NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost) return;

        //TODO: Make this collision avoid hitting the core collider which may mess up the part damage calculation a bit  (for missiles as well)
        var hit = collision.transform.root; // grab collision, get the topmost GameObject of the hierarchy, which would have the craft component
        var craft = hit.GetComponent<IDamageable>(); // check if it has a craft component
        if (collision.GetComponentInParent<Entity>().GetID() != previousID.GetID())
        {
            if (collision.GetComponentInParent<Entity>().GetID() != target.GetComponent<Entity>().GetID())
            {
                if (craft != null && !craft.GetIsDead() && owner) // check if the component was obtained
                {
                    if (!FactionManager.IsAllied(faction, craft.GetFaction()) && CheckCategoryCompatibility(craft) && craft.GetTransform() != owner.GetTransform())
                    {
                        if (MasterNetworkAdapter.mode == MasterNetworkAdapter.NetworkMode.Off || !NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
                        {
                            var residue = craft.TakeShellDamage(damage, pierceFactor, owner); // deal the damage to the target, no shell penetration  

                            // if the shell is low, damage the part
                            ShellPart part = collision.transform.GetComponent<ShellPart>();
                            if (part)
                            {
                                part.TakeDamage(residue); // damage the part
                            }
                        }

                        if (craft is Drone drone && disableDrones)
                        {
                            drone.DisableAITemporarily(Time.time + 3);
                        }
                        previousID = collision.GetComponentInParent<Entity>();
                    }
                }
            }
        }
    }

    public void InstantiateMissPrefab()
    {
        if (missParticles)
        {
            Instantiate(missPrefab, transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg));
        } 
        if (MasterNetworkAdapter.mode != MasterNetworkAdapter.NetworkMode.Off && !MasterNetworkAdapter.lettingServerDecide)
        {
            MasterNetworkAdapter.instance.BulletEffectClientRpc("bullet_miss_prefab",transform.position, vector);
        }
    }

    public void OnDestroy()
    {
        if (transform.GetComponentInChildren<TrailRenderer>())
        {
            transform.GetComponentInChildren<TrailRenderer>().autodestruct = true;
            transform.DetachChildren();
        }
    }

    void Start()
    {
        vector = GetComponent<Rigidbody2D>().velocity;
        GetComponent<SpriteRenderer>().color = particleColor;
    }

    public void StartSurvivalTimer(float time)
    {
        StartCoroutine(DestroyTimer(time));
        StartCoroutine(Particles());
    }

    IEnumerator Particles()
    {

        yield return new WaitForEndOfFrame();
    }
    IEnumerator DestroyTimer(float time)
    {
        yield return new WaitForSeconds(time);

        if (MasterNetworkAdapter.mode == MasterNetworkAdapter.NetworkMode.Off || !MasterNetworkAdapter.lettingServerDecide)
            Destroy(gameObject);
    }
}