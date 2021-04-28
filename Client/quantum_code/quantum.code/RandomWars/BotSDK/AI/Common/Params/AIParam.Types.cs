using Photon.Deterministic;
using System;

namespace Quantum
{
  [System.Serializable]
  public unsafe sealed class AIParamInt : AIParam<int> 
  {
    public static implicit operator AIParamInt(int value) { return new AIParamInt() { DefaultValue = value }; }

    protected override int GetBlackboardValue(BlackboardValue value)
    {
      return *value.IntegerValue;
    }

    protected override int GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.Integer;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamBool : AIParam<bool> 
  {
    public static implicit operator AIParamBool(bool value) { return new AIParamBool() { DefaultValue = value }; }

    protected override bool GetBlackboardValue(BlackboardValue value)
    {
      return *value.BooleanValue;
    }

    protected override bool GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.Boolean;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamByte : AIParam<byte> 
  {
    public static implicit operator AIParamByte(byte value) { return new AIParamByte() { DefaultValue = value }; }

    protected override byte GetBlackboardValue(BlackboardValue value)
    {
      return *value.ByteValue;
    }

    protected override byte GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.Byte;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamFP : AIParam<FP> 
  {
    public static implicit operator AIParamFP(FP value) { return new AIParamFP() { DefaultValue = value }; }

    protected override FP GetBlackboardValue(BlackboardValue value)
    {
      return *value.FPValue;
    }

    protected override FP GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.FP;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamFPVector2 : AIParam<FPVector2> 
  {
    public static implicit operator AIParamFPVector2(FPVector2 value) { return new AIParamFPVector2() { DefaultValue = value }; }

    protected override FPVector2 GetBlackboardValue(BlackboardValue value)
    {
      return *value.FPVector2Value;
    }

    protected override FPVector2 GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.FPVector2;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamFPVector3 : AIParam<FPVector3> 
  {
    public static implicit operator AIParamFPVector3(FPVector3 value) { return new AIParamFPVector3() { DefaultValue = value }; }

    protected override FPVector3 GetBlackboardValue(BlackboardValue value)
    {
      return *value.FPVector3Value;
    }

    protected override FPVector3 GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.FPVector3;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamString : AIParam<string> 
  {
    public static implicit operator AIParamString(string value) { return new AIParamString() { DefaultValue = value }; }

    protected override string GetBlackboardValue(BlackboardValue value)
    {
      throw new NotSupportedException("Blackboard variables as strings are not supported.");
    }

    protected override string GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.String;
    }
  }

  [System.Serializable]
  public unsafe sealed class AIParamEntityRef : AIParam<EntityRef> 
  {
    public static implicit operator AIParamEntityRef(EntityRef value) { return new AIParamEntityRef() { DefaultValue = value }; }

    protected override EntityRef GetBlackboardValue(BlackboardValue value)
    {
      return *value.EntityRefValue;
    }

    protected override EntityRef GetConfigValue(AIConfig.KeyValuePair config)
    {
      return config.Value.EntityRef;
    }
  }
}
