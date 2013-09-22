using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Util
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 从缓存服务中获取一个对象
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns>返回从缓存中得到的对象</returns>
        object Get(string key);

        /// <summary>
        /// 从缓存服务中获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键值</param>
        /// <returns>返回从缓存中得到的对象</returns>
        //T Get<T>(string key);

        /// <summary>
        /// 添加一个对象到缓存中
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存对象</param>
        /// <returns>返回操作是否成功</returns>
        void Set(string key, object value);

        /// <summary>
        /// 添加一个对象到缓存中
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存对象</param>
        /// <param name="minute">缓存过期时间(单位:分钟)</param>
        /// <returns>返回操作是否成功</returns>
        void Set(string key, object value, int minute);

        /// <summary>
        /// 从缓存中删除一个对象
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns>返回操作是否成功</returns>
        void Remove(string key);
    }
}
