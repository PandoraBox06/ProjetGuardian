using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public bool showWaypointsInPlayMode;
    private Transform[] waypoints;
    public Transform[] Waypoints => waypoints;

    private void Awake()
    {
        FetchWaypoints();
    }

    private void FetchWaypoints()
    {
        waypoints = new Transform[transform.childCount];
        var i = 0;
        foreach (Transform child in transform)
        {
            waypoints[i] = child;
            i++;
        }
    }

    private void ShowWaypoints()
    {
        Transform previousWaypoint = waypoints[^1];
        foreach (Transform waypoint in waypoints)
        {
            Debug.DrawLine(previousWaypoint.position, waypoint.position, new Color(1f, 0.44f, 0.21f));
            previousWaypoint = waypoint;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && !showWaypointsInPlayMode) return;
        
        if (waypoints == null || waypoints.Length != transform.childCount)
        {
            FetchWaypoints();
        }
        ShowWaypoints();
    }
}