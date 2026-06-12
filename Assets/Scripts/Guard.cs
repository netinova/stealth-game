using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Guard : MonoBehaviour
{
    public static event Action onGuardHasSpottedPlayer;

    public Transform pathHolder;
    public Light spotLight;
    public float viewDistance;
    public float timeToSpotPlayer = .5f;

    float viewAngle;
    float playerVisibleTimer;

    Transform player;
    Color originalSpotlightColor;

    public float speed = 5;
    public float waitTime = .3f;
    public float turnSpeed = 90f;
    public LayerMask viewMask;

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    void Start()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));

        viewAngle = spotLight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalSpotlightColor = spotLight.color;
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > viewDistance)
        {
            return false;
        }

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        if (angleBetweenGuardAndPlayer > viewAngle / 2f)
        {
            return false;
        }

        if (Physics.Linecast(transform.position, player.position, viewMask))
        {
            return false;
        }

        return true;
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.y,
                targetAngle,
                turnSpeed * Time.deltaTime
            );

            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWaypoint,
                speed * Time.deltaTime
            );

            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp01(playerVisibleTimer);
        spotLight.color = Color.Lerp(
            originalSpotlightColor,
            Color.red,
            playerVisibleTimer / timeToSpotPlayer
        );

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (onGuardHasSpottedPlayer != null)
            {
                onGuardHasSpottedPlayer();
            }
        }
    }
}
