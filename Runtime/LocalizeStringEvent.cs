using System;
using TMPro;
using UnityEngine;

namespace HatchStudio.Localization
{
    public class LocalizeStringEvent : MonoBehaviour
    {
        [SerializeField] private LString m_LString = new LString();
        private TextMeshProUGUI m_Text;
        Action _UnsubscribeAction;
        private void OnEnable()
        { 
            m_LString.SubscribeToLocalization((loc) =>
            {
                m_Text.text = loc;
            });
        }
        
        private void OnDisable()
        {
            m_LString.Dispose();
        }
    }
}
