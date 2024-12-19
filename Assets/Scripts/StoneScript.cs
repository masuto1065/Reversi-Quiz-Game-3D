using System.Collections;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
    // 石のアニメーション（なし、落下、回転）
    public enum AnimationType{None, Drop, Flip}

    /* 石の状態を更新する関数          */
    /* 引数：                          */
    /* stoneState：非表示、白、黒      */
    /* animationType：なし、落下、回転 */
    /* 戻り値：なし                    */
    public void SetState(ReversiScript.stoneState stoneState, AnimationType animationType = AnimationType.None)
    {
        // 石の状態がBlack or WhiteならTrue、NoneならFalse
        var isActive = stoneState != ReversiScript.stoneState.None;

        // アニメーションが回転でなければ、石を高速回転して表示 or 非表示
        if(animationType != AnimationType.Flip)
        {
            if(stoneState == ReversiScript.stoneState.White)  // 石を白向きに高速回転
            {
                gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else if(stoneState == ReversiScript.stoneState.Black)  // 石を黒向きに高速回転
            {
                gameObject.transform.rotation = Quaternion.Euler(270, 0, 0);
            }
        }
        gameObject.SetActive(isActive);
        
        // アニメーション設定
        switch(animationType)
        {
            case AnimationType.Drop:  // 石を置くときに落下させる
                StartCoroutine(DropAnimation());
                break;
            case AnimationType.Flip:  // 石を裏返すときに、ゆっくり回転させる
                StartCoroutine(FlipAnimation(stoneState));
                break;
            case AnimationType.None:  // 何もしない
            default:
                break;
        }
    }

    /* 落下のアニメーションを設定する関数 */
    /* 引数：なし                         */
    /* 戻り値：なし                       */
    private IEnumerator DropAnimation()
    {
        var dropHeight = 1.0f;  // 落下の高さ
        Vector3 currentPosition = gameObject.transform.position;  // 現在の石の位置を取得
        Vector3 startPos = new Vector3(  // 落下開始位置
            currentPosition.x,
            dropHeight,
            currentPosition.z
        );
        Vector3 endPos = new Vector3(  // 落下終了位置
            currentPosition.x,
            0.0f,
            currentPosition.z
        );
        var duration = 0.5f;     // アニメーションにかかる時間
        var elapsedTime = 0.0f;  // アニメーションの経過時間

        // 落下処理
        while(elapsedTime < duration)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 一応、位置を修正
        gameObject.transform.position = endPos;
    }

    /* 回転のアニメーションを設定するコルーチン */
    /* 引数：                                   */
    /* stoneState：石の状態（非表示、黒、白）   */
    /* 戻り値：なし                             */
    private IEnumerator FlipAnimation(ReversiScript.stoneState stoneState)
    {
        var rotationAngleBtoW = 90.0f;   // 黒から白に回転するときの目標角度
        var rotationAngleWtoB = 270.0f;  // 白から黒に回転するときの目標角度
        var duration = 0.5f;            // アニメーションにかかる時間
        var elapsedTime = 0.0f;          // アニメーションの経過時間

        Vector3 startPos = gameObject.transform.position;  // 開始座標
        Vector3 raisedPos = startPos + Vector3.up * 1.0f;  // 浮かせたときの最高到達座標
        
        // 回転処理
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            gameObject.transform.position = Vector3.Lerp(startPos, raisedPos, Mathf.Sin(t * Mathf.PI));
            if(stoneState == ReversiScript.stoneState.White)  // 黒 → 白
            {
                gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(-90.0f, rotationAngleBtoW, t), 0, 0);
            }
            else if(stoneState == ReversiScript.stoneState.Black)  // 白 → 黒
            {
                gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(90.0f, rotationAngleWtoB, t), 0, 0);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 一応、位置と回転を修正
        gameObject.transform.position = startPos;
        if(stoneState == ReversiScript.stoneState.White)
        {
            gameObject.transform.rotation = Quaternion.Euler(rotationAngleBtoW, 0, 0);   
        }
        else if(stoneState == ReversiScript.stoneState.Black)
        {
            gameObject.transform.rotation = Quaternion.Euler(rotationAngleWtoB, 0, 0);   
        }  
    }
}