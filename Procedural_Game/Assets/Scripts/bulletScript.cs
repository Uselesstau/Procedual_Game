using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
	public GameObject eyes;
    public GameObject bulletMark;
    public float speed;

	public bool isplayers;
	public GameObject shooter;
	private void Start()
	{
		Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), shooter.GetComponent<CapsuleCollider>());
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
				collision.gameObject.GetComponent<EyesBehaviour>().runAway = 10;
			}
			if (collision.gameObject.GetComponent<Automaton_Behaviour>() != null)
			{
				collision.gameObject.GetComponent<Automaton_Behaviour>().hp -= 1;
			}
			if (collision.gameObject.GetComponentInParent<Bug_Cs>() != null)
			{
				collision.gameObject.GetComponentInParent<Bug_Cs>().hp -= 1;
			}
			Destroy(gameObject);
			return;
		}
		if (collision.gameObject.CompareTag("Player") && !isplayers)
		{
			collision.gameObject.GetComponentInParent<PlayerScript>().oxygenLeft -= 2;
			Destroy(gameObject);
			return;
		}
		Instantiate(bulletMark, transform.position, Quaternion.FromToRotation(transform.up, collision.transform.forward) * transform.rotation);
		Destroy(gameObject);
	}
}
