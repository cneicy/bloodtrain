新建一个对象池类继承`SingletonObjectPool<T>`后，在UnityEditor中会自动创建以类名为名称的单例实例。
```csharp
public class EnemyPool : SingletonObjectPool<Enemy>
{
}
```
用法示例如下方刷怪圈
```csharp
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int initialPoolSize = 5;
    public int maxPoolSize = 10;
    public float spawnInterval = 3f;
    public int spawnCount = 1;
    public float spawnRadius = 5f;

    private void Start()
    {
        SingletonObjectPool<Enemy>.Instance.Init(enemyPrefab.GetComponent<Enemy>(), initialPoolSize, maxPoolSize);
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        for (var i = 0; i < spawnCount; i++)
        {
            var spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            SingletonObjectPool<Enemy>.Instance.Spawn(transform).transform.position = spawnPosition;
        }
    }
}
```
