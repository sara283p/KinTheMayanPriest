using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker_Joystick : MonoBehaviour
{
	public float selectionDelay = 0.4f;
	public LayerMask obstacleLayerMask;
	
	private bool _selectingWait;
	
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
	
	public Star GetNearestStar()
	{
		Star[] stars = FindObjectsOfType<Star>();
		var distances = stars
			.Select(x => (kin.position - x.transform.position).magnitude)
			.ToArray();

		if (stars.Length == 0) return null;
		return stars[Array.IndexOf(distances, distances.Min())];
	}

	public Star GetNearestAvailableStar()
	{
		Star[] stars = FindObjectsOfType<Star>();
		stars = stars.Where(x => !x.isInCooldown).ToArray();
		var distances = stars
			.Select(x => (kin.position - x.transform.position).magnitude)
			.ToArray();

		if (stars.Length == 0) return null;
		
		var selectedStar = stars[Array.IndexOf(distances, distances.Min())];
		Vector2 relativePosition = selectedStar.transform.position - kin.position;
		RaycastHit2D hit = Physics2D.Raycast(kin.position, relativePosition, relativePosition.magnitude, obstacleLayerMask);
		if (hit.collider)
			return null;
		
		return selectedStar;
	}

	public Star GetAvailableStarByRaycast(Transform origin)
	{
		if (_selectingWait) return null;
		
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);
		
		RaycastHit2D[] hit = Physics2D.RaycastAll(origin.position, direction, Mathf.Infinity, starLayerMask);
        Debug.DrawRay(origin.position, direction * 5.0f, Color.green);

        if (hit.Length > 0)
        {
	        var stars = hit
		        .Select(x => x.transform.GetComponent<Star>())
		        .Where(x => !x.isInCooldown)
		        .ToArray();
	        if (stars.Length > 0)
	        {
		        StartCoroutine(SelectionWait());
		        var selectedStar = stars[0];
		        Vector2 relativePosition = selectedStar.transform.position - origin.position;
		        RaycastHit2D groundHit = Physics2D.Raycast(origin.position, direction, relativePosition.magnitude, obstacleLayerMask);
		        if (groundHit.collider)
		        {
			        return null;
		        }
		        return selectedStar;
	        }
        }

        return null;
	}
	
	public Enemy GetEnemyByRaycast()
	{
		if (_selectingWait) return null;
		
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);
		
		RaycastHit2D hit = Physics2D.Raycast(viewfinder.position, direction, Mathf.Infinity, enemyLayerMask);
		Debug.DrawRay(viewfinder.position, direction * 5.0f, Color.green);

		if (hit)
		{
			StartCoroutine(SelectionWait());
			return hit.transform.GetComponent<Enemy>();
		}
		else
		{
			return null;
		}

	}
	
	public Enemy GetNearestEnemy()
	{
		Enemy[] enemy = FindObjectsOfType<Enemy>();
		print(enemy.Length);
		var distances = enemy
			.Select(x => (viewfinder.position - x.transform.position).magnitude)
			.ToArray();

		return enemy[Array.IndexOf(distances, distances.Min())];
	}

	public List<Star> GetStarsInRange(float maxStarDistance)
	{
		return FindObjectsOfType<Star>()
			.Where(x => !x.isInCooldown)
			.Where(x => (x.transform.position - kin.position).magnitude < maxStarDistance)
			.ToList();
	}
	
	IEnumerator SelectionWait()
	{
		_selectingWait = true;
		yield return new WaitForSeconds(selectionDelay);
		_selectingWait = false;
	}
	
}
