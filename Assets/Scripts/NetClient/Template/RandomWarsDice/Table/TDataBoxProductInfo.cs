using System;
using Service.Template.Table;


namespace Template.Item.RandomWarsDice.Table
{
    public class TDataBoxProductInfo : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public int boxId { get; set; }
        public int userRankGrade { get; set; }
        public int gold { get; set; }
        public int dia { get; set; }
        public int rewardCardGradeType1 { get; set; }
        public int rewardCardGradeCnt1 { get; set; }
        public int[] rewardCardValue1 { get; set; }
        public bool rewardIsProbability1 { get; set; }
        public int rewardProbability1 { get; set; }
        public int rewardCardGradeType2 { get; set; }
        public int rewardCardGradeCnt2 { get; set; }
        public int[] rewardCardValue2 { get; set; }
        public bool rewardIsProbability2 { get; set; }
        public int rewardProbability2 { get; set; }
        public int rewardCardGradeType3 { get; set; }
        public int rewardCardGradeCnt3 { get; set; }
        public int[] rewardCardValue3 { get; set; }
        public bool rewardIsProbability3 { get; set; }
        public int rewardProbability3 { get; set; }
        public int rewardCardGradeType4 { get; set; }
        public int rewardCardGradeCnt4 { get; set; }
        public int[] rewardCardValue4 { get; set; }
        public bool rewardIsProbability4 { get; set; }
        public int rewardProbability4 { get; set; }
        public int rewardCardGradeType5 { get; set; }
        public int rewardCardGradeCnt5 { get; set; }
        public int[] rewardCardValue5 { get; set; }
        public bool rewardIsProbability5 { get; set; }
        public int rewardProbability5 { get; set; }
        public int buyLimitCnt { get; set; }
        public bool isUse { get; set; }


        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            boxId = int.Parse(cols[2]);
            userRankGrade = int.Parse(cols[3]);
            gold = int.Parse(cols[4]);
            dia = int.Parse(cols[5]);
            rewardCardGradeType1 = int.Parse(cols[6]);
            rewardCardGradeCnt1 = int.Parse(cols[7]);
            rewardCardValue1 = Array.ConvertAll(cols[8].Split(','), delegate (string s) { return int.Parse(s); });
            rewardIsProbability1 = bool.Parse(cols[9]);
            rewardProbability1 = int.Parse(cols[10]);
            rewardCardGradeType2 = int.Parse(cols[11]);
            rewardCardGradeCnt2 = int.Parse(cols[12]);
            rewardCardValue2 = Array.ConvertAll(cols[13].Split(','), delegate (string s) { return int.Parse(s); });
            rewardIsProbability2 = bool.Parse(cols[14]);
            rewardProbability2 = int.Parse(cols[15]);
            rewardCardGradeType3 = int.Parse(cols[16]);
            rewardCardGradeCnt3 = int.Parse(cols[17]);
            rewardCardValue3 = Array.ConvertAll(cols[18].Split(','), delegate (string s) { return int.Parse(s); });
            rewardIsProbability3 = bool.Parse(cols[19]);
            rewardProbability3 = int.Parse(cols[20]);
            rewardCardGradeType4 = int.Parse(cols[21]);
            rewardCardGradeCnt4 = int.Parse(cols[22]);
            rewardCardValue4 = Array.ConvertAll(cols[23].Split(','), delegate (string s) { return int.Parse(s); });
            rewardIsProbability4 = bool.Parse(cols[24]);
            rewardProbability4 = int.Parse(cols[25]);
            rewardCardGradeType5 = int.Parse(cols[26]);
            rewardCardGradeCnt5 = int.Parse(cols[27]);
            rewardCardValue5 = Array.ConvertAll(cols[28].Split(','), delegate (string s) { return int.Parse(s); });
            rewardIsProbability5 = bool.Parse(cols[29]);
            rewardProbability5 = int.Parse(cols[30]);
            buyLimitCnt = int.Parse(cols[31]);
            isUse = bool.Parse(cols[32]);
        }
    }
}