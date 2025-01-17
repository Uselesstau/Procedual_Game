using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public GameObject bulletMark;
    public float speed;
    public float damage;

	public bool isplayers;
	public GameObject shooter;
	private void Start()
	{
		if (GetComponent<CapsuleCollider>() != null)
		{
			Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), shooter.GetComponent<CapsuleCollider>());
		}
		if (GetComponent<SphereCollider>() != null)
		{
			Physics.IgnoreCollision(GetComponent<SphereCollider>(), shooter.GetComponent<CapsuleCollider>());
		}
		Physics.IgnoreLayerCollision(9, 9);
	}

	void Update()
    {
		GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward * speed);
    }
	private void OnCollisionEnter(Collision collision)
	{
		if (shooter == null)
		{
			return;
		}
		if (collision.gameObject.CompareTag("Enemy") && isplayers)
		{
			if (collision.gameObject.GetComponent<EyesBehaviour>() != null)
			{
				collision.gameObject.GetComponent<EyesBehaviour>().runAway = damage * 5;
			}
			if (collision.gameObject.GetComponent<Automaton_Behaviour>() != null)
			{
				collision.gameObject.GetComponent<Automaton_Behaviour>().hp -= damage;
			}
			if (collision.gameObject.GetComponentInParent<Bug_Cs>() != null)
			{
				collision.gameObject.GetComponentInParent<Bug_Cs>().hp -= damage;
			}
			Destroy(gameObject);
			return;
		}
		if (collision.gameObject.CompareTag("Player") && !isplayers)
		{
			collision.gameObject.GetComponentInParent<PlayerScript>().oxygenLeft -= damage * 2;
			Destroy(gameObject);
			return;
		}
		Instantiate(bulletMark, transform.position, Quaternion.FromToRotation(transform.up, collision.transform.forward) * transform.rotation);
		Destroy(gameObject);
	}
}
