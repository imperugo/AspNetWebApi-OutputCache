﻿using System;
using System.Collections.Generic;
using ServiceStack.Redis;
using WebApi.OutputCache.Core.Cache;

namespace WebApi.OutputCache.Redis
{
	public class RedisCacheProvider : IApiOutputCache
	{
		private readonly IRedisClient _redisClient;

		public RedisCacheProvider(IRedisClient redisClient)
		{
			_redisClient = redisClient;
		}

		public void RemoveStartsWith(string key)
		{
			var keys = _redisClient.SearchKeys(string.Format("{0}*", key));
			_redisClient.RemoveAll(keys);
		}

		public T Get<T>(string key) where T : class
		{
			return _redisClient.Get<T>(key);
		}

		public object Get(string key)
		{
			var result = _redisClient.Get<object>(key);
			return result;
		}

		public void Remove(string key)
		{
			_redisClient.Remove(key);
		}

		public bool Contains(string key)
		{
			return _redisClient.ContainsKey(key);
		}

		public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
		{
			var primaryAdded = _redisClient.Add(key, o, expiration.Subtract(DateTimeOffset.Now));

			if (dependsOnKey != null && primaryAdded)
			{
				_redisClient.AddItemToList(dependsOnKey, key);
			}
		}

		public IEnumerable<string> AllKeys
		{
			get { return _redisClient.GetAllKeys(); }
		}
	}
}