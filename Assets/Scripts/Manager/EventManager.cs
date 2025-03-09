using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utils;

namespace Manager
{
    /*
     * 订阅事件的方法一定要是public且返回值为object
     */
    public class EventManager : Singleton<EventManager>
    {
        // 使用泛型事件字典来存储事件和处理程序
        private readonly Dictionary<string, Delegate> _eventHandlers = new();

        // 注册事件处理程序
        public void RegisterEvent<T>(string eventName, Func<T, object> handler)
        {
            if (!_eventHandlers.TryAdd(eventName, handler))
            {
                _eventHandlers[eventName] = Delegate.Combine(_eventHandlers[eventName], handler);
            }
        }

        // 注销事件处理程序
        public void UnregisterEvent<T>(string eventName, Func<T, object> handler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = Delegate.Remove(_eventHandlers[eventName], handler);
            }
        }

        // 触发事件并返回结果（带返回值版本，支持多个参数）
        public object TriggerEvent<T>(string eventName, T args)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var eventHandler)) return null;
            if (eventHandler is Func<T, object> handler)
            {
                return handler(args);
            }

            return null;
        }


        // 取消指定事件
        public void CancelEvent(string eventName)
        {
            if (!_eventHandlers.ContainsKey(eventName)) return;
            _eventHandlers[eventName] = null;
            Debug.Log($"事件 {eventName} 已取消。");
        }

        public void UnregisterAllEventsForObject(object targetObject)
        {
            // 将事件名称存入列表以避免遍历时修改集合
            var eventNames = new List<string>(_eventHandlers.Keys);

            foreach (var eventName in eventNames)
            {
                var eventHandler = _eventHandlers[eventName];
                if (eventHandler == null) continue;

                var handlers = eventHandler.GetInvocationList();

                // 遍历并移除与目标对象相关的处理程序
                foreach (var handler in handlers)
                {
                    if (handler.Target == targetObject)
                    {
                        _eventHandlers[eventName] = Delegate.Remove(_eventHandlers[eventName], handler);
                    }
                }

                // 检查并移除空的事件
                if (_eventHandlers[eventName] == null || _eventHandlers[eventName].GetInvocationList().Length == 0)
                {
                    _eventHandlers.Remove(eventName);
                }
            }

            Debug.Log($"已为 {targetObject} 注销所有事件订阅。");
        }


        // 注销所有事件
        public void UnregisterAllEvents()
        {
            _eventHandlers.Clear();
            Debug.Log("所有事件订阅已被注销。");
        }

        // 从带有 EventSubscribe 特性的方法中自动注册事件处理程序
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
                    {
                        _eventHandlers[eventName] = Delegate.Combine(_eventHandlers[eventName], handler);
                    }
                }
            }
        }
    }

    // 特性类，用于标记事件订阅方法
    [AttributeUsage(AttributeTargets.Method)]
    public class EventSubscribeAttribute : Attribute
    {
        public string EventName { get; }

        public EventSubscribeAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}