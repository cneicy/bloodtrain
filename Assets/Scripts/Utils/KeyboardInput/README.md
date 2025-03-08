<code>KeySettingManager</code>为单例类

在其<code>LoadKeySettings()</code>方法中填写自己需要的键位对即可。

调用<code>GetKey(string actionName)</code>获取行为对应的KeyCode

调用<code>SetKey(string actionName, KeyCode newKeyCode)</code>设置行为对应KeyCode
