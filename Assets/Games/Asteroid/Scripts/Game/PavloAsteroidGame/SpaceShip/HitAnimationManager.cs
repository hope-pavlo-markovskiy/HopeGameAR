using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip
{
    [Serializable]
    public class HitAnimationManager
    {
        [Header("ForceField")]
            [SerializeField] private GameObject _forceField;
            [SerializeField] private Material _forceFieldMat;
            [SerializeField] private float _forceFieldFlashTime;
            [SerializeField] private float _forceFieldPowerOn = 6.95f;
            [SerializeField] private float _forceFieldPowerOff = 30f;
        
            [Header("KnockBack")]
            [SerializeField] private float _knockBackDist;
            [SerializeField] private float _knockBackTime;
            [SerializeField] private float _returnTime;

            private GameObject _currentShip;

            public void Init(GameObject currentShip)
            {
                _currentShip = currentShip;
            }
            
            public async void KnockBack(GameObject gameObject)
            {
                Vector3 hitPos = _currentShip.transform.position;
                Vector3 knockBackDir = (hitPos - gameObject.transform.position).normalized;
                Vector3 knockBackPos = hitPos + (knockBackDir * _knockBackDist);
            
                knockBackPos.y = hitPos.y;
                knockBackPos.z = Mathf.Clamp(knockBackPos.z, -_knockBackDist, hitPos.z);
                await AnimateMove(hitPos, knockBackPos, _knockBackTime);
                Recovery(knockBackPos, hitPos);
            }

            private async void Recovery(Vector3 knockBackPos, Vector3 hitPos)
            {
                Vector3 recoveryPos = new (knockBackPos.x, hitPos.y, hitPos.z);
                await AnimateMove(knockBackPos, recoveryPos, _returnTime);
            }
        
            public async void ForceFieldFlashAnim()
            {
                _forceField.SetActive(true);
            
                float halfTime = _forceFieldFlashTime / 2f;
                await ForceFieldTransition(_forceFieldPowerOff, _forceFieldPowerOn, halfTime);
                await ForceFieldTransition(_forceFieldPowerOn, _forceFieldPowerOff, halfTime);
            
                _forceField.SetActive(false);
            }

            private async Task ForceFieldTransition(float startPower, float endPower, float time)
            {
                float percent = 0f;
                while (percent < 1f)
                {
                    percent += 1f / time * Time.deltaTime;
                    _forceFieldMat.SetFloat("_Power", Mathf.Lerp(startPower, endPower, time));

                    await Task.Yield();
                }
                _forceFieldMat.SetFloat("_Power", endPower);
            }
        
            private async Task AnimateMove(Vector3 startPos, Vector3 endPos, float time)
            {
                float percent = 0f;
                while (percent < 1f)
                {
                    percent += 1f / time * Time.deltaTime;
                    if (_currentShip.transform.position != null)
                    {
                        _currentShip.transform.position = Vector3.Lerp(startPos, endPos, percent);
                    }

                    await Task.Yield();
                }
                _currentShip.transform.position = endPos;
            }
    }
}