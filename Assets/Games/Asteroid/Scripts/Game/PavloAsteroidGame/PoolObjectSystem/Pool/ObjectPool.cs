using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.PoolObjectSystem.Pool
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private Transform _container;
        
        private readonly List<GameObject> _enabledPool = new();
        private readonly List<GameObject> _disabledPool = new();

        public List<GameObject> EnabledPool => _enabledPool;
        public List<GameObject> DisabledPool => _disabledPool;

        public void InitializePool(int sizeOfPool, GameObject prefab, Transform container)
        {
            _container = container;
            _prefab = prefab;
            for (int i = 0; i < sizeOfPool; i++)
            {
                GameObject obj = Object.Instantiate(_prefab, _container, true);
                obj.gameObject.SetActive(false);
                _disabledPool.Add(obj);
            }
        }
        
        public GameObject GetComponent()
        {
            GameObject obj;
            if (_disabledPool.Count > 0)
            {
                obj = _disabledPool[^1];
                _disabledPool.Remove(obj);
            }
            else
            {
                obj = Object.Instantiate(_prefab, _container,true);
            }
            _enabledPool.Add(obj);
        
            return obj;
        }

        public void DisableComponent(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            _disabledPool.Add(obj);
            _enabledPool.Remove(obj);

        }
    
    }
    public class ObjectPool<T> where T : Component
    {
        private T _prefab;
    
        private readonly List<T> _enabledPool = new();
        private readonly List<T> _disabledPool = new();

        public List<T> EnabledPool => _enabledPool;
        public List<T> DisabledPool => _disabledPool;

        public void InitializePool(int sizeOfPool, T prefab)
        {
            _prefab = prefab;
            for (int i = 0; i < sizeOfPool; i++)
            {
                T obj = Object.Instantiate(_prefab);
                obj.gameObject.SetActive(false);
                _disabledPool.Add(obj);
            }
        }
        
        public T GetComponent()
        {
            T obj;
            if (_disabledPool.Count > 0)
            {
                obj = _disabledPool[^1];
                _disabledPool.Remove(obj);
            }
            else
            {
                obj = Object.Instantiate(_prefab);
            }
            _enabledPool.Add(obj);
        
            return obj;
        }

        public void DisableComponent(T obj)
        {
            obj.gameObject.SetActive(false);
            _disabledPool.Add(obj);
            _enabledPool.Remove(obj);

        }
    
    }
    
}
