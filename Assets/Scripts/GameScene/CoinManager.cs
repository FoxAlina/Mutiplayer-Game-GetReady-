using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private float horizontalRadius;
    [SerializeField] private float verticalRadius;
    private ObjectPool coinPool;
    private float coinNumber;
    [SerializeField] private float coinHeight;
    [SerializeField] private float coinRate;

    void Start()
    {
        coinPool = GetComponent<ObjectPool>();
        coinNumber = coinPool.SharedInstance.amountToPool;

        for (int i = 0; i < coinNumber; i++)
        {
            GameObject tmp = coinPool.SharedInstance.GetPooledObject();
            if (tmp)
            {
                float y = Random.Range(-verticalRadius, verticalRadius);
                float x = Random.Range(-horizontalRadius, horizontalRadius);

                tmp.transform.position = new Vector3(x, y, coinHeight);
                tmp.SetActive(true);
            }
        }
    }

    bool reload = true;

    private void Update()
    {
        if (coinPool.SharedInstance.ActiveObjectsAmount() < coinNumber)
        {
            if (reload) StartReloadCoroutine();
        }
    }

    public void StartReloadCoroutine()
    {
        reload = false;
        StartCoroutine(reloadCoroutine());
    }

    IEnumerator reloadCoroutine()
    {
        yield return new WaitForSeconds(coinRate);
        
        GameObject tmp = coinPool.SharedInstance.GetPooledObject();
        if (tmp)
        {
            float y = Random.Range(-verticalRadius, verticalRadius);
            float x = Random.Range(-horizontalRadius, horizontalRadius);

            tmp.transform.position = new Vector3(x, y, coinHeight);
            tmp.SetActive(true);
        }

        reload = true;
    }
}
