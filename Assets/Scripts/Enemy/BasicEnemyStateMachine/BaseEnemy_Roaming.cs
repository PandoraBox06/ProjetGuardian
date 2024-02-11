using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_Roaming : BaseEnemy_BaseState
    {
        private int targetWaypointId;
        private Transform TargetWaypoint;

        public override void EnterState(BaseEnemy_StateManager state)
        {
            if (state.roamingPoints.Length == 0) state.SwitchState(state.IdleState);

            if (!state.randomRoaming)
            {
                targetWaypointId = GetClosestWaypointId(state.transform.position, state);
                TargetWaypoint = state.roamingPoints[targetWaypointId];
                state.agent.SetDestination(TargetWaypoint.position);
            }
            else
            {
                targetWaypointId = GetRandomWaypointId(state);
                TargetWaypoint = state.roamingPoints[targetWaypointId];
                state.agent.SetDestination(TargetWaypoint.position);
            }

        }
        public override void UpdateState(BaseEnemy_StateManager state)
        {
            Debug.DrawLine(state.transform.position, TargetWaypoint.transform.position);
            if (state.agent.remainingDistance < 0.25f)
            {
                ChangeTargetWaypoint(state);
            }

            if(state.player != null) { state.SwitchState(state.TrackPlayerState); }
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

        private void ChangeTargetWaypoint(BaseEnemy_StateManager state)
        {
            targetWaypointId = IncrementLoop(targetWaypointId, state);
            TargetWaypoint = state.roamingPoints[targetWaypointId];
            state.agent.SetDestination(TargetWaypoint.position);
        }

        private int IncrementLoop(int id, BaseEnemy_StateManager state)
        {
            int dir = state.inverseDirection ? -1 : 1;
            return (int)Mathf.Repeat(id + dir, state.roamingPoints.Length);
        }

        private Transform GetClosestWaypoint(Vector3 position, BaseEnemy_StateManager state)
        {
            float closestDistance = Mathf.Infinity;
            Transform closestWaypoint = null;
            for (var i = 0; i < state.roamingPoints.Length; i++)
            {
                float dist = Vector3.Distance(position, state.roamingPoints[i].position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestWaypoint = state.roamingPoints[i];
                }
            }

            return closestWaypoint;
        }

        private int GetClosestWaypointId(Vector3 position, BaseEnemy_StateManager state)
        {
            float closestDistance = Mathf.Infinity;
            int closestWaypointId = 0;
            for (var i = 0; i < state.roamingPoints.Length; i++)
            {
                float dist = Vector3.Distance(position, state.roamingPoints[i].position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestWaypointId = i;
                }
            }

            return closestWaypointId;
        }

        private int GetRandomWaypointId(BaseEnemy_StateManager state, bool ignorePrevious = true)
        {
            int rd = Random.Range(0, state.roamingPoints.Length);
            if (ignorePrevious && rd == targetWaypointId)
            {
                rd = IncrementLoop(rd, state);
            }

            return rd;
        }
    }
}