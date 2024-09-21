using System;
using System.Collections.Generic;

namespace RoundHero
{
    public class Attribute
    {
        public Dictionary<EHeroAttribute, float> Value = new Dictionary<EHeroAttribute, float>();

        public Attribute()
        {
            foreach (EHeroAttribute attribute in Enum.GetValues(typeof(EHeroAttribute)))
            {
                Value.Add(attribute, 0);
            }
        }

        public void SetAttribute(EHeroAttribute heroAttribute, float value)
        { 
            Value[heroAttribute] = (int)value;
        }
        
        public void ChangeAttribute(EHeroAttribute heroAttribute, float value)
        {

            Value[heroAttribute] = GetAttribute(heroAttribute) + value;
            
            

            if (Value[heroAttribute] <= 0.001f)
            {
                Value[heroAttribute] = 0;
            }
            Value[heroAttribute] = (int)Value[heroAttribute];
        }
        
        public float GetAttribute(EHeroAttribute heroAttribute)
        {
            return Value[heroAttribute];
        }
        
        public Attribute Copy()
        {
            var attribute = new Attribute();
            foreach (var kv in Value)
            {
                attribute.Value[kv.Key] = kv.Value;
            }

            return attribute;
        }
    }
}