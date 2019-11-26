using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker_Joystick : MonoBehaviour
{
	public float selectionDelay = 0.5f;
	public LayerMask obstacleLayerMask;
	
	private bool _selectingWait;
	private float _maxDistance = 5;
	
	public LayerMask starLayerMask;
	public LayerMask enemyLayerMask;

	public Transform kin;
	public Transform viewfinder;
	
	IEnumerator SelectionWait()
	{
		_selectingWait = true;
		yield return new WaitForSeconds(selectionDelay);
		_selectingWait = false;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////            STARS            //////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public Star GetTargetedStar()
	{
		var viewfinderPosition = this.viewfinder.position;
		var stars = Physics2D.OverlapCircleAll(viewfinderPosition, 50f, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.Where(x => !Physics2D.Raycast(kin.position, x.transform.position - viewfinderPosition, (x.transform.position - viewfinderPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (viewfinderPosition - x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length == 0)
		{
			return null;
		}
		return stars[0];
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
	
	public List<Star> GetStarsInRange(float maxStarDistance)
	{
		var kinPosition = kin.position;
		return Physics2D.OverlapCircleAll(kinPosition, maxStarDistance, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.ToList();
	}
	
	public Star GetNearestAvailableStar()
	{
		var kinPosition = kin.position;
		var stars = Physics2D.OverlapCircleAll(kinPosition, 50f, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.Where(x => !Physics2D.Raycast(kin.position, x.transform.position - kinPosition, (x.transform.position - kinPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (kinPosition - x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length == 0)
		{
			return null;
		}
		return stars[0];
	}
	
	public Star GetNearestAvailableStarInRange(float range)
	{
		var kinPosition = kin.position;
		var stars = Physics2D.OverlapCircleAll(kinPosition, range, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.Where(x => !Physics2D.Raycast(kin.position, x.transform.position - kinPosition, (x.transform.position - kinPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (kinPosition - x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length == 0)
		{
			return null;
		}
		return stars[0];
	}
	
	public Star GetAvailableStarByRaycast(Transform origin)
	{
		if (_selectingWait) return null;

		var originPosition = origin.position;
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);
		
		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var stars = Physics2D.RaycastAll(originPosition, direction, Mathf.Infinity, starLayerMask)
			.Select(x => x.transform.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.Where(x => !Physics2D.Raycast(origin.position, direction, (x.transform.position - originPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (origin.position - x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return stars[0];
		}

		return null;
	}
	
	public Star GetAvailableStarByRaycastInRange(Transform origin, float range)
	{
		if (_selectingWait) return null;

		var originPosition = origin.position;
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);
		
		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var stars = Physics2D.RaycastAll(originPosition, direction, range, starLayerMask)
			.Select(x => x.transform.GetComponent<Star>())
			.Where(x => !x.isInCooldown)
			.Where(x => !Physics2D.Raycast(origin.position, direction, (x.transform.position - originPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (origin.position - x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return stars[0];
		}

		return null;
	}
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////            ENEMIES            ////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
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
	
	public Enemy GetNearestEnemy()
	{
		Enemy[] enemy = FindObjectsOfType<Enemy>();
		print(enemy.Length);
		var distances = enemy
			.Select(x => (viewfinder.position - x.transform.position).magnitude)
			.ToArray();

		return enemy[Array.IndexOf(distances, distances.Min())];
	}
	
	public Enemy GetNearestAvailableEnemy(Vector3 lastSelectedStar)
	{
		var enemies = Physics2D.OverlapCircleAll(lastSelectedStar, 50f, enemyLayerMask)
			.Select(x => x.transform.GetComponent<Enemy>())
			.Where(x =>
			{
				var position = x.transform.position;
				var relativeDirection = x.transform.position - lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, (position - lastSelectedStar).magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (lastSelectedStar - x.transform.position).sqrMagnitude)
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
	public Enemy GetNearestAvailableEnemyInRange(Vector3 lastSelectedStar, float range)
	{
		var enemies = Physics2D.OverlapCircleAll(lastSelectedStar, range, enemyLayerMask)
			.Select(x => x.transform.GetComponent<Enemy>())
			.Where(x =>
			{
				var position = x.transform.position;
				var relativeDirection = x.transform.position - lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, (position - lastSelectedStar).magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (lastSelectedStar - x.transform.position).sqrMagnitude)
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}

	public Enemy GetAvailableEnemyByRaycast(Transform origin, Vector3 lastSelectedStar)
	{
		if (_selectingWait) return null;

		var originPosition = origin.position;
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);

		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var enemies = Physics2D.RaycastAll(originPosition, direction, Mathf.Infinity, enemyLayerMask)
			.Select(x => x.transform.GetComponent<Enemy>())
			.Where(x =>
			{
				var position = x.transform.position;
				var relativeDirection = x.transform.position - lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, (position - lastSelectedStar).magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (origin.position - x.transform.position).sqrMagnitude)
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
	public Enemy GetAvailableEnemyByRaycastInRange(Transform origin, Vector3 lastSelectedStar, float range)
	{
		if (_selectingWait) return null;

		var originPosition = origin.position;
		var horizontalMove = Input.GetAxisRaw("StarViewfinderHorizontal");
		var verticalMove = Input.GetAxisRaw("StarViewfinderVertical");
		var direction = new Vector3(horizontalMove, verticalMove);

		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var enemies = Physics2D.RaycastAll(originPosition, direction, Mathf.Infinity, enemyLayerMask)
			.Select(x => x.transform.GetComponent<Enemy>())
			.Where(x =>
			{
				var position = x.transform.position;
				var relativeDirection = x.transform.position - lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, (position - lastSelectedStar).magnitude, obstacleLayerMask);
			})
			.Where(x => (lastSelectedStar - x.transform.position).magnitude < range)
			.OrderBy(x => (origin.position - x.transform.position).sqrMagnitude)
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
}
