using System;
using TMPro;
using UnityEngine;

namespace HatchStudio.Localization
{
    public class LocalizeStringEvent : MonoBehaviour
    {
        [SerializeField] private LString m_LString = new LString();
        [SerializeField] private TextMeshProUGUI m_Text;
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
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if(m_Text == null)
                m_Text = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}
