using System;
using System.Collections.Generic;
using System.Reflection;
using Handler;
using UnityEngine;
using Utils;

namespace Manager
{
    /// <summary>
    /// 全局事件管理系统（单例模式）
    /// <para>支持泛型事件参数和特性驱动的事件订阅</para>
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        // 使用泛型事件字典来存储事件和处理程序
        private readonly Dictionary<string, Delegate> _eventHandlers = new();

        /// <summary>
        /// 注册事件处理方法
        /// <para>⚠️ 同一事件的多个处理程序按注册顺序执行</para>
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">事件唯一标识</param>
        /// <param name="handler">事件处理方法（必须返回 object）</param>
        public void RegisterEvent<T>(string eventName, Func<T, object> handler)
        {
            if (!_eventHandlers.TryAdd(eventName, handler))
                _eventHandlers[eventName] = Delegate.Combine(_eventHandlers[eventName], handler);
        }

        /// <summary>
        /// 注销指定事件的单个处理程序
        /// <para>⚠️ 需要提供与注册时完全相同的委托实例</para>
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">目标事件名称</param>
        /// <param name="handler">要移除的处理程序</param>
        public void UnregisterEvent<T>(string eventName, Func<T, object> handler)
        {
            if (_eventHandlers.ContainsKey(eventName))
                _eventHandlers[eventName] = Delegate.Remove(_eventHandlers[eventName], handler);
        }

        /// <summary>
        /// 触发指定事件并收集所有处理程序返回值
        /// <para>⚠️ 事件参数类型必须与注册时一致</para>
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventName">目标事件名称</param>
        /// <param name="args">事件参数对象</param>
        /// <returns>所有处理程序返回值的列表（可能包含 null）</returns>
        public List<object> TriggerEvent<T>(string eventName, T args)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var eventHandler))
                return new List<object>();

            List<object> results = new List<object>();
            foreach (Delegate handler in eventHandler.GetInvocationList())
            {
                if (handler is Func<T, object> typedHandler)
                {
                    try 
                    {
                        results.Add(typedHandler(args));
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsDebugging)
                        {
                            Debug.LogError($"执行事件 {eventName} 时发生异常: {ex}");
                        }
                    }
                }
                else
                {
                    if (Debugger.IsDebugging)
                    {
                        Debug.LogError($"事件 {eventName} 的处理程序类型不匹配");
                    }
                }
            }
            return results;
        }


        /// <summary>
        /// 取消指定事件的所有处理程序
        /// <para>⚠️ 立即生效且不可逆</para>
        /// </summary>
        /// <param name="eventName">要取消的事件名称</param>
        public void CancelEvent(string eventName)
        {
            if (!_eventHandlers.ContainsKey(eventName)) return;
            _eventHandlers[eventName] = null;
            if (Debugger.IsDebugging)
            {
                Debug.Log($"事件 {eventName} 已取消。");
            }
        }

        /// <summary>
        /// 注销指定对象的所有事件订阅
        /// <para>用于对象销毁时自动清理订阅</para>
        /// </summary>
        /// <param name="targetObject">要清理的订阅者对象</param>
        /// <example>
        /// void OnDestroy() => EventManager.Instance.UnregisterAllEventsForObject(this);
        /// </example>
        public void UnregisterAllEventsForObject(object targetObject)
        {
            if (targetObject is null)
            {
                if (Debugger.IsDebugging)
                {
                    Debug.Log("已销毁物体无法取消订阅。");
                }
                return;
            }

            // 将事件名称存入列表以避免遍历时修改集合
            var eventNames = new List<string>(_eventHandlers.Keys);

            foreach (var eventName in eventNames)
            {
                var eventHandler = _eventHandlers[eventName];
                if (eventHandler == null) continue;

                var handlers = eventHandler.GetInvocationList();

                // 遍历并移除与目标对象相关的处理程序
                foreach (var handler in handlers)
                    if (handler.Target == targetObject)
                        _eventHandlers[eventName] = Delegate.Remove(_eventHandlers[eventName], handler);

                // 检查并移除空的事件
                if (_eventHandlers[eventName] == null || _eventHandlers[eventName].GetInvocationList().Length == 0)
                    _eventHandlers.Remove(eventName);
            }
            if (Debugger.IsDebugging)
            {
                Debug.Log($"已为 {targetObject} 注销所有事件订阅。");
            }
        }


        /// <summary>
        /// 清空所有事件注册信息
        /// <para>⚠️ 通常在场景切换时调用</para>
        /// </summary>
        public void UnregisterAllEvents()
        {
            _eventHandlers.Clear();
            if (Debugger.IsDebugging)
            {
                Debug.Log("所有事件订阅已被注销。");
            }
            
        }

        /// <summary>
        /// 通过反射自动注册带 [EventSubscribe] 特性的方法
        /// <para>要求方法格式：object Method(T args)</para>
        /// </summary>
        /// <param name="target">包含订阅方法的对象实例</param>
        /// <example>
        /// public class Player {
        ///     [EventSubscribe("PlayerHurt")]
        ///     private object OnHurt(HurtEventArgs args) { ... }
        /// }
        /// EventManager.Instance.RegisterEventHandlersFromAttributes(player);
        /// </example>
        public void RegisterEventHandlersFromAttributes(object target)
        {
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(EventSubscribeAttribute), false);
                foreach (var attribute in attributes)
                {
                    if (attribute is not EventSubscribeAttribute eventSubscribe) continue;
                    var eventName = eventSubscribe.EventName;
                    var handler = Delegate.CreateDelegate(
                        typeof(Func<,>).MakeGenericType(method.GetParameters()[0].ParameterType, method.ReturnType),
                        target, method);
                    if (!_eventHandlers.TryAdd(eventName, handler))
                        _eventHandlers[eventName] = Delegate.Combine(_eventHandlers[eventName], handler);
                }
            }
        }
    }

    /// <summary>
    /// 事件订阅特性（用于自动注册处理方法）
    /// <para>标记在符合格式的方法上：public object Method(T args)</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventSubscribeAttribute : Attribute
    {
        /// <summary>
        /// 创建事件订阅特性实例
        /// </summary>
        /// <param name="eventName">要订阅的事件名称</param>
        public EventSubscribeAttribute(string eventName)
        {
            EventName = eventName;
        }

        /// <summary>
        /// 目标事件名称
        /// </summary>
        public string EventName { get; }
    }
}