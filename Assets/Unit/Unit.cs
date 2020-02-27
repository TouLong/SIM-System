using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    Transform target;
    Vector3 targetPos;
    float stopDistance;
    Action onArrived, onActionCompleted;
    Func<bool> actionRepeat;
    readonly float turnSpeed = 240f;
    Action action;

    string actionAnim, moveAnim;
    UnitAnimatorController animator;
    NavMeshAgent navMeshAgent;
    protected void Awake()
    {
        animator = new UnitAnimatorController(GetComponent<Animator>());
        navMeshAgent = GetComponent<NavMeshAgent>();
        action = Idle;
        targetPos = transform.position;
    }
    protected void Update()
    {
        animator.UpdateState();
        action();
    }
    public void MoveTo(Vector3 position, float stopDist, string moveAnim, Action onArrived)
    {
        this.target = null;
        targetPos = position;
        if (Vector3.Distance(transform.position, targetPos) > stopDist)
        {
            action = Moving;
            this.moveAnim = moveAnim;
            animator.Play(this.moveAnim);
            navMeshAgent.isStopped = false;
            this.stopDistance = stopDist;
            this.onArrived = onArrived;
        }
        else
        {
            action = Idle;
            onArrived?.Invoke();
            this.onArrived = null;
        }
    }
    public void MoveTo(Transform target, float stopDist, string moveAnim, Action onArrived)
    {
        this.target = target;
        MoveTo(target.position, stopDist, moveAnim, onArrived);
    }
    public void Action(string actionAnim, float actionTime, Action onAction, Action onCompleted)
    {
        if (onAction == null)
            animator.Play(actionAnim);
        else
            animator.Play(actionAnim, actionTime, onAction);
        this.actionAnim = actionAnim;
        action = Doing;
        onActionCompleted = onCompleted;
    }
    public void Action(string actionAnim, Action onCompleted)
    {
        Action(actionAnim, 0, null, onCompleted);
    }
    public void ActionPeriod(string actionAnim, Func<bool> repeat, Action onCompleted)
    {
        actionRepeat = repeat;
        Action(actionAnim, onCompleted);
    }
    public void MoveAndAction(Transform target, float stopDist, string moveAnim, string actionAnim, Action onCompleted)
    {
        MoveTo(target, stopDist, moveAnim, () => { Action(actionAnim, onCompleted); });
    }
    public void MoveAndActionPeriod(Transform target, float stopDist, string moveAnim, string actionAnim, Func<bool> repeat, Action onCompleted)
    {
        MoveTo(target, stopDist, moveAnim, () => { ActionPeriod(actionAnim, repeat, onCompleted); });
    }
    public void StopAction()
    {
        action = Idle;
        animator.Play(UnitAnim.Idle);
        actionAnim = "";
        moveAnim = "";
        actionRepeat = null;
        onActionCompleted = null;
        onArrived = null;
        navMeshAgent.SetDestination(transform.position);
    }
    void Idle()
    {
        if (Vector3.Distance(new Vector3(targetPos.x, transform.position.y, targetPos.z), transform.position) <= 0.1f)
            targetPos = V3Random.DirectionXZ() * UnityEngine.Random.Range(3, 10) + transform.position;
        else
            navMeshAgent.SetDestination(targetPos);
        RotateToTarget();
    }
    void Moving()
    {
        if (target != null)
            targetPos = target.position;
        if (Vector3.Distance(new Vector3(targetPos.x, transform.position.y, targetPos.z), transform.position) <= stopDistance)
        {
            navMeshAgent.SetDestination(transform.position);
            if (RotateToTarget())
            {
                action = Idle;
                onArrived();
                onArrived = null;
            }
        }
        else
            navMeshAgent.SetDestination(targetPos);
    }
    void Doing()
    {
        if (animator.IsCompleted)
        {
            if (actionRepeat?.Invoke() == true)
            {
                animator.Play(actionAnim);
            }
            else
            {
                action = Idle;
                onActionCompleted?.Invoke();
                onActionCompleted = null;
                actionRepeat = null;
            }
        }
    }
    bool RotateToTarget()
    {
        Vector3 dir = (targetPos - transform.position);
        dir.y = 0;
        dir.Normalize();
        if ((dir - transform.forward).normalized.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
            return false;
        }
        else
            return true;
    }
}

public class UnitAnimatorController
{
    enum State
    {
        Ready,
        Playing,
        Completed,
    }

    readonly Animator animator;
    readonly int index;
    State state;
    Action animationEvent;
    float eventTime;
    public UnitAnimatorController(Animator animator, int index = 0)
    {
        this.animator = animator;
        this.index = index;
    }

    public bool IsCompleted => state == State.Completed;

    public void Play(string name)
    {
        animator.enabled = true;
        animator.CrossFade(name, 0.1f);
        state = State.Ready;
    }
    public void Play(string name, float time, Action action)
    {
        Play(name);
        animationEvent = action;
        eventTime = time;
    }
    public void UpdateState()
    {
        if (state == State.Ready)
        {
            if (animator.GetCurrentAnimatorStateInfo(index).normalizedTime < 1)
            {
                state = State.Playing;
            }
        }
        else if (state == State.Playing)
        {
            float normalizedTime = animator.GetCurrentAnimatorStateInfo(index).normalizedTime;
            if (animationEvent != null)
            {
                if (normalizedTime >= eventTime)
                {
                    animationEvent();
                    animationEvent = null;
                    eventTime = 0;
                }
            }
            else if (normalizedTime > 1)
            {
                state = State.Completed;
                animationEvent = null;
                eventTime = 0;
            }
        }
    }
}
