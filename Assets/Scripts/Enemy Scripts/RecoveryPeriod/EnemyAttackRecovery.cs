using UnityEngine;
using System;
using System.Collections;

public class EnemyAttackRecovery : MonoBehaviour
{
    public bool IsBusy { get; private set; }

    public bool TryBeginAttack(
        float attackDuration,
        float recoveryDuration,
        Action onRecoveryComplete = null)
    {
        if (IsBusy)
            return false;

        StartCoroutine(AttackSequence(
            attackDuration,
            recoveryDuration,
            onRecoveryComplete
        ));

        return true;
    }

    private IEnumerator AttackSequence(
        float attackDuration,
        float recoveryDuration,
        Action onRecoveryComplete)
    {
        IsBusy = true;
        //Debug.Log("Enemy executing attack.");

        yield return new WaitForSeconds(
            Mathf.Max(0f, attackDuration)
        );

        if (recoveryDuration > 0f)
        {
            //Debug.Log("Enemy entered recovery.");

            yield return new WaitForSeconds(
                recoveryDuration
            );
        }

        IsBusy = false;

        //Debug.Log("Enemy ready for next action.");

        onRecoveryComplete?.Invoke();
    }
}