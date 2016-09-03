using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;


public class Projectile : MonoBehaviour
{
    public float speed = 10;
    public float bounceFactor = 0.5f;
    float threshold = 0.1f;
    bool bounce = false;

    Vector3 targetPos;
    Vector3 direction;
    private float distanceToTarget;
    private bool move = false;

    private Vector3 originPos;
    private Vector3 originRot;

    public bool Move
    {
        get
        {
            return move;
        }
    }

    event Action PorjectileFinishEvent;

    void Start()
    {
        originPos = transform.localPosition;
        originRot = transform.localRotation.eulerAngles;
        StartCoroutine(Shoot());
    }



    IEnumerator Shoot()
    {
        while (true)
        {
            if (!move)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            this.transform.LookAt(targetPos);
            float angle = Mathf.Min(1, Vector3.Distance(this.transform.position, targetPos) / distanceToTarget) * 45;
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
            float currentDist = Vector3.Distance(this.transform.position, targetPos);
            if (currentDist < threshold)
            {
                //if (!bounce)
                //{
                //    bounce = true;
                //    targetPos = targetPos + direction * distanceToTarget * bounceFactor;
                //    distanceToTarget = Vector3.Distance(this.transform.position, targetPos);
                //}
                //else
                {
                    move = false;
                    if (PorjectileFinishEvent != null)
                    {
                        PorjectileFinishEvent();
                    }
                }
            }
            this.transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
            yield return null;
        }
    }

    public void ResetTransform()
    {
        transform.localPosition = originPos;
        transform.localRotation = Quaternion.Euler(originRot);
    }

    /// <summary>
    /// start play
    /// </summary>
    /// <param name="targetPosiotn"> target position in worldspace </param>
    public void StartProjectile(Vector3 targetPosiotn, Action OnFinish)
    {
        bounce = false;

        targetPos = targetPosiotn;
        direction = Vector3.Normalize(targetPos - transform.position);
        PorjectileFinishEvent = OnFinish;
        distanceToTarget = Vector3.Distance(this.transform.position, targetPos);
        move = true;
    }



}
