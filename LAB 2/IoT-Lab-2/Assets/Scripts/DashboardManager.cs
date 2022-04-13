using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace IoT_dashboard
{
    public class DashboardManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasLayer1;
        [SerializeField]
        private InputField broker;
        [SerializeField]
        private InputField username;
        [SerializeField]
        private InputField password;
        [SerializeField]
        private InputField temp;
        [SerializeField]
        private InputField humid;
        [SerializeField]
        private Toggle LEDToggle;
        [SerializeField]
        private Toggle PumpToggle;


        [SerializeField]
        private CanvasGroup _canvasLayer2;
        private Tween twenFade;

        public void Start()
        {
            broker.text = "mqttserver.tk";
            username.text = "bkiot";
            password.text = "12345678";
        }


        public void Update_Status(Status_Data _status_data)
        {
            temp.text = "NHIỆT ĐỘ\n" + _status_data.temperature.ToString() + "°C";
            humid.text = "ĐỘ ẨM\n" + _status_data.humidity.ToString() + "%";
        }


        public ControlDevice_Data OnLEDValueChange(ControlDevice_Data _deviceStatus)
        {
            _deviceStatus.device = "LED";
            _deviceStatus.status = (LEDToggle.isOn ? "ON" : "OFF");
            Debug.Log(_deviceStatus);
            return _deviceStatus;
        }

        public ControlDevice_Data OnPumpValueChange(ControlDevice_Data _deviceStatus)
        {
            _deviceStatus.device = "PUMP";
            _deviceStatus.status = (PumpToggle.isOn ? "ON" : "OFF");
            Debug.Log(_deviceStatus);
            return _deviceStatus;
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _IESwitchLayer()
        {
            if (_canvasLayer1.interactable == true)
            {
                FadeOut(_canvasLayer1, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }

    }
}
