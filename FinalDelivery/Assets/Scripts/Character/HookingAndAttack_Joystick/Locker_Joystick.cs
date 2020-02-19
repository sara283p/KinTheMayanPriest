using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Locker_Joystick : MonoBehaviour
{
	public float selectionDelay = 0.5f;
	private const float DeadZone = 0.2f;

	public LayerMask obstacleLayerMask;
	
	private bool _selectingWait;

	public LayerMask starLayerMask;
	public LayerMask enemyLayerMask;

	public Rigidbody2D kin;
	public Transform viewfinder;
	public LineRenderer selectEffect;

	private RaycastHit2D[] dummyRaycastHit2Ds = new RaycastHit2D[5];
	
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
	
	public Star GetNearestAvailableStar(float range)
	{
		Vector2 kinPosition = kin.position;
		var stars = Physics2D.OverlapCircleAll(kinPosition, range, starLayerMask)
			.Select(x => x.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x =>
			{
				var xPosition = x.GetComponent<Rigidbody2D>().position;
				return !Physics2D.Raycast(kinPosition, xPosition - kinPosition,
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
	
	public Star GetAvailableStarByRaycast(Transform origin, float range)
	{
		if (_selectingWait) return null;

		Vector2 originPosition = origin.position;
		var horizontalMove = InputManager.GetAxisRaw("RHorizontal");
		var verticalMove = InputManager.GetAxisRaw("RVertical");

		//if (Mathf.Abs(verticalMove) < DeadZone && Mathf.Abs(horizontalMove) < DeadZone) return null;
		
		var direction = new Vector2(horizontalMove, verticalMove);
		var magnitude = direction.magnitude;
		var endPoint = originPosition + (range * magnitude * direction.normalized);
		//print(direction.magnitude);
		//print(direction.normalized);
		selectEffect.positionCount = 2;
		selectEffect.SetPosition(0, originPosition);
		selectEffect.SetPosition(1, endPoint);
		
		//Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var stars = Physics2D.RaycastAll(originPosition, direction, magnitude*range, starLayerMask)
			.Select(x => x.transform.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x => !Physics2D.Raycast(originPosition, direction, ((Vector2) x.transform.position - originPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (originPosition - (Vector2) x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length > 0)
		{
			selectEffect.positionCount = 0;
			StartCoroutine(SelectionWait());
			return stars[0];
		}

		return null;
	}
	
	public Star GetFirstStarByRaycast(Transform origin, float range, Transform kin)
	{
		if (_selectingWait) return null;

		Vector2 originPosition = origin.position;
		Vector2 kinPosition = kin.position;
		var horizontalMove = InputManager.GetAxisRaw("RHorizontal");
		var verticalMove = InputManager.GetAxisRaw("RVertical");

		//if (Mathf.Abs(verticalMove) < DeadZone && Mathf.Abs(horizontalMove) < DeadZone) return null;
		
		var direction = new Vector2(horizontalMove, verticalMove);
		var magnitude = direction.magnitude;
		var endPoint = originPosition + (range * magnitude * direction.normalized);
		//print(direction.magnitude);
		//print(direction.normalized);
		selectEffect.positionCount = 2;
		selectEffect.SetPosition(0, originPosition);
		selectEffect.SetPosition(1, endPoint);
		
		//Debug.DrawRay(originPosition, direction * 5.0f, Color.green);
		
		var stars = Physics2D.RaycastAll(originPosition, direction, magnitude*range, starLayerMask)
			.Select(x => x.transform.GetComponent<Star>())
			.Where(x => !x.isDisabled)
			.Where(x => !Physics2D.Raycast(kinPosition, ((Vector2) x.transform.position - kinPosition), ((Vector2) x.transform.position - kinPosition).magnitude, obstacleLayerMask))
			.OrderBy(x => (originPosition - (Vector2) x.transform.position).sqrMagnitude)
			.ToArray();

		if (stars.Length > 0)
		{
			selectEffect.positionCount = 0;
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
	
	public IDamageable GetNearestAvailableEnemy(Vector2 lastSelectedStar, float range)
	{
		var enemies = Physics2D.OverlapCircleAll(lastSelectedStar, range, enemyLayerMask)
			.Where(x =>
			{
				Vector2 position = x.GetComponent<IDamageable>().GetPosition();
				var relativeDirection = position - lastSelectedStar;
				var size = Physics2D.RaycastNonAlloc(lastSelectedStar, relativeDirection, dummyRaycastHit2Ds, relativeDirection.magnitude, obstacleLayerMask);
				Debug.DrawRay(lastSelectedStar, relativeDirection.normalized * (relativeDirection.magnitude), Color.green);
				return size < 1 || (size == 1 && x == dummyRaycastHit2Ds[0].collider);
			})
			.Select(x => x.GetComponent<IDamageable>())
			.OrderBy(x => (lastSelectedStar -  x.GetPosition()).sqrMagnitude)
			.ToArray();
		

		if (enemies.Length > 0)
		{
			StartCoroutine(SelectionWait());
			return enemies[0];
		}

		return null;
	}
	
	public IDamageable GetAvailableEnemyByRaycast(Transform targetEnemy, Vector2 lastSelectedStar, float range)
	{
		if (_selectingWait) return null;

		Vector2 viewfinderPosition = viewfinder.transform.position;
		var horizontalMove = InputManager.GetAxisRaw("RHorizontal");
		var verticalMove = InputManager.GetAxisRaw("RVertical");
		
		// if (Mathf.Abs(verticalMove) < DeadZone && Mathf.Abs(horizontalMove) < DeadZone) return null;
		
		var direction = new Vector2(horizontalMove, verticalMove);
		var magnitude = direction.magnitude;
		var endPoint = viewfinderPosition + (range * magnitude * direction.normalized);

		selectEffect.positionCount = 2;
		selectEffect.SetPosition(0, viewfinderPosition);
		selectEffect.SetPosition(1, endPoint);

		var enemies = Physics2D.RaycastAll(viewfinderPosition, direction, magnitude*range, enemyLayerMask)
			.Where(x => x.transform != targetEnemy)
			.Where(x =>
			{
				Vector2 position = x.transform.GetComponent<IDamageable>().GetPosition();
				var relativeDirection = position - lastSelectedStar;
				var size = Physics2D.RaycastNonAlloc(lastSelectedStar, relativeDirection, dummyRaycastHit2Ds, relativeDirection.magnitude, obstacleLayerMask);
				Debug.DrawRay(lastSelectedStar, relativeDirection.normalized * (relativeDirection.magnitude), Color.green);
				return size < 1 || (size == 1 && x.collider == dummyRaycastHit2Ds[0].collider);
			})
			.Select(x => x.transform.GetComponent<IDamageable>())
			.Where(x => (lastSelectedStar - x.GetPosition()).magnitude < range)
			.OrderBy(x => (viewfinderPosition - x.GetPosition()).sqrMagnitude)
			.ToArray();

		if (enemies.Length > 0)
		{
			selectEffect.positionCount = 0;
			StartCoroutine(SelectionWait());
			return enemies[0];
		}
		
		return null;
	}
	
}
