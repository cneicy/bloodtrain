# AnnoyingUtils
随便写的轮子，不优雅但能用

Github: https://github.com/CrashWork-Dev/AnnoyingUtils

开源许可证: [Unlicense license](https://github.com/CrashWork-Dev/AnnoyingUtils?tab=Unlicense-1-ov-file#)

关联项目: https://github.com/cneicy/Rouge

用法：下载后直接解压到Assets/Scripts下。

目前拥有键盘键位自定义、自定义相机比例、对象池功能。

如果你有更好的解决方案或者有功能需求，欢迎你在[评论区](https://forum.crash.work/d/54)或[Github Issues](https://github.com/CrashWork-Dev/AnnoyingUtils/issues)留言。

###  键盘键位自定义
`KeySettingManager`为单例类

在`KeySettingManager`的`LoadKeySettings()`中填写自己需要的键位对即可。

调用`GetKey(string actionName)`获取行为对应的`KeyCode`

调用`SetKey(string actionName, KeyCode newKeyCode)`设置行为对应`KeyCode`

###  相机比例自定义
将[ScreenAspect.cs](https://github.com/CrashWork-Dev/AnnoyingUtils/blob/main/ScreenAspect/ScreenAspect.cs "ScreenAspect.cs")挂载相机上即可生效

`public float TargetAspect = 16f / 9f;`

修改以上数值即可设置相机比例。

###  对象池
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
