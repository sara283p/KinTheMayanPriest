using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker_Joystick : MonoBehaviour
{
	public float selectionDelay = 0.5f;
	private const float MaxSearchRadius = 30f;
	private const float DeadZone = 0.2f;

	public LayerMask obstacleLayerMask;
	
	private bool _selectingWait;

	public LayerMask starLayerMask;
	public LayerMask enemyLayerMask;

	public Rigidbody2D kin;
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
		var viewfinderPosition = (Vector2) viewfinder.position;
		var stars = Physics2D.OverlapCircleAll(viewfinderPosition, 2f, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x =>
			{
				var xPosition = x.GetComponent<Rigidbody2D>().position;
				return !Physics2D.Raycast(kin.position, xPosition - viewfinderPosition,
						(xPosition - viewfinderPosition).magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (viewfinderPosition - x.GetComponent<Rigidbody2D>().position).sqrMagnitude)
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
    		.Select(x => (kin.position - x.GetComponent<Rigidbody2D>().position).magnitude)
    		.ToArray();
    
    	if (stars.Length == 0) return null;
    	return stars[Array.IndexOf(distances, distances.Min())];
    }
	
	public List<Star> GetStarsInRange(float maxStarDistance)
	{
		var kinPosition = kin.position;
		return Physics2D.OverlapCircleAll(kinPosition, maxStarDistance, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.ToList();
	}
	
	public Star GetNearestAvailableStar(float range = MaxSearchRadius)
	{
		var kinPosition = kin.position;
		var stars = Physics2D.OverlapCircleAll(kinPosition, range, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x =>
			{
				var xPosition = x.GetComponent<Rigidbody2D>().position;
				return !Physics2D.Raycast(kin.position, xPosition - kinPosition,
						(xPosition - kinPosition).magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (kinPosition - x.GetComponent<Rigidbody2D>().position).sqrMagnitude)
			.ToArray();

		if (stars.Length == 0)
		{
			return null;
		}
		return stars[0];
	}
	
	public Star GetAvailableStarByRaycast(Transform origin, float range = MaxSearchRadius)
	{
		if (_selectingWait) return null;

		Vector2 originPosition = origin.position;
		var horizontalMove = InputManager.GetAxisRaw("RHorizontal");
		var verticalMove = InputManager.GetAxisRaw("RVertical");

		if (Mathf.Abs(verticalMove) < DeadZone && Mathf.Abs(horizontalMove) < DeadZone) return null;
		
		var direction = new Vector3(horizontalMove, verticalMove);
		
		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var stars = Physics2D.RaycastAll(originPosition, direction, range, starLayerMask)
			.Select(x => x.transform.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x => !Physics2D.Raycast(origin.position, direction, ((Vector2) x.transform.position - originPosition).magnitude, obstacleLayerMask))
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
	/////////////////////////////////      ENEMIES & OBSTACLES      ////////////////////////////////////////////////////
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
			.Select(x => ((Vector2) viewfinder.position - x.GetComponent<Rigidbody2D>().position).magnitude)
			.ToArray();

		return enemy[Array.IndexOf(distances, distances.Min())];
	}
	
	public IDamageable GetNearestAvailableEnemy(Vector2 lastSelectedStar, float range = MaxSearchRadius)
	{
		
		var enemies = Physics2D.OverlapCircleAll(lastSelectedStar, range, enemyLayerMask)
			.Where(x =>
			{
				var position = x.GetComponent<Transform>().position;
				var relativeDirection = (Vector2) position - lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, relativeDirection.magnitude, obstacleLayerMask);
			})
			.OrderBy(x => (lastSelectedStar - (Vector2) x.GetComponent<Transform>().position).sqrMagnitude)
			.Select(x => x.GetComponent<IDamageable>())
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
	public IDamageable GetAvailableEnemyByRaycast(Transform origin, Vector2 lastSelectedStar, float range = MaxSearchRadius)
	{
		if (_selectingWait) return null;

		var originPosition = origin.position;
		var horizontalMove = InputManager.GetAxisRaw("RHorizontal");
		var verticalMove = InputManager.GetAxisRaw("RVertical");
		
		if (Mathf.Abs(verticalMove) < DeadZone && Mathf.Abs(horizontalMove) < DeadZone) return null;
		
		var direction = new Vector3(horizontalMove, verticalMove);

		Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var enemies = Physics2D.RaycastAll(originPosition, direction, Mathf.Infinity, enemyLayerMask)
			.Where(x =>
			{
				Vector2 position = x.transform.position;
				Vector2 relativeDirection = position - (Vector2) lastSelectedStar;
				return !Physics2D.Raycast(lastSelectedStar, relativeDirection, relativeDirection.magnitude, obstacleLayerMask).collider;
			})
			.Where(x => (lastSelectedStar - (Vector2) x.transform.position).magnitude < range)
			.OrderBy(x => (origin.position - x.transform.position).sqrMagnitude)
			.Select(x => x.transform.GetComponent<IDamageable>())
			.ToArray();

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
}
