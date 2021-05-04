using RandomWarsResource;
using RandomWarsResource.Data;

namespace Quantum {
  public partial class FrameContextUser
  {
      public QuantumCodeTableDataContainer TableData;
      public FieldPositions FieldPositions;
      
      public void SetData(QuantumCodeTableDataContainer tableData)
      {
          TableData = tableData;
      }
      
      public void SetFieldPositions(FieldPositions fieldPositions)
      {
          FieldPositions = fieldPositions;
      }
  }

  //INFO: 퀀텀 코드에서 사용하는 테이블데이터를 담는 클래스
  public class QuantumCodeTableDataContainer
  {
      public TableData<int, TDataVsmode> VsMode;
      public TableData<int, TDataDiceInfo> DiceInfo;
      public TableData<int, TDataGuardianInfo> GuardianInfo;
      public TableData<int, TDataCoopMode> CoopMode;
  }
}