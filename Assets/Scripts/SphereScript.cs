using UnityEngine;

public class SphereScript : MonoBehaviour
{
    /* そのマスに石を置くことが出来るならば、Sphereを表示する関数 */
    /* 引数：                                                     */
    /* canPutState：置けない、置ける                              */
    /* 戻り値：なし                                               */
    public void ShowAssist(ReversiScript.canPutState canPutState)
    {
        var isActive = canPutState != ReversiScript.canPutState.Cannot;
        gameObject.SetActive(isActive);
    }

    /* アシストSphereをプレイヤーターンの色に変更する関数 */
    /* 引数：                                             */
    /* playerTurn：プレイヤーターン                       */
    /* 戻り値：なし                                       */
    public void FlipSphere(ReversiScript.stoneState playerTurn)
    {
        if(playerTurn == ReversiScript.stoneState.White)                  // Sphereを白向きに回転
        {
            gameObject.transform.rotation = Quaternion.Euler(270, 0, 0);
        }
        else if(playerTurn == ReversiScript.stoneState.Black)             // Sphereを黒向きに回転
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
