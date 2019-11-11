using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker : MonoBehaviour
{
	public LayerMask starLayerMask;
	public LayerMask enemyLayerMask;

	public Transform kin;
	public Transform viewfinder;
	
	public Star GetTargetedStar()
	{
		Vector3 kinPosition = kin.position;
		Vector3 viewfinderPosition = viewfinder.position;
		Vector3 relativeViewfinderPosition = viewfinderPosition - kinPosition;
		RaycastHit2D[] hit = Physics2D.RaycastAll(kinPosition, relativeViewfinderPosition, Mathf.Infinity, starLayerMask);

		if (hit.Length == 0) return null;
		if (hit.Length == 1)
		{
			return (Star) hit[0].rigidbody.gameObject.GetComponent(typeof(Star));
		}
		else
		{
			var distances = hit
				.Select(x => (viewfinderPosition - x.transform.position).magnitude)
				.ToArray();
			
			return (Star) hit[Array.IndexOf(distances, distances.Min())].rigidbody.gameObject.GetComponent(typeof(Star));
		}
	}
	
	public Enemy GetTargetedEnemy(Star star)
	{
		Vector2 starPosition = star.transform.position;
		Vector2 viewfinderPosition = viewfinder.position;
		Vector2 relativeViewfinderPosition = viewfinderPosition - starPosition;
		RaycastHit2D hit = Physics2D.Raycast(starPosition, relativeViewfinderPosition, Mathf.Infinity, enemyLayerMask);
		
		if (hit)
		{
			return (Enemy) hit.rigidbody.gameObject.GetComponent(typeof(Enemy));
		}

		return null;
	}
	
}
