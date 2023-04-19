using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ValueManager : MonoBehaviour
{
    public Dictionary<string, ValueWrapper> Values = new();

    public class ValueWrapper
    {
        protected delegate dynamic GenericGetValue();
        protected delegate void GenericSetValue(dynamic value);

        protected GenericGetValue Getter;
        protected GenericSetValue Setter;
        protected ValueWrapper(GenericGetValue getter, GenericSetValue setter)
        {
            Getter = getter;
            Setter = setter;
        }
        protected dynamic Value
        {
            get => Getter();
            set => Setter(value);
        }
    }
    public class ValueWrapper<T> : ValueWrapper
    {
        public delegate T GetValue();
        public delegate void SetValue(T value);
        public ValueWrapper(GetValue getter, SetValue setter) : base(() => getter(), v => setter(v))
        {

        }
        public new T Value
        {
            get => base.Value;
            set => base.Value = value;
        }
    }

    public ContinuousTurnProviderBase RotationControl;
    private float defaultTurnSpeed;
    void Start()
    {
        defaultTurnSpeed = RotationControl.turnSpeed;
        Values["RotSpeed"] = new ValueWrapper<float>(() =>
        {
            return 100 * defaultTurnSpeed / RotationControl.turnSpeed;
        }, (value) =>
        {
            RotationControl.turnSpeed = value * defaultTurnSpeed / 100;
        });
    }

    void Update()
    {
        
    }
}
